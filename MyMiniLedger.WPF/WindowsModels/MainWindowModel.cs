using MyMiniLedger.BLL.Context;
using MyMiniLedger.BLL.Models;
using MyMiniLedger.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.WPF.WindowsModels
{
	public class MainWindowModel : BaseNotify
	{
		private readonly Context _context;
		private string _title = "Минибухгалтерия";
        public string Title 
		{
			get => _title;
			set => SetField(ref _title, value);
		}

        public ObservableCollection<PositionUIModel> Positions { get; set; }
		public ObservableCollection<CategoryUIModel> Categories { get; set; }
		public ObservableCollection<CoinUIModel> Coins { get; set; }
		public ObservableCollection<KindUIModel> Kinds { get; set; }
		public ObservableCollection<StatusUIModel> Statuses { get; set; }


		public MainWindowModel()
		{
			//ToDo В файле сделать базовые настройки строк, дат, пользователя 
			var cf = new BLL.InitConfigBLL("config.json");

			_context = new BLL.Context.Context();

			//Инициализация позиций
			BLL.Context.ListOfPositions tempPos = new BLL.Context.ListOfPositions();
			////.Net 6.0 не поддерживает .ToBlockingEnumerable()
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
			Statuses = new ObservableCollection<StatusUIModel>(tempStatuses);
		}

	}
}
