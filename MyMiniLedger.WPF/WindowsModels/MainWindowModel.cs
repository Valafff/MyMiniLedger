using MyMiniLedger.BLL;
using MyMiniLedger.BLL.Context;
using MyMiniLedger.BLL.Models;
using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.Windows.CategoryWindow;
using MyMiniLedger.WPF.Windows.CoinWindow;
using MyMiniLedger.WPF.Windows.KindWindow;
using MyMiniLedger.WPF.Windows.NewPositionWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyMiniLedger.WPF.WindowsModels
{
	public class MainWindowModel : BaseNotify
	{
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

		private PositionUIModel? _positionConstruct;
		public PositionUIModel? PositionConstruct
		{
			get => _positionConstruct;
			set => SetField(ref _positionConstruct, value);
		}


		public ObservableCollection<PositionUIModel> Positions { get; set; }
		public ObservableCollection<CategoryUIModel> Categories { get; set; }
		public ObservableCollection<CoinUIModel> Coins { get; set; }
		public ObservableCollection<KindUIModel> Kinds { get; set; }
		public ObservableCollection<StatusUIModel> StatusesForService { get; set; }
		public ObservableCollection<StatusUIModel> StatusesForUser { get; set; }

		public LambdaCommand OpenCategoryWindow { get; set; }
		public LambdaCommand OpenKindWindow { get; set; }
		public LambdaCommand OpenCoinWindow { get; set; }
		public LambdaCommand OpenNewPositionWindow { get; set; }
		public LambdaCommand InsertNewPosition { get; set; }


		public LambdaCommand RefreshPositions { get; set; }


		//Для привязки поле должно быть пропой {get; set;}
		//public List<string> searchTypes { get; set; } = new List<string> { "Номер позиции", "Категория", "Вид", "Валюта", "Тег", "Статус" };
		public string[] searchTypes { get; set; } = new string[] { "Номер позиции", "Категория", "Вид", "Валюта", "Тег", "Статус" };


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

			Positions = new ObservableCollection<PositionUIModel>(tempPositionsAsync);

			//Инициализация категорий
			BLL.Context.ListOfCategories tempCat = new BLL.Context.ListOfCategories();
			List<CategoryUIModel> tempCategoriesAsync = tempCat.GetAllAsync().Result.Select(cat => Mappers.UIMapper.MapCategoryBLLToCategoryUI(cat)).ToList();
			Categories = new ObservableCollection<CategoryUIModel>(tempCategoriesAsync);

			//Инициализация монет
			BLL.Context.ListOfCoins tempCoin = new BLL.Context.ListOfCoins();
			List<CoinUIModel> tempCoins = tempCoin.GetAllAsync().Result.Select(coin => Mappers.UIMapper.MapCoinBLLToCoinUI(coin)).ToList();
			Coins = new ObservableCollection<CoinUIModel>(tempCoins);

			//Инициализация видов
			BLL.Context.ListOfKinds tempKind = new BLL.Context.ListOfKinds();
			List<KindUIModel> tempKinds = tempKind.GetAllAsync().Result.Select(kind => Mappers.UIMapper.MapKindBLLToKindUI(kind)).ToList();
			Kinds = new ObservableCollection<KindUIModel>(tempKinds);

			//Инициализация статусов
			BLL.Context.ListOfStatuses tempStat = new BLL.Context.ListOfStatuses();
			List<StatusUIModel> tempStatuses = tempStat.GetAllAsync().Result.Select(stat => Mappers.UIMapper.MapStatusBLLToStatusUI(stat)).ToList();
			StatusesForService = new ObservableCollection<StatusUIModel>(tempStatuses);

			StatusesForUser = new ObservableCollection<StatusUIModel>();
			setStatusesForUser(StatusesForService);

			PositionConstruct = new PositionUIModel() { Kind = new KindUIModel() { Category = new CategoryUIModel() }, Coin = new CoinUIModel(), Status = new StatusUIModel() };

			OpenCategoryWindow = new LambdaCommand(execute => { new CategoryWindow().Show(); });
			OpenKindWindow = new LambdaCommand(execute => { new KindWindow().Show(); });
			OpenCoinWindow = new LambdaCommand(execute => { new CoinWindow().Show(); });
			//OpenNewPositionWindow = new LambdaCommand(execute => { new NewPositionWindow().Show();  });
			OpenNewPositionWindow = new LambdaCommand(execute =>
			{
				NewPositionWindow np = new NewPositionWindow();
				np.Owner = Application.Current.MainWindow;
				np.Show();
			}
			);

			//Вставка новой позиции
			InsertNewPosition = new LambdaCommand(
				async execute =>
				{
					//Добавление
					if ( PositionConstruct.CloseDate == null ) { PositionConstruct.CloseDate =   DateTime.MinValue.ToString(); }
					//DateTime t = DateTime.ParseExact( PositionConstruct.OpenDate, "MM/dd/yyyy hh:mm:ss tt", null);
					PositionConstruct.OpenDate = "15.04.2024 00:00:00";
					PositionConstruct.CloseDate = "15.04.2024 00:00:00";
					PositionConstruct.PositionKey = 100500;
					await _context.PositionsTableBL.InsertAsync(Mappers.UIMapper.MapPositionUIToPositionBLL(PositionConstruct));
					//Обновление списка UI
					var maxId = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList()).Max(p => p.Id);
					var updatedPos = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList()).Where(p => p.Id == maxId);
					PositionUIModel temp = new PositionUIModel();
					temp = updatedPos.First(t => t.Id == maxId);
					Positions.Add(temp);

					//var updatedPos = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList());
					//Positions.Clear();
					//foreach ( var item in updatedPos )
					//{
					//	Positions.Add(item);
					//}
				}
			//canExecute => SelectedKind is not null &&
			//SelectedKind.Kind != null &&
			//Kinds.Any(k => k.Kind == _selectedKind.Kind) == false
			);

			RefreshPositions = new LambdaCommand(
				execute =>
				{
					//var updatedPos = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList());
					//Positions.Clear();
					//foreach (var item in updatedPos)
					//{
					//	Positions.Add(item);
					//}
					var maxId = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList()).Max(p => p.Id);
					var updatedPos = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList()).Where(p => p.Id == maxId);
					PositionUIModel temp = new PositionUIModel();
					temp = updatedPos.First(t => t.Id == maxId);
					Positions.Add(temp);
				}
				);
		}

		void setStatusesForUser(ObservableCollection<StatusUIModel> _input)
		{
			foreach (var stat in _input)
			{
				if (stat.StatusName == "Open")
				{
					stat.StatusName = "Открыто";
					StatusesForUser.Add(stat);
				}
				else if (stat.StatusName == "Closed")
				{
					stat.StatusName = "Закрыто";
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

		//public void hardUpdate(BLL.Context.ListOfPositions tempPos)
		//{

		//	var maxId = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList()).Max(p => p.Id);
		//	var updatedPos = (tempPos.GetAllAsync().Result.Select(p => Mappers.UIMapper.MapPositionBLLToPositionUI(p)).ToList()).Where(p => p.Id == maxId);
		//	PositionUIModel temp = new PositionUIModel();
		//	temp = updatedPos.First(t => t.Id == maxId);
		//	Positions.Add(temp);
		//}
	}
}
