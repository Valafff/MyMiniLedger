using Dapper;
using MyMiniLedger.BLL.Context;
using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.Windows.EditContinuePosition;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.WPF.WindowsModels
{
	//Блокировка события комбобоксов
	public delegate void LockDelegate();
	public delegate void UpdateDelegate();
	public class EditContinuePositionWindowsModel : BaseNotify
	{
		public event LockDelegate LockEvent; 
		public event UpdateDelegate UpdateEvent;

		private readonly Context _context;

		//Название окна
		private string _titleEditContinuePos = "Редактирование/Продолжение позиции";
		public string TitleEditContinuePosition
		{
			get => _titleEditContinuePos;
			set => SetField(ref _titleEditContinuePos, value);
		}

		//Модель выбранной позиции
		private PositionUIModel? _selectedPosition;
		public PositionUIModel? SelectedPosition
		{
			get => _selectedPosition;
			set => SetField(ref _selectedPosition, value);
		}

		//Оригинальная модель выбранной позиции - для отката изменений
		private PositionUIModel? _originalSelectedPosition;
		public PositionUIModel? OriginalSelectedPosition
		{
			get => _originalSelectedPosition;
			set => SetField(ref _originalSelectedPosition, value);
		}


		private PositionUIModel? _dataGridtSelectedItem;
		public PositionUIModel? DataGridSelectedItem
		{
			get => _dataGridtSelectedItem;
			set => SetField(ref _dataGridtSelectedItem, value);
		}

		//Определение выбранной позиции в cb_Category
		private string? _selectedCategory;
		public string? SelectedCategory
		{
			get => _selectedCategory;
			set => SetField(ref _selectedCategory, value);
		}

		private KindUIModel? _selectedKind;
		public KindUIModel? SelectedKind
		{
			get => _selectedKind;
			set => SetField(ref _selectedKind, value);
		}

		private int? _selectedKindIndex;
		public int? SelectedKindIndex
		{
			get => _selectedKindIndex;
			set => SetField(ref _selectedKindIndex, value);
		}

		private CoinUIModel? _selectedCoin;
		public CoinUIModel? SelectedCoin
		{
			get => _selectedCoin;
			set => SetField(ref _selectedCoin, value);
		}

		private string? _selectedStatus;
		public string? SelectedStatus
		{
			get => _selectedStatus;
			set => SetField(ref _selectedStatus, value);
		}

		//Позиции которые должны подтянуться, если позиция является комплексной, всегда подтягивается первая позиция
		//https://stackoverflow.com/questions/45292905/wpf-collectionview-error-collection-was-modified-enumeration-operation-may-no
		//https://stackoverflow.com/questions/23108045/how-to-make-observablecollection-thread-safe/29288294#29288294
		public ObservableCollection<PositionUIModel> SelectedPositions { get; set; }
		//public List<PositionUIModel> SelectedPositions { get; set; }

		//Ссылка на список позиций из главного окна
		public ObservableCollection<PositionUIModel> MAINPOSITIONSCOLLECTION { get; set; }

		//Служебные поля
		public ObservableCollection<CategoryUIModel> Categories { get; set; }
		//Для ограниченного выбора при фильтрации в комбобоксе отдельного окна
		public ObservableCollection<string> tempCategories { get; set; } = new ObservableCollection<string>();
		public ObservableCollection<CoinUIModel> Coins { get; set; }
		public ObservableCollection<KindUIModel> Kinds { get; set; }
		//Для ограниченного выбора при фильтрации в комбобоксе отдельного окна
		public ObservableCollection<KindUIModel> TempKinds { get; set; }
		public ObservableCollection<StatusUIModel> StatusesForService { get; set; }
		public ObservableCollection<StatusUIModel> StatusesForUser { get; set; }


		public LambdaCommand UndoSelectedPosition { get; set; }
		public LambdaCommand UpdatePosition { get; set; }
		public LambdaCommand AddComplexPosition { get; set; }

		public LambdaCommand SelectNewPosition { get; set; }


		public EditContinuePositionWindowsModel()
		{
			_context = new Context();

			//Иницмализируется в классе *xaml.cs
			//Создание выбранной позиции
			SelectedPosition = new PositionUIModel();
			OriginalSelectedPosition = new PositionUIModel();

			SelectedPositions = new ObservableCollection<PositionUIModel>();
			MAINPOSITIONSCOLLECTION = new ObservableCollection<PositionUIModel>();

			//Инициализация категорий
			Categories = CategoriesInicialization();

			//Инициализация монет
			Coins = CoinsInicialization();

			//Инициализация видов
			Kinds = KindsInicialization();

			TempKinds = new ObservableCollection<KindUIModel>();

			setTempCaregories(tempCategories, Categories);

			//Инициализация статусов
			BLL.Context.ListOfStatuses tempStat = new BLL.Context.ListOfStatuses();
			List<StatusUIModel> tempStatuses = tempStat.GetAllAsync().Result.Select(stat => Mappers.UIMapper.MapStatusBLLToStatusUI(stat)).ToList();
			StatusesForService = new ObservableCollection<StatusUIModel>(tempStatuses);
			StatusesForUser = new ObservableCollection<StatusUIModel>();
			setStatusesForUser(StatusesForService);

			UndoSelectedPosition = new LambdaCommand(execute =>
			{
				ValuePositionCopy(SelectedPosition, OriginalSelectedPosition);
				SelectedPositionsInicailization(SelectedPositions);
				UpdateEvent();
			},
			canExecute => SelectedPosition != null && OriginalSelectedPosition != null
			);

			UpdatePosition = new LambdaCommand(
			async execute =>
			{
				//Добавление новой позиции
				SelectedPosition.CloseDate = (ViewTools.FormatterPositions.SetCloseDate(SelectedPosition.Status.StatusName)).ToString();
				await _context.PositionsTableBL.UpdateAsync(Mappers.UIMapper.MapPositionUIToPositionBLL(SelectedPosition));
				UpdateEvent();
				double.TryParse(SelectedPosition.Income.ToString(), out double r1);
				double.TryParse(SelectedPosition.Expense.ToString(), out double r2);
				SelectedPosition.Saldo = (r1 - r2).ToString();
				SelectedPositionsInicailization(SelectedPositions);

			},
				canExecute => SelectedPosition.Kind is not null && SelectedPosition.Income != null && SelectedPosition.Expense != null
			);

			AddComplexPosition = new LambdaCommand(

				async execute =>
				{
					//Добавление новой комплексной позиции
				},
				canExecute => SelectedPosition.Kind is not null && SelectedPosition.Income != null && SelectedPosition.Expense != null && SelectedPosition.Status.StatusName == "Открыта"
			);

			//Активация через костыль в кодбехайнд
			SelectNewPosition = new LambdaCommand(

				async execute =>
					{
						ValuePositionCopy(SelectedPosition, DataGridSelectedItem);

						LockEvent();
						TempKindInicialization();
						UpdateEvent();
	
					},
				canExecute => true
				);
		}

		//Методы класса
		void setStatusesForUser(ObservableCollection<StatusUIModel> _input)
		{
			foreach (var stat in _input)
			{
				if (stat.StatusName == "Open")
				{
					stat.StatusName = "Открыта";
					StatusesForUser.Add(stat);
				}
				else if (stat.StatusName == "Closed")
				{
					stat.StatusName = "Закрыта";
					StatusesForUser.Add(stat);
				}
				else if (stat.StatusName == "Deleted")
				{
					continue;
				}
				else
				{
					StatusesForUser.Add(stat);
				}
			}
		}


		//Инициализация временных категорий для добавления новой позиции
		public void setTempCaregories(ObservableCollection<string> t, ObservableCollection<CategoryUIModel> _cat)
		{
			foreach (var stat in _cat)
			{
				t.Add(stat.Category);
			}
		}

		//Инициализация категорий
		public ObservableCollection<CategoryUIModel> CategoriesInicialization()
		{
			BLL.Context.ListOfCategories tempCat = new BLL.Context.ListOfCategories();
			List<CategoryUIModel> tempCategoriesAsync = tempCat.GetAllAsync().Result.Select(cat => Mappers.UIMapper.MapCategoryBLLToCategoryUI(cat)).ToList();
			return new ObservableCollection<CategoryUIModel>(tempCategoriesAsync);
		}

		//Инициализация монет
		public ObservableCollection<CoinUIModel> CoinsInicialization()
		{
			BLL.Context.ListOfCoins tempCoin = new BLL.Context.ListOfCoins();
			List<CoinUIModel> tempCoins = tempCoin.GetAllAsync().Result.Select(coin => Mappers.UIMapper.MapCoinBLLToCoinUI(coin)).ToList();
			return new ObservableCollection<CoinUIModel>(tempCoins);
		}

		//Инициализация выбранных позиций
		public void SelectedPositionsInicailization(ObservableCollection<PositionUIModel> _selectedPositions, int _flagSource = 0)
		{
			PositionUIModel sp = new PositionUIModel();
			if (_flagSource == 0)
			{
				sp = SelectedPosition;
			}
			else
			{
				sp = (PositionUIModel)OriginalSelectedPosition.Clone();
			}

			////Чтение позиций из БД
			//BLL.Context.ListOfPositions tempPos = new BLL.Context.ListOfPositions();
			//List<PositionUIModel> loadedPositions = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList());
			////Форматирование
			//loadedPositions = ViewTools.FormatterPositions.EditPosFromTableByDate(loadedPositions.AsList(), new DateTime(2000, 01, 01));
			//loadedPositions = ViewTools.FormatterPositions.EditPosFromTableByStatus(loadedPositions.AsList(), "Open", "Открыта");
			//loadedPositions = ViewTools.FormatterPositions.EditPosFromTableByStatus(loadedPositions.AsList(), "Closed", "Закрыта");
			//loadedPositions = ViewTools.FormatterPositions.EditDecemalPosFromTableByMarker(loadedPositions.AsList(), "fiat", "0.00");

			_selectedPositions.Clear();
			//Если позиция простая или выбран родитель
			if (sp.ZeroParrentKey == null)
			{
				//List<PositionUIModel> filtredList = loadedPositions.FindAll(f => f.ZeroParrentKey == sp.PositionKey);
				List<PositionUIModel> filtredList = MAINPOSITIONSCOLLECTION.AsList().FindAll(f => f.ZeroParrentKey == sp.PositionKey);

				_selectedPositions.Add(sp);
				filtredList = (filtredList.OrderBy(f => f.ParrentKey)).ToList();
				foreach (var item in filtredList)
				{
					if (item.Status.StatusName != "Deleted")
					{
						_selectedPositions.Add(item);
					}
				}
			}
			//Если выбрана дочерняя позиция
			else
			{
				//var childs = loadedPositions.FindAll(f => f.ZeroParrentKey == sp.ZeroParrentKey);
				//var parrent = loadedPositions.Find(f => f.PositionKey == sp.ZeroParrentKey);
				var childs = MAINPOSITIONSCOLLECTION.AsList().FindAll(f => f.ZeroParrentKey == sp.ZeroParrentKey);
				var parrent = MAINPOSITIONSCOLLECTION.AsList().Find(f => f.PositionKey == sp.ZeroParrentKey);
				childs.Add(parrent);
				childs = (childs.OrderBy(f => f.ParrentKey)).ToList();
				foreach (var item in childs)
				{
					if (item.Status.StatusName != "Deleted")
					{
						_selectedPositions.Add(item);
					}
				}
			}
		}

		//Инициализация видов
		public ObservableCollection<KindUIModel> KindsInicialization()
		{
			BLL.Context.ListOfKinds tempKind = new BLL.Context.ListOfKinds();
			List<KindUIModel> _tempKinds = tempKind.GetAllAsync().Result.Select(kind => Mappers.UIMapper.MapKindBLLToKindUI(kind)).ToList();
			return new ObservableCollection<KindUIModel>(_tempKinds);
		}

		//Инициализация всех видов которые находятся в категории выбранной позиции
		public void TempKindInicialization()
		{
			try
			{
				if (TempKinds != null)
				{
					var cloneKind = (KindUIModel)SelectedPosition.Kind.Clone();
					TempKinds.Clear();
					SelectedPosition.Kind = cloneKind;

					if (SelectedPosition != null)
					{
						foreach (var item in Kinds)
						{
							if (item.Category.Id == SelectedPosition.Kind.Category.Id)
							{
								TempKinds.Add((KindUIModel)item.Clone());
							}
						}
					}
					SelectedCategory = SelectedPosition.Kind.Category.Category;

					for (int i = 0; i < TempKinds.Count; i++)
					{
						if (TempKinds[i].Kind == SelectedPosition.Kind.Kind)
						{
							SelectedKindIndex = i;
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{ex.Message} При попытке инициализировать временные позиции");
				SelectedPositionsInicailization(SelectedPositions, 1);
				Console.WriteLine($"Список временных позиций перестроен");
			}
		}

		//Инициализация временных видов по выбранной категории 
		public void TempKindInicializationOfCategory()
		{
			try
			{
				if (TempKinds != null)
				{
					var cloneKind = (KindUIModel)SelectedPosition.Kind.Clone();
					TempKinds.Clear();
					SelectedPosition.Kind = cloneKind;

					//var clonePosition = (PositionUIModel)SelectedPosition.Clone();
					//TempKinds.Clear();
					//ValuePositionCopy(SelectedPosition, clonePosition);

					if (SelectedCategory != null)
					{
						foreach (var item in Kinds)
						{
							if (item.Category.Category == SelectedCategory)
							{
								TempKinds.Add((KindUIModel)item.Clone());
							}
						}

						//for (int i = 0; i < TempKinds.Count; i++)
						//{
						//	if (TempKinds[i].Kind == SelectedPosition.Kind.Kind)
						//	{
						//		SelectedKindIndex = i;
						//		break;
						//	}
						//}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public void TempKindInicializationTextInput()
		{
			try
			{
				if (TempKinds != null)
				{
					//var cloneKind = (KindUIModel)SelectedPosition.Kind.Clone();
					TempKinds.Clear();
					//SelectedPosition.Kind = cloneKind;
					foreach (var item in Kinds)
					{
						TempKinds.Add((KindUIModel)item.Clone());
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		void ValuePositionCopy(PositionUIModel _tocopy, PositionUIModel _fromCopy)
		{
			_tocopy.Id = _fromCopy.Id;
			_tocopy.PositionKey = _fromCopy.PositionKey;
			_tocopy.OpenDate = _fromCopy.OpenDate;
			_tocopy.CloseDate = _fromCopy.CloseDate;

			//_tocopy.Kind = _fromCopy.Kind;
			_tocopy.Kind.Id = _fromCopy.Kind.Id;
			_tocopy.Kind.Kind = _fromCopy.Kind.Kind;
			_tocopy.Kind.Category.Id = _fromCopy.Kind.Category.Id;
			_tocopy.Kind.Category.Category = _fromCopy.Kind.Category.Category;
			_tocopy.Kind.Category.RefNumber = _fromCopy.Kind.Category.RefNumber;

			_tocopy.Income = _fromCopy.Income;
			_tocopy.Expense = _fromCopy.Expense;
			_tocopy.Saldo = _fromCopy.Saldo;

			//_tocopy.Coin = _fromCopy.Coin;
			_tocopy.Coin.Id = _fromCopy.Coin.Id;
			_tocopy.Coin.FullName = _fromCopy.Coin.FullName;
			_tocopy.Coin.ShortName = _fromCopy.Coin.ShortName;
			_tocopy.Coin.RefNumber = _fromCopy.Coin.RefNumber;
			_tocopy.Coin.CoinNotes = _fromCopy.Coin.CoinNotes;

			//_tocopy.Status = _fromCopy.Status;
			_tocopy.Status.Id = _fromCopy.Status.Id;
			_tocopy.Status.StatusName = _fromCopy.Status.StatusName;
			_tocopy.Status.StatusNotes = _fromCopy.Status.StatusNotes;

			_tocopy.Tag = _fromCopy.Tag;
			_tocopy.Notes = _fromCopy.Notes;
			_tocopy.ZeroParrentKey = _fromCopy.ZeroParrentKey;
			_tocopy.ParrentKey = _fromCopy.ParrentKey;
		}

		void ValueKindCopy(KindUIModel _tocopy, KindUIModel _fromCopy)
		{
			_tocopy.Id = _fromCopy.Id;
			_tocopy.Kind = _fromCopy.Kind;
			_tocopy.Category.Id = _fromCopy.Category.Id;
			_tocopy.Category.Category = _fromCopy.Category.Category;
			_tocopy.Category.RefNumber = _fromCopy.Category.RefNumber;
		}
	}

}
