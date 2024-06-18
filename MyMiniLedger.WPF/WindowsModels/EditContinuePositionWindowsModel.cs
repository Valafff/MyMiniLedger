using Dapper;
using MyMiniLedger.BLL.Context;
using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.ViewTools;
using MyMiniLedger.WPF.Windows.EditContinuePosition;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.WPF.WindowsModels
{
	//Блокировка события комбобоксов
	public delegate void UpdateDelegate();
	public delegate void LockDelegate();
	public class EditContinuePositionWindowsModel : BaseNotify
	{
		public event UpdateDelegate UpdateEvent;
		public event LockDelegate LockEvent;

		private ComplexPositionHendler complexPositionsHendler = new ComplexPositionHendler();

		bool block = false;

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

		private string? _selectedKind;
		public string? SelectedKind
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

		//Для корректного переводы в нужный формат
		private DateTime? _tempselectedOpenDate;
		public DateTime? TempSelectedOpenDate
		{
			get => _tempselectedOpenDate;
			set => SetField(ref _tempselectedOpenDate, value);
		}

		private string? _selectedOpenDate;
		public string? SelectedOpenDate
		{
			get => _selectedOpenDate;
			set => SetField(ref _selectedOpenDate, value);
		}

		private string? _selectedCloseDate;
		public string? SelectedCloseDate
		{
			get => _selectedCloseDate;
			set => SetField(ref _selectedCloseDate, value);
		}

		private string? _textKind;
		public string? TextKind
		{
			get => _textKind;
			set => SetField(ref _textKind, value);
		}




		//Тупо записывается название цвета через биндинг

		//один на опен и клоус дэйт
		private string? _tb_date_color;
		public string? TB_DateColor
		{
			get => _tb_date_color;
			set => SetField(ref _tb_date_color, value);
		}

		private string? _tb_category_color;
		public string? TB_CategoryColor
		{
			get => _tb_category_color;
			set => SetField(ref _tb_category_color, value);
		}

		private string? _tb_kind_color;
		public string? TB_KindColor
		{
			get => _tb_kind_color;
			set => SetField(ref _tb_kind_color, value);
		}

		private string? _tb_coincolor;
		public string? TB_CoinColor
		{
			get => _tb_coincolor;
			set => SetField(ref _tb_coincolor, value);
		}

		private string? _tb_statuscolor;
		public string? TB_StatusColor
		{
			get => _tb_statuscolor;
			set => SetField(ref _tb_statuscolor, value);
		}





		//Названия полей позиции для комбобокса
		private string? _selectedCoin;
		public string? SelectedCoin
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
		public ObservableCollection<string> StringCategories { get; set; }
		public ObservableCollection<CoinUIModel> Coins { get; set; }
		public ObservableCollection<string> StringCoins { get; set; }
		public ObservableCollection<KindUIModel> Kinds { get; set; }


		//Для ограниченного выбора при фильтрации в комбобоксе отдельного окна
		public ObservableCollection<KindUIModel> TempKinds { get; set; }

		public ObservableCollection<StatusUIModel> Statuses { get; set; }
		public ObservableCollection<string> StringStatuses { get; set; }


		public LambdaCommand UndoSelectedPosition { get; set; }
		public LambdaCommand UpdatePosition { get; set; }
		public LambdaCommand AddComplexPosition { get; set; }
		public LambdaCommand SelectNewPosition { get; set; }
		public LambdaCommand Dp_OpenDateChange { get; set; }
		public LambdaCommand Cb_KindTextChange { get; set; }
		public LambdaCommand cb_CoinSelectionChanged { get; set; }
		public LambdaCommand cb_StatusSelectionChanged { get; set; }
		public LambdaCommand Cb_CategorySelectionChanged { get; set; }
		public LambdaCommand Cb_KindSelected { get; set; }



		public EditContinuePositionWindowsModel()
		{


			_context = new Context();

			LockEvent += SetBlock;

			//Иницмализируется в классе *xaml.cs
			//Создание выбранной позиции
			SelectedPosition = new PositionUIModel();
			OriginalSelectedPosition = new PositionUIModel();

			SelectedPositions = new ObservableCollection<PositionUIModel>();
			MAINPOSITIONSCOLLECTION = new ObservableCollection<PositionUIModel>(); //Ссылки на все позиции из главного окна

			//Инициализация цвета
			TB_KindColor = "White";

			Categories = new ObservableCollection<CategoryUIModel>(); //Изначально подгруженные категории

			StringCategories = new ObservableCollection<string>();

			Kinds = new ObservableCollection<KindUIModel>();  //Изначально подгруженные виды

			TempKinds = new ObservableCollection<KindUIModel>(); // Не подгружаются

			Coins = new ObservableCollection<CoinUIModel>(); //Изначально подгруженные монеты

			StringCoins = new ObservableCollection<string>();

			Statuses = new ObservableCollection<StatusUIModel>(); //Изначально подгруженные статусы

			StringStatuses = new ObservableCollection<string>(); //Подгружаются из Statuses



			//Команды
			UndoSelectedPosition = new LambdaCommand(execute =>
			{
				//Console.WriteLine($"Оригинальная позиция {OriginalSelectedPosition.Kind.Kind}");
				ValuePositionCopy(SelectedPosition, OriginalSelectedPosition);
				//Безопасный откат выбранных позиций в комбобокс
				
				SelectedOpenDate = SelectedPosition.OpenDate;
				SelectedCloseDate = SelectedPosition.CloseDate;
				SelectedCategory = SelectedPosition.Kind.Category.Category;
				SelectedCoin = SelectedPosition.Coin.ShortName;
				SelectedStatus = SelectedPosition.Status.StatusName;


				//Console.WriteLine($"Оригинальная позиция {OriginalSelectedPosition.Kind.Kind}");
				SelectedPositionsInicailization(SelectedPositions);
				UpdateEvent();
			},
			canExecute => SelectedPosition != null && OriginalSelectedPosition != null
			);

			Dp_OpenDateChange = new LambdaCommand(execute =>
			{
				DateTime temp = TempSelectedOpenDate.Value + DateTime.Now.TimeOfDay;
				SelectedOpenDate = temp.ToString();
				if (SelectedStatus == "Закрыта" || SelectedStatus == "Closed")
				{
					SelectedCloseDate = SelectedOpenDate;
					//Console.WriteLine($"Мы в закрытой позиции дата: {SelectedCloseDate}");
				}
				else if (SelectedStatus == "Открыта" || SelectedStatus == "Open")
				{
					SelectedCloseDate = "";
					//Console.WriteLine($"Мы в открытой позиции дата: {SelectedCloseDate}") ;
				}
				SelectedPosition.OpenDate = SelectedOpenDate;
				SelectedPosition.CloseDate = SelectedCloseDate;
				TB_DateColor = "Yellow";
			},
			canExecute =>TempSelectedOpenDate != null
			);


			Cb_CategorySelectionChanged = new LambdaCommand(execute =>
			{
				TB_CategoryColor = "Yellow";
				//Console.WriteLine($"Обновляю cb_category ");
				TempKindInicializationOfCategory();
				SelectedKindIndex = 0;
			},
			canExecute => !block
			);

			Cb_KindTextChange = new LambdaCommand(execute =>
			{
				LockEvent();
				//Console.WriteLine($"Обновляю cb_kind {TextKind}");
				if (TextKind != null && TextKind != "")
				{
					if (!TempKinds.Any(k => k.Kind == TextKind))
					{
						TempKindInicializationTextInput();
					}
				}
				else
				{
					TempKindInicializationTextInput();
				}

				if (Kinds.Any(k => k.Kind == TextKind))
				{
					SelectedCategory = (Kinds.First(k => k.Kind == TextKind)).Category.Category;
				}
				block = false;
				TB_KindColor = "Yellow";
			},
			canExecute => true
			);

			Cb_KindSelected = new LambdaCommand(execute =>
			TB_KindColor = "Yellow",
			canExecute => true
			);

			cb_CoinSelectionChanged = new LambdaCommand(execute =>
			{
				ValueCoinCopy(SelectedPosition.Coin, Coins, SelectedCoin);
				TB_CoinColor = "Yellow";

			},		
			canExecute => true
			);

			//сброс отображения времени если статус изменился
			cb_StatusSelectionChanged = new LambdaCommand(execute =>
			{
				ValueStatusCopy(SelectedPosition.Status, Statuses, SelectedStatus);
				TB_StatusColor = "Yellow";

				if (SelectedStatus == "Закрыта" || SelectedStatus == "Closed")
				{
					SelectedCloseDate = SelectedOpenDate;
				}
				else if (SelectedStatus == "Открыта" || SelectedStatus == "Open")
				{
					SelectedCloseDate = "";
				}
				TB_DateColor = "Yellow";
			},
			canExecute => true
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
					var editPosition = (PositionUIModel)SelectedPosition.Clone();
					ValuePositionCopy(SelectedPosition, OriginalSelectedPosition);
					complexPositionsHendler.AddComplexPosition(SelectedPositions, editPosition, _context);
					//Console.WriteLine($"Оригинальная позиция {OriginalSelectedPosition.Kind.Kind}");
					UpdateEvent();
					SelectedPositionsInicailization(SelectedPositions);
















					//Добавление новой комплексной позиции
				},
				canExecute => SelectedPosition.Kind is not null && SelectedPosition.Income != null && SelectedPosition.Expense != null && SelectedPosition.Status.StatusName == "Открыта"
			);

			//Активация через костыль в кодбехайнд
			SelectNewPosition = new LambdaCommand(

				async execute =>
					{
						LockEvent();
						ValuePositionCopy(SelectedPosition, DataGridSelectedItem);

						//Безопасный откат выбранных позиций в комбобокс
						SelectedOpenDate = SelectedPosition.OpenDate;
						SelectedCloseDate = SelectedPosition.CloseDate;
						SelectedCategory = SelectedPosition.Kind.Category.Category;
						SelectedCoin = SelectedPosition.Coin.ShortName;
						SelectedStatus = SelectedPosition.Status.StatusName;

						TempKindInicialization();
						UpdateEvent();
						block = false;
					},
				canExecute => SelectedPosition.Kind != null
				);
		}


		//Методы класса
		public void SetStringStatusesAndTranslation(ObservableCollection<StatusUIModel> _input, ObservableCollection<string> _output)
		{
			_output.Clear();
			foreach (var stat in _input)
			{
				if (stat.StatusName == "Open")
				{
					stat.StatusName = "Открыта";
					_output.Add(stat.StatusName);
				}
				else if (stat.StatusName == "Closed")
				{
					stat.StatusName = "Закрыта";
					_output.Add(stat.StatusName);
				}
				else if (stat.StatusName == "Deleted")
				{
					continue;
				}
				else
				{
					_output.Add(stat.StatusName);
				}
			}
		}


		//Инициализация временных категорий для добавления новой позиции
		public void SetStringCaregories(ObservableCollection<string> _output, ObservableCollection<CategoryUIModel> _input)
		{
			foreach (var stat in _input)
			{
				_output.Add(stat.Category);
			}
		}

		public void SetStringKinds(ObservableCollection<string> _output, ObservableCollection<KindUIModel> _input)
		{
			foreach (var stat in _input)
			{
				_output.Add(stat.Kind);
			}
		}

		public void SetStringCoins(ObservableCollection<string> _output, ObservableCollection<CoinUIModel> _input)
		{
			foreach (var stat in _input)
			{
				_output.Add(stat.ShortName);
			}
		}

		////Инициализация категорий
		//public ObservableCollection<CategoryUIModel> CategoriesInicialization()
		//{
		//	BLL.Context.ListOfCategories tempCat = new BLL.Context.ListOfCategories();
		//	List<CategoryUIModel> tempCategoriesAsync = tempCat.GetAllAsync().Result.Select(cat => Mappers.UIMapper.MapCategoryBLLToCategoryUI(cat)).ToList();
		//	return new ObservableCollection<CategoryUIModel>(tempCategoriesAsync);
		//}

		////Инициализация монет
		//public ObservableCollection<CoinUIModel> CoinsInicialization()
		//{
		//	BLL.Context.ListOfCoins tempCoin = new BLL.Context.ListOfCoins();
		//	List<CoinUIModel> tempCoins = tempCoin.GetAllAsync().Result.Select(coin => Mappers.UIMapper.MapCoinBLLToCoinUI(coin)).ToList();
		//	return new ObservableCollection<CoinUIModel>(tempCoins);
		//}

		////Инициализация статусов отдельный метод не написан
		//BLL.Context.ListOfStatuses tempStat = new BLL.Context.ListOfStatuses();
		//List<StatusUIModel> tempStatuses = tempStat.GetAllAsync().Result.Select(stat => Mappers.UIMapper.MapStatusBLLToStatusUI(stat)).ToList();

		////Инициализация видов
		//public ObservableCollection<KindUIModel> KindsInicialization()
		//{
		//	BLL.Context.ListOfKinds tempKind = new BLL.Context.ListOfKinds();
		//	List<KindUIModel> _tempKinds = tempKind.GetAllAsync().Result.Select(kind => Mappers.UIMapper.MapKindBLLToKindUI(kind)).ToList();
		//	return new ObservableCollection<KindUIModel>(_tempKinds);
		//}

		//Инициализация выбранных позиций
		public void SelectedPositionsInicailization(ObservableCollection<PositionUIModel> _selectedPositions, int _flagSource = 0)
		{
			try
			{
				PositionUIModel sp = new PositionUIModel();
				if (_flagSource == 0)
				{
					sp = (PositionUIModel)SelectedPosition.Clone();
				}
				else
				{
					sp = (PositionUIModel)OriginalSelectedPosition.Clone();
				}

				_selectedPositions.Clear();
				//Если позиция простая или выбран родитель
				if (sp.ZeroParrentKey == null)
				{
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
			catch (Exception ex)
			{

				Console.WriteLine($"{ex.Message} ZeroParrentKey: {_selectedPosition.ZeroParrentKey} ParrentKey: {_selectedPosition.ParrentKey}");
			}
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
				SelectedPositionsInicailization(SelectedPositions, 0);
				Console.WriteLine($"Список временных позиций перестроен");
			}
		}

		//Инициализация временных видов по выбранной категории 
		public void TempKindInicializationOfCategory()
		{
			try
			{
				if (TempKinds != null && SelectedPosition.Kind != null)
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
								TempKinds.Add((KindUIModel)item.Clone());
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

		//При обновлении текста
		public void TempKindInicializationTextInput()
		{
			try
			{
				//  && SelectedPosition.Kind != null НЕ ПРОВЕРЯЛОСЬ!
				if (TempKinds != null && SelectedPosition.Kind != null)
				{
					//Создаю позицию с оригинальным адресом
					var OrigAddressPosKind = SelectedPosition.Kind;
					var cloneKind = (KindUIModel)SelectedPosition.Kind.Clone();
					SelectedPosition.Kind = cloneKind;
					TempKinds.Clear();
					SelectedPosition.Kind = OrigAddressPosKind;

					//Работало, но не совсем корректно
					////var cloneKind = (KindUIModel)SelectedPosition.Kind.Clone();
					//TempKinds.Clear();
					////SelectedPosition.Kind = cloneKind;

					foreach (var item in Kinds)
					{
						TempKinds.Add((KindUIModel)item.Clone());
					}
				}
				else if (TempKinds != null)
				{
					TempKinds.Clear();
					foreach (var item in Kinds)
					{
						TempKinds.Add((KindUIModel)item.Clone());
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка при обновлении текста: {ex.Message}");
			}
		}

		void ValuePositionCopy(PositionUIModel _tocopy, PositionUIModel _fromCopy)
		{
			try
			{
				if (_tocopy != null && _fromCopy != null && _tocopy.Kind != null && _tocopy.Kind.Category != null)
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
				else
				{
					Console.WriteLine("Сработала передача вида по новой ссылке (не исключение)");
					SelectedPosition.Kind = (KindUIModel)OriginalSelectedPosition.Kind.Clone();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{ex.Message} _tocopy {_tocopy.Kind.Kind} _fromcopy {_fromCopy.Kind.Kind}");
			}
		}

		void ValueKindCopy(KindUIModel _tocopy, KindUIModel _fromCopy)
		{
			_tocopy.Id = _fromCopy.Id;
			_tocopy.Kind = _fromCopy.Kind;
			_tocopy.Category.Id = _fromCopy.Category.Id;
			_tocopy.Category.Category = _fromCopy.Category.Category;
			_tocopy.Category.RefNumber = _fromCopy.Category.RefNumber;
		}

		void ValueCoinCopy(CoinUIModel _tocopy, CoinUIModel _fromCopy)
		{
			_tocopy.Id = _fromCopy.Id;
			_tocopy.FullName = _fromCopy.FullName;
			_tocopy.ShortName = _fromCopy.ShortName;
			_tocopy.RefNumber = _fromCopy.RefNumber;
			_tocopy.CoinNotes = _fromCopy.CoinNotes;
		}

		void ValueCoinCopy(CoinUIModel _tocopy, ObservableCollection<CoinUIModel> _fromCollection, string _coinShortName)
		{
			if (_tocopy !=null && _fromCollection != null)
			{
				var _fromCopy = Coins.First(c => c.ShortName == SelectedCoin);
				_tocopy.Id = _fromCopy.Id;
				_tocopy.FullName = _fromCopy.FullName;
				_tocopy.ShortName = _fromCopy.ShortName;
				_tocopy.RefNumber = _fromCopy.RefNumber;
				_tocopy.CoinNotes = _fromCopy.CoinNotes;
			}
			else
			{
                Console.WriteLine("Некорректные аргументы метода ValueCoinCopy");
            }
		}

		void ValueStatusCopy(StatusUIModel _tocopy, ObservableCollection<StatusUIModel> _fromCollection, string _statusName)
		{
			if (_tocopy != null && _fromCollection != null)
			{
				var _fromCopy = Statuses.First(s => s.StatusName == SelectedStatus);
				_tocopy.Id = _fromCopy.Id;
				_tocopy.StatusName = _fromCopy.StatusName;
				_tocopy.StatusNotes = _fromCopy.StatusNotes;
			}
			else
			{
				Console.WriteLine("Некорректные аргументы метода ValueStatusCopy");
			}
		}



		void SoftEraseKind(KindUIModel _toerase)
		{
			_toerase.Id = 0;
			_toerase.Kind = null;
			_toerase.Category.Id = 0;
			_toerase.Category.Category = null;
			_toerase.Category.RefNumber = 0;
		}

		void SetBlock()
		{
			block = true;
		}

	}

}
