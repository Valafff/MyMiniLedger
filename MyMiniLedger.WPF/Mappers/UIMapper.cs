using MyMiniLedger.WPF.Models;

namespace MyMiniLedger.WPF.Mappers
{
	public static class UIMapper
	{

		public static CategoryUIModel MapCategoryBLLToCategoryUI(BLL.Models.CategoryBLLModel _cat)
		{
			return new CategoryUIModel() { Id = _cat.Id, Category = _cat.Category };
		}

		public static BLL.Models.CategoryBLLModel MapCategoryUIToCategoryBLL(CategoryUIModel _cat)
		{
			return new BLL.Models.CategoryBLLModel() { Id = _cat.Id, Category = _cat.Category };
		}


		public static CoinUIModel MapCoinBLLToCoinUI(BLL.Models.CoinBLLModel _coin)
		{
			return new CoinUIModel() { Id = _coin.Id, FullName = _coin.FullName, ShortName = _coin.ShortName, CoinNotes = _coin.CoinNotes };
		}

		public static BLL.Models.CoinBLLModel MapCoinUIToCoinBLL(CoinUIModel _coin)
		{
			return new BLL.Models.CoinBLLModel() { Id = _coin.Id, FullName = _coin.FullName, ShortName = _coin.ShortName, CoinNotes = _coin.CoinNotes };
		}


		public static StatusUIModel MapStatusBLLToStatusUI(BLL.Models.StatusBLLModel _status)
		{
			return new StatusUIModel() { Id = _status.Id, StatusName = _status.StatusName, StatusNotes = _status.StatusNotes };
		}

		public static BLL.Models.StatusBLLModel MapStatusUIToStatusBLL(StatusUIModel _status)
		{
			return new BLL.Models.StatusBLLModel() { Id = _status.Id, StatusName = _status.StatusName, StatusNotes = _status.StatusNotes };
		}


		public static KindUIModel MapKindBLLToKindUI(BLL.Models.KindBLLModel _kind)
		{
			return new KindUIModel() { Id = _kind.Id, Category = MapCategoryBLLToCategoryUI(_kind.Category), Kind = _kind.Kind };
		}

		public static BLL.Models.KindBLLModel MapKindUIToKindBLL(KindUIModel _kind)
		{
			return new BLL.Models.KindBLLModel()
			{
				Id = _kind.Id,
				Category = MapCategoryUIToCategoryBLL(_kind.Category),
				Kind = _kind.Kind
			};
		}

		//Перевод даты в string, перевод income expense saldo в string
		public static PositionUIModel MapPositionBLLToPositionUI(BLL.Models.PositionBLLModel _pos)
		{
			return new PositionUIModel()
			{
				Id = _pos.Id,
				PositionKey = _pos.PositionKey,
				OpenDate = _pos.OpenDate.ToString(),
				CloseDate = _pos.CloseDate.ToString(),
				Kind = MapKindBLLToKindUI(_pos.Kind),
				Income = _pos.Income.ToString(),
				Expense = _pos.Expense.ToString(),
				Saldo = _pos.Saldo.ToString(),
				Coin = MapCoinBLLToCoinUI(_pos.Coin),
				Status = MapStatusBLLToStatusUI(_pos.Status),
				Tag = _pos.Tag,
				Notes = _pos.Notes,
				ZeroParrentKey = _pos.additionalPositionDataBLL.ZeroParrentKey,
				ParrentKey = _pos.additionalPositionDataBLL.PerrentKey	
			};
		}

		//Перевод даты в Datetime,  перевод income expense saldo в decemal
		public static BLL.Models.PositionBLLModel MapPositionUIToPositionBLL(PositionUIModel _pos)
		{
			return new BLL.Models.PositionBLLModel()
			{

				Id = _pos.Id,
				PositionKey = _pos.PositionKey,
                OpenDate = _pos.OpenDate,
                CloseDate = _pos.CloseDate,
                Kind = MapKindUIToKindBLL(_pos.Kind),
				Income = Convert.ToDecimal(_pos.Income),
				Expense = Convert.ToDecimal(_pos.Expense),
				Saldo = Convert.ToDecimal(_pos.Saldo),
				Coin = MapCoinUIToCoinBLL(_pos.Coin),
				Status = MapStatusUIToStatusBLL(_pos.Status),
				Tag = _pos.Tag,
				Notes = _pos.Notes,
				additionalPositionDataBLL = new BLL.Models.AdditionalPositionDataClass(_pos.ZeroParrentKey, _pos.ParrentKey)
			};
		}

	}
}
