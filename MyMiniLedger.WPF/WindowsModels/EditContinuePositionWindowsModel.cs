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
	//public delegate void ResetDelegate();
	public delegate void UpdateDelegate();
	public class EditContinuePositionWindowsModel : BaseNotify
	{
		//public event ResetDelegate ResetEvent; все поставил на updateevent
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

		private CoinUIModel? _selectedCoin;
		public CoinUIModel? SelectedCoin
		{
			get => _selectedCoin;
			set => SetField(ref _selectedCoin, value);
		}

		//Позиции которые должны подтянуться, если позиция является комплексной, всегда подтягивается первая позиция
		//https://stackoverflow.com/questions/45292905/wpf-collectionview-error-collection-was-modified-enumeration-operation-may-no
		//https://stackoverflow.com/questions/23108045/how-to-make-observablecollection-thread-safe/29288294#29288294
		public ObservableCollection<PositionUIModel> SelectedPositions { get; set; }
		//public List<PositionUIModel> SelectedPositions { get; set; }




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
				SelectedPosition.Id = OriginalSelectedPosition.Id;
				SelectedPosition.PositionKey = OriginalSelectedPosition.PositionKey;
				SelectedPosition.OpenDate = OriginalSelectedPosition.OpenDate;
				SelectedPosition.CloseDate = OriginalSelectedPosition.CloseDate;
				SelectedPosition.Kind = OriginalSelectedPosition.Kind;
				SelectedPosition.Income = OriginalSelectedPosition.Income;
				SelectedPosition.Expense = OriginalSelectedPosition.Expense;
				SelectedPosition.Saldo = OriginalSelectedPosition.Saldo;
				SelectedPosition.Coin = OriginalSelectedPosition.Coin;
				SelectedPosition.Status = OriginalSelectedPosition.Status;
				SelectedPosition.Tag = OriginalSelectedPosition.Tag;
				SelectedPosition.Notes = OriginalSelectedPosition.Notes;
				SelectedPosition.ZeroParrentKey = OriginalSelectedPosition.ZeroParrentKey;
				SelectedPosition.ParrentKey = OriginalSelectedPosition.ParrentKey;
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
						//Передается через SelectedItem в xaml DataGrid
						SelectedPosition = DataGridSelectedItem;
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
		public void SelectedPositionsInicailization(ObservableCollection<PositionUIModel> _selectedPositions)
		{
			//Чтение позиций из БД
			BLL.Context.ListOfPositions tempPos = new BLL.Context.ListOfPositions();
			List<PositionUIModel> loadedPositions = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList());
			//Форматирование
			loadedPositions = ViewTools.FormatterPositions.EditPosFromTableByDate(loadedPositions.AsList(), new DateTime(2000, 01, 01));
			loadedPositions = ViewTools.FormatterPositions.EditPosFromTableByStatus(loadedPositions.AsList(), "Open", "Открыта");
			loadedPositions = ViewTools.FormatterPositions.EditPosFromTableByStatus(loadedPositions.AsList(), "Closed", "Закрыта");
			loadedPositions = ViewTools.FormatterPositions.EditDecemalPosFromTableByMarker(loadedPositions.AsList(), "fiat", "0.00");

			_selectedPositions.Clear();
			//Если позиция простая или выбран родитель
			if (SelectedPosition.ZeroParrentKey == null)
			{
				List<PositionUIModel> filtredList = loadedPositions.FindAll(f => f.ZeroParrentKey == SelectedPosition.PositionKey);

				_selectedPositions.Add(SelectedPosition);
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
				var childs = loadedPositions.FindAll(f => f.ZeroParrentKey == SelectedPosition.ZeroParrentKey);
				var parrent = loadedPositions.Find(f => f.PositionKey == SelectedPosition.ZeroParrentKey);
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
								TempKinds.Add(item);
							}
						}
					}
					SelectedCategory = SelectedPosition.Kind.Category.Category;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
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

					if (SelectedCategory != null)
					{
						foreach (var item in Kinds)
						{
							if (item.Category.Category == SelectedCategory)
							{
								TempKinds.Add(item);
							}
						}
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
						TempKinds.Add(item);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}

}
