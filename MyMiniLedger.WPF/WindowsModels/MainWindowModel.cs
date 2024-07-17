using Dapper;
using MyMiniLedger.BLL;
using MyMiniLedger.BLL.Context;
using MyMiniLedger.BLL.Models;
using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.Windows.CategoryWindow;
using MyMiniLedger.WPF.Windows.CoinWindow;
using MyMiniLedger.WPF.Windows.EditContinuePosition;
using MyMiniLedger.WPF.Windows.KindWindow;
using MyMiniLedger.WPF.Windows.NewPositionWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyMiniLedger.WPF.WindowsModels
{
	public delegate void UpdateCoinsIndexDelegate();
	public delegate void UpdateCategoriesIndexDelegate();
	public delegate void UpdateKindsIndexDelegate();
	public delegate void UpdateDatePicker();

	public class MainWindowModel : BaseNotify
	{
		//При выполнении метода UpdateCoins() вызывается событие UpdateCoinsIndexEvent, аналогично с другими полями
		public event UpdateCoinsIndexDelegate UpdateCoinsIndexEvent;
		public event UpdateCategoriesIndexDelegate UpdateCategoriesIndexEvent;
		public event UpdateKindsIndexDelegate UpdateKindsIndexEvent;
		public event UpdateDatePicker UpdateDatePickerEvent;

		public InitConfigBLL cf { get; set; }
		private readonly Context _context;
		private string _title = "МиниДомашняяБухгалтерия";
		public string Title
		{
			get => _title;
			set => SetField(ref _title, value);
		}
		private string _createPostitle = "Создание новой позиции";
		public string CreatePostitle
		{
			get => _createPostitle;
			set => SetField(ref _createPostitle, value);
		}

		private string _editContinuePos = "Редактирование/Продолжение позиции";
		public string EditContinuePosition
		{
			get => _editContinuePos;
			set => SetField(ref _editContinuePos, value);
		}

		private PositionUIModel? _positionConstruct;
		public PositionUIModel? PositionConstruct
		{
			get => _positionConstruct;
			set => SetField(ref _positionConstruct, value);
		}

		private PositionUIModel? _selectedPosition;
		public PositionUIModel? SelectedPosition
		{
			get => _selectedPosition;
			set => SetField(ref _selectedPosition, value);
		}

		//Даты для выбора позиции вограниченном диапазоне времени
		private DateTime _startDate;
		public DateTime StartDate
		{
			get => _startDate;
			set => SetField(ref _startDate, value);
		}
		private DateTime _endDate;
		public DateTime EndDate
		{
			get => _endDate;
			set => SetField(ref _endDate, value);
		}


		public ObservableCollection<PositionUIModel> Positions { get; set; }
		public ObservableCollection<CategoryUIModel> Categories { get; set; }
		public ObservableCollection<CoinUIModel> Coins { get; set; }
		public ObservableCollection<KindUIModel> Kinds { get; set; }

		//Для ограниченного выбора при фильтрации в комбобоксе отдельного окна
		public ObservableCollection<KindUIModel> TempKinds { get; set; }
		//Для ограниченного выбора при фильтрации в комбобоксе главного окна
		public ObservableCollection<KindUIModel> TempKindsMain { get; set; }

		public ObservableCollection<StatusUIModel> StatusesForService { get; set; }
		public ObservableCollection<StatusUIModel> StatusesForUser { get; set; }

		//Объявление команд
		public LambdaCommand OpenCategoryWindow { get; set; }
		public LambdaCommand OpenKindWindow { get; set; }
		public LambdaCommand OpenCoinWindow { get; set; }
		public LambdaCommand OpenNewPositionWindow { get; set; }
		public LambdaCommand OpenEditContinueWindow { get; set; }


		public LambdaCommand InsertNewPosition { get; set; }
		public LambdaCommand DeletePosition { get; set; }



		public LambdaCommand RefreshPositions { get; set; }
		public LambdaCommand SearchByDateRange { get; set; }
		public int MaxPosKey;

		//Для привязки поле должно быть пропой {get; set;}
		public string[] searchTypes { get; set; } = new string[] { "Номер позиции", "Категория", "Вид", "Валюта", "Тег", "Статус" };

		public ObservableCollection<string> tempCategories { get; set; } = new ObservableCollection<string>();


		public MainWindowModel()
		{
			//ToDo В файле сделать базовые настройки строк, дат, пользователя 
			cf = new BLL.InitConfigBLL("config.json");

			////Тестовое открытие формы при запуске приложения
			//new CategoryWindow().Show();

			_context = new BLL.Context.Context();
			//Инициализация позиций
			BLL.Context.ListOfPositions tempPos = new BLL.Context.ListOfPositions();

			//.Net 6.0 не поддерживает .ToBlockingEnumerable()
			List<PositionUIModel> tempPositionsAsync = tempPos.GetAllAsync().Result.Select(pos => Mappers.UIMapper.MapPositionBLLToPositionUI(pos)).ToList();

			//Удаление Deleted позиций
			tempPositionsAsync = ViewTools.FormatterPositions.ErasePosFromTableByStatus(tempPositionsAsync, "Deleted");
			//Удаление нулевой даты
			tempPositionsAsync = ViewTools.FormatterPositions.EditPosFromTableByDate(tempPositionsAsync, new DateTime(2000, 01, 01));
			//Переименовывание статусов
			tempPositionsAsync = ViewTools.FormatterPositions.EditPosFromTableByStatus(tempPositionsAsync, "Open", "Открыта");
			tempPositionsAsync = ViewTools.FormatterPositions.EditPosFromTableByStatus(tempPositionsAsync, "Closed", "Закрыта");
			//Отбрасывание 0 после разделителя в зависимости от типа валюты fiat crypto
			tempPositionsAsync = ViewTools.FormatterPositions.EditDecemalPosFromTableByMarker(tempPositionsAsync, "fiat", "0.00");

			tempPositionsAsync.Sort();

			Positions = new ObservableCollection<PositionUIModel>(tempPositionsAsync);
			//Positions = Positions.AsList().Sort();

			//Инициализация категорий
			Categories = CategoriesInicialization();

			//Инициализация монет
			Coins = CoinsInicialization();

			//Инициализация видов
			Kinds = KindsInicialization();
			TempKinds = KindsInicialization();
			TempKindsMain = KindsInicialization();

			setTempCaregories(tempCategories, Categories);

			//Инициализация статусов
			BLL.Context.ListOfStatuses tempStat = new BLL.Context.ListOfStatuses();
			List<StatusUIModel> tempStatuses = tempStat.GetAllAsync().Result.Select(stat => Mappers.UIMapper.MapStatusBLLToStatusUI(stat)).ToList();
			StatusesForService = new ObservableCollection<StatusUIModel>(tempStatuses);

			StatusesForUser = new ObservableCollection<StatusUIModel>();
			setStatusesForUser(StatusesForService);

			PositionConstruct = new PositionUIModel() { Kind = new KindUIModel() { Category = new CategoryUIModel() }, Coin = new CoinUIModel(), Status = new StatusUIModel(), Income = "0", Expense = "0" };

			//PositionConstruct.OpenDate = DateTime.Today.ToString();
			//PositionConstruct.Status.StatusName = StatusesForUser.ElementAt(0).StatusName;

			OpenCategoryWindow = new LambdaCommand(execute => { new CategoryWindow(this).Show(); });
			OpenKindWindow = new LambdaCommand(execute => { new KindWindow(this).Show(); });
			OpenCoinWindow = new LambdaCommand(execute => { new CoinWindow(this).Show(); });
			//Открывается как модальное окно
			OpenNewPositionWindow = new LambdaCommand(execute =>
			{
				NewPositionWindow np = new NewPositionWindow(this);
				np.Owner = Application.Current.MainWindow;
				np.ShowDialog();
			}
			);
			OpenEditContinueWindow = new LambdaCommand(execute =>
			{
				EditContinuePositionWindow ecp = new EditContinuePositionWindow(SelectedPosition, Positions, Categories, Kinds, Coins, StatusesForService);
				ecp.Owner = Application.Current.MainWindow;
				ecp.ShowDialog();
			},
			canExecute => SelectedPosition != null
			);

			//Вставка новой позиции
			InsertNewPosition = new LambdaCommand(
				async execute =>
				{
					//Добавление новой позиции

					PositionConstruct.CloseDate = (ViewTools.FormatterPositions.SetCloseDate(PositionConstruct.Status.StatusName)).ToString();
					//dp_OpenDate_SelectedDateChanged задает время в виде string конвертирую его в datetime и прибавляю настоящее время
					DateTime t = Convert.ToDateTime(PositionConstruct.OpenDate);
					t += DateTime.Now.TimeOfDay;
					PositionConstruct.OpenDate = t.ToString();

					await _context.PositionsTableBL.InsertAsync(Mappers.UIMapper.MapPositionUIToPositionBLL(PositionConstruct));

					//Обновление списка UI
					var maxId = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList()).Max(p => p.Id);
					var updatedPos = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList()).Where(p => p.Id == maxId);
					updatedPos = ViewTools.FormatterPositions.EditPosFromTableByDate(updatedPos.AsList(), new DateTime(2000, 01, 01));
					updatedPos = ViewTools.FormatterPositions.EditPosFromTableByStatus(updatedPos.AsList(), "Open", "Открыта");
					updatedPos = ViewTools.FormatterPositions.EditPosFromTableByStatus(updatedPos.AsList(), "Closed", "Закрыта");
					updatedPos = ViewTools.FormatterPositions.EditDecemalPosFromTableByMarker(updatedPos.AsList(), "fiat", "0.00");
					PositionUIModel temp = new PositionUIModel();
					temp = updatedPos.First(t => t.Id == maxId);
					Positions.Insert(0, temp);
					PositionConstruct.Income = "0";
					PositionConstruct.Expense = "0";
					PositionConstruct.Tag = "";
					PositionConstruct.Notes = "";
					PositionConstruct.OpenDate = DateTime.Today.ToString();
					UpdateDatePickerEvent();
				},
			canExecute => PositionConstruct.Kind is not null && PositionConstruct.Income != null && PositionConstruct.Expense != null
			//&& SelectedKind.Kind != null &&
			//Kinds.Any(k => k.Kind == _selectedKind.Kind) == false
			);

			RefreshPositions = new LambdaCommand(
				execute =>
				{
					UpdatePositionsCollection();
				}
				);

			SearchByDateRange = new LambdaCommand(
				execute => 
				{
					//Console.WriteLine(StartDate);
					//Console.WriteLine(EndDate);
					UpdatePositionsByDateRange();
                }
				);

			//Полное удаление категории
			DeletePosition = new LambdaCommand(async execute =>
			{
                if (Positions.Any(p => p.ZeroParrentKey == SelectedPosition.PositionKey) || SelectedPosition.ParrentKey != null)
                {
                    MessageBox.Show("Удаление комплексной позиции запрещено! Используйте меню \"Редактирование\\Продолжение позиции\"");
                }
                else
				{
					await _context.PositionsTableBL.DeleteAsync(Mappers.UIMapper.MapPositionUIToPositionBLL(_selectedPosition));
					var t = Positions.Where(t => t.Id == _selectedPosition.Id);
					Positions.Remove(t.First());
					SelectedPosition = null;
				}

			},
			canExecute => SelectedPosition is not null);

		}

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

		//Инициализация категорий
		public ObservableCollection<CategoryUIModel> CategoriesInicialization()
		{
			BLL.Context.ListOfCategories tempCat = new BLL.Context.ListOfCategories();
			List<CategoryUIModel> tempCategoriesAsync = tempCat.GetAllAsync().Result.Select(cat => Mappers.UIMapper.MapCategoryBLLToCategoryUI(cat)).ToList();
			return new ObservableCollection<CategoryUIModel>(tempCategoriesAsync);
		}

		//Обновление списка категорий
		public void UpdateCategories()
		{
			Categories.Clear();
			var tc = CategoriesInicialization();
			foreach (var cat in tc) { Categories.Add(cat); }

			tempCategories.Clear();
			setTempCaregories(tempCategories, Categories);
			UpdateCategoriesIndexEvent();
		}

		//Инициализация временных категорий для добавления новой позиции
		public void setTempCaregories(ObservableCollection<string> t, ObservableCollection<CategoryUIModel> _cat)
		{
			foreach (var stat in _cat)
			{
				t.Add(stat.Category);
			}
		}

		//Инициализация видов
		public ObservableCollection<KindUIModel> KindsInicialization()
		{
			BLL.Context.ListOfKinds tempKind = new BLL.Context.ListOfKinds();
			List<KindUIModel> _tempKinds = tempKind.GetAllAsync().Result.Select(kind => Mappers.UIMapper.MapKindBLLToKindUI(kind)).ToList();
			return new ObservableCollection<KindUIModel>(_tempKinds);
		}

		// Обновление видов
		public void UpdateKinds()
		{
			Kinds.Clear();
			TempKinds.Clear();
			TempKindsMain.Clear();

			var tk = KindsInicialization();
			foreach (var k in tk) { Kinds.Add(k); TempKinds.Add(k); TempKindsMain.Add(k); }
			UpdateKindsIndexEvent();
		}

		//Инициализация монет
		public ObservableCollection<CoinUIModel> CoinsInicialization()
		{
			BLL.Context.ListOfCoins tempCoin = new BLL.Context.ListOfCoins();
			List<CoinUIModel> tempCoins = tempCoin.GetAllAsync().Result.Select(coin => Mappers.UIMapper.MapCoinBLLToCoinUI(coin)).ToList();
			return new ObservableCollection<CoinUIModel>(tempCoins);
		}

		//Обновление списка монет
		public void UpdateCoins()
		{
			//var tc = CoinsInicialization();
			//CoinUIModel tCoin = new CoinUIModel();
			//for (int i = 0; i < Coins.Count; i++)
			//{
			//	for (int j = 0; j < tc.Count; j++)
			//	{
			//		if (Coins[i].Id != tc[j].Id)
			//		{
			//			tCoin = tc[j];
			//		}
			//	}
			//}
			//Coins.Add(tCoin);
			Coins.Clear();
			var tc = CoinsInicialization();
			foreach (var coin in tc) { Coins.Add(coin); }
			UpdateCoinsIndexEvent();
		}

		public async void DeleteSelectedPosDataGrid()
		{
			if (SelectedPosition != null)
			{
				if (Positions.Any(p => p.ZeroParrentKey == SelectedPosition.PositionKey) || SelectedPosition.ParrentKey != null)
				{
					MessageBox.Show("Удаление комплексной позиции запрещено! Используйте меню \"Редактирование\\Продолжение позиции\"");
				}
				else
				{
					await _context.PositionsTableBL.DeleteAsync(Mappers.UIMapper.MapPositionUIToPositionBLL(_selectedPosition));
					var t = Positions.Where(t => t.Id == _selectedPosition.Id);
					Positions.Remove(t.First());
					SelectedPosition = null;
				}

				//await _context.PositionsTableBL.DeleteAsync(Mappers.UIMapper.MapPositionUIToPositionBLL(SelectedPosition));
				//var t = Positions.Where(t => t.Id == _selectedPosition.Id);
				//Positions.Remove(t.First());
				//SelectedPosition = null;
			}
		}

		public void UpdatePositionsCollection()
		{
			BLL.Context.ListOfPositions tempPos = new BLL.Context.ListOfPositions();
			List<PositionUIModel> updatedPos = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList());
			//Форматирование
			updatedPos = ViewTools.FormatterPositions.EditPosFromTableByDate(updatedPos.AsList(), new DateTime(2000, 01, 01));
			updatedPos = ViewTools.FormatterPositions.EditPosFromTableByStatus(updatedPos.AsList(), "Open", "Открыта");
			updatedPos = ViewTools.FormatterPositions.EditPosFromTableByStatus(updatedPos.AsList(), "Closed", "Закрыта");
			updatedPos = ViewTools.FormatterPositions.EditDecemalPosFromTableByMarker(updatedPos.AsList(), "fiat", "0.00");

			Positions.Clear();
			foreach (var item in updatedPos)
			{
				int i = 0;
				if (item.Status.StatusName != "Deleted")
				{
					Positions.Insert(i, item);
					i++;
				}
			}
		}

		public void UpdatePositionsByDateRange()
		{
            BLL.Context.ListOfPositions tempPos = new BLL.Context.ListOfPositions();
            List<PositionUIModel> updatedPos = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList());
            //Форматирование
            updatedPos = ViewTools.FormatterPositions.EditPosFromTableByDate(updatedPos.AsList(), new DateTime(2000, 01, 01));
            updatedPos = ViewTools.FormatterPositions.EditPosFromTableByStatus(updatedPos.AsList(), "Open", "Открыта");
            updatedPos = ViewTools.FormatterPositions.EditPosFromTableByStatus(updatedPos.AsList(), "Closed", "Закрыта");
            updatedPos = ViewTools.FormatterPositions.EditDecemalPosFromTableByMarker(updatedPos.AsList(), "fiat", "0.00");

            Positions.Clear();
            foreach (var item in updatedPos)
            {
				DateTime openDate = DateTime.Parse(item.OpenDate).Date;
                Console.WriteLine(openDate);
                int i = 0;
                if (item.Status.StatusName != "Deleted" && openDate >= StartDate && openDate <= EndDate)
                {
                    Positions.Insert(i, item);
                    i++;
                }
            }
        }

		//public void hardUpdate(BLL.Context.ListOfPositions tempPos)
		//{
		//	var maxId = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList()).Max(p => p.Id);
		//	var updatedPos = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList()).Where(p => p.Id == maxId);
		//	PositionUIModel temp = new PositionUIModel();
		//	temp = updatedPos.First(t => t.Id == maxId);
		//	Positions.Add(temp);
		//}

		//Добавлие даты закрытия
		//DateTime TempOpenDate = DateTime.ParseExact(PositionConstruct.OpenDate, "M/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
		////Установка текущего времени
		//TempOpenDate = TempOpenDate + DateTime.Now.TimeOfDay;
		//PositionConstruct.OpenDate = TempOpenDate.ToString();
	}
}
