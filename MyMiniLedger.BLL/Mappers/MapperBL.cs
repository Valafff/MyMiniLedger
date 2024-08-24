using MyMiniLedger.BLL.Models;
using System.Text.Json;
using static MyMiniLedger.BLL.Models.PositionBLLModel;

namespace MyMiniLedger.BLL.Mappers
{
	public static class MapperBL
	{
		//Категория из DAL в BLL
        public static CategoryBLLModel MapCategoryDALToCategoryBLL(DAL.Models.CategoryModel categoryDAL)
		{
			return new CategoryBLLModel() { Id = categoryDAL.Id, Category = categoryDAL.Category };
		}

		//Категория из BLL в DAL
		public static DAL.Models.CategoryModel MapCategoryBLLToCategoryDAL( CategoryBLLModel categoryBLL)
		{
			return new DAL.Models.CategoryModel() {Id = categoryBLL.Id, Category = categoryBLL.Category };
		}

		//Монеты из DAL в BLL
		public static CoinBLLModel MapCoinDALToCoinBLL(DAL.Models.CoinModel coinDAL)
		{
			return new CoinBLLModel() { Id = coinDAL.Id, ShortName = coinDAL.ShortName, FullName = coinDAL.FullName, CoinNotes = coinDAL.CoinNotes };
		}

		//Монеты из BLL в DAL
		public static DAL.Models.CoinModel MapCoinBLLToCoinDAL(CoinBLLModel coinBLL)
		{
			return new DAL.Models.CoinModel() { Id = coinBLL.Id, ShortName = coinBLL.ShortName, FullName = coinBLL.FullName, CoinNotes = coinBLL.CoinNotes };
		}

		//Статус из DAL в BLL
		public static StatusBLLModel MapStatusDALToStatusBLL(DAL.Models.StatusModel statusDAL)
		{
			return new StatusBLLModel() { Id = statusDAL.Id, StatusName = statusDAL.StatusName, StatusNotes = statusDAL.StatusNotes };
		}

		//Статус из BLL в DAL
		public static DAL.Models.StatusModel MapStatusBLLToStatusDAL(StatusBLLModel statusBLL)
		{
			return new DAL.Models.StatusModel() { Id = statusBLL.Id, StatusName = statusBLL.StatusName, StatusNotes= statusBLL.StatusNotes };
		}

		//Вид из DAL в BLL (аналог Join)
		public static KindBLLModel MapKindDALToKindBLL(DAL.Models.KindModel kindDAL, IEnumerable<DAL.Models.CategoryModel> categoriesDAL)
		{
			// К объекту Category передается значение из набора категорий, где Id категории == CategoryId из объекта kindDAL
			return new KindBLLModel() {Id = kindDAL.Id, Category = MapCategoryDALToCategoryBLL( categoriesDAL.First(cat => cat.Id == kindDAL.CategoryId) ), Kind = kindDAL.Kind};
		}

		//Вид из BLL в DAL
		public static DAL.Models.KindModel MapKindBLLToKindDAL(KindBLLModel kindBLL)
		{
			return new DAL.Models.KindModel() { Id = kindBLL.Id, CategoryId = kindBLL.Category.Id, Kind = kindBLL.Kind};
		}

		// Позиция из DAL в BLL 
		public static PositionBLLModel MapPositionDALToPositionBLL(DAL.Models.PositionModel positionDAL,
			IEnumerable<DAL.Models.KindModel> kindsDAL,
			IEnumerable<DAL.Models.CoinModel> coinsDAL,
			IEnumerable<DAL.Models.StatusModel> statusesDAL,
			// categoriesDAL приклеиваются тк у видов есть зависимая таблица категории
			IEnumerable<DAL.Models.CategoryModel> categoriesDAL)
		{
			if (positionDAL.AdditionalPositionData != null)
			{
				return new PositionBLLModel()
				{
					Id = positionDAL.Id,
					PositionKey = positionDAL.PositionKey,
					OpenDate = positionDAL.OpenDate,
					CloseDate = positionDAL.CloseDate,
					Kind = MapKindDALToKindBLL(kindsDAL.First(kind => kind.Id == positionDAL.KindId), categoriesDAL),
					Income = positionDAL.Income,
					Expense = positionDAL.Expense,
					Saldo = positionDAL.Saldo,
					Coin = MapCoinDALToCoinBLL(coinsDAL.First(coin => coin.Id == positionDAL.CoinId)),
					Status = MapStatusDALToStatusBLL(statusesDAL.First(status => status.Id == positionDAL.StatusId)),
					Tag = positionDAL.Tag,
					Notes = positionDAL.Notes,
					additionalPositionDataBLL = JsonSerializer.Deserialize<AdditionalPositionDataClass>(json: positionDAL.AdditionalPositionData)
				};
			}
			else
			{
				return new PositionBLLModel()
				{
					Id = positionDAL.Id,
					PositionKey = positionDAL.PositionKey,
					OpenDate = positionDAL.OpenDate,
					CloseDate = positionDAL.CloseDate,
					Kind = MapKindDALToKindBLL(kindsDAL.First(kind => kind.Id == positionDAL.KindId), categoriesDAL),
					Income = positionDAL.Income,
					Expense = positionDAL.Expense,
					Saldo = positionDAL.Saldo,
					Coin = MapCoinDALToCoinBLL(coinsDAL.First(coin => coin.Id == positionDAL.CoinId)),
					Status = MapStatusDALToStatusBLL(statusesDAL.First(status => status.Id == positionDAL.StatusId)),
					Tag = positionDAL.Tag,
					Notes = positionDAL.Notes,
					additionalPositionDataBLL = new AdditionalPositionDataClass()
				};
			}
			
		}

		// Позиция из BLL в DAL
		public static DAL.Models.PositionModel MapPositionBLLToPositionDAL(PositionBLLModel positionBLL)
		{
			if (positionBLL.additionalPositionDataBLL.ZeroParrentKey != null & positionBLL.additionalPositionDataBLL.PerrentKey != null)
			{
				return new DAL.Models.PositionModel()
				{
					Id = positionBLL.Id,
					PositionKey = positionBLL.PositionKey,
					OpenDate = positionBLL.OpenDate,
					CloseDate = positionBLL.CloseDate,
					KindId = positionBLL.Kind.Id,
					Income = positionBLL.Income,
					Expense = positionBLL.Expense,
					Saldo = positionBLL.Saldo,
					CoinId = positionBLL.Coin.Id,
					StatusId = positionBLL.Status.Id,
					Tag = positionBLL.Tag,
					Notes = positionBLL.Notes,
					AdditionalPositionData = JsonSerializer.Serialize(positionBLL.additionalPositionDataBLL)
				};
			}
			else
			{
				return new DAL.Models.PositionModel()
				{
					Id = positionBLL.Id,
					PositionKey = positionBLL.PositionKey,
					OpenDate = positionBLL.OpenDate,
					CloseDate = positionBLL.CloseDate,
					KindId = positionBLL.Kind.Id,
					Income = positionBLL.Income,
					Expense = positionBLL.Expense,
					Saldo = positionBLL.Saldo,
					CoinId = positionBLL.Coin.Id,
					StatusId = positionBLL.Status.Id,
					Tag = positionBLL.Tag,
					Notes = positionBLL.Notes,
					AdditionalPositionData = null
				};
			}
			
		}
	}

}
