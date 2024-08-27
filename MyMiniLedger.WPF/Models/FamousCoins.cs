using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.WPF.Models
{
	//TODO Сделать инициализацию из конфига
	public class FamousCoins
	{
		public List<string> Rating_0 { get; init; }
		public List<string> Rating_1_stable { get; init; }
		public List<string> Rating_2 { get; init; }
		public List<string> Other { get; init; }
		//public List<string> Exceptions { get; init; }

		public FamousCoins()
		{
			Rating_0 = new List<string>();
			Rating_1_stable = new List<string>();
			Rating_2 = new List<string>();
			Other = new List<string>();

			////Rating_0 - основновная валюта или монеты, которые должны определятся перед stablecoinами xxx/usdt
			//Rating_0 = new List<string>() { "KAS", "USD" };
			////Стэйблкоины
			//Rating_1_stable = new List<string>() { "USDT", "USDC" };
			////Монеты со стандартным курсом usd(t)/xxx
			//Rating_2 = new List<string>() { "BTC" };
			////Прочие монеты, валюты
			//Other = new List<string>() { "RUB" };
			////Exceptions = new List<string>() { "KAS" };
		}

		public void CheckCourse(ref PairDealModel _resultDeal, string _keyCoin)
		{
			//Стандартный курс приводим к баксу, стэйблкоину или монете в Rating_0
			var deal = _resultDeal;
			if (_resultDeal.BuyItem == _keyCoin)
			{
				double ratio = Math.Abs(_resultDeal.TotalBuyAmount / _resultDeal.TotalSellAmount);

				if (ratio < 1 || Rating_1_stable.Contains(_resultDeal.SellItem) /*&& !Exceptions.Contains(_resultDeal.SellItem)*/)
				{
					ratio = 1 / ratio;
					_resultDeal.StandartCourse = string.Format("{0:F8}", ratio);
					_resultDeal.invertedCourse = false;
					_resultDeal.strongCoin = false;
				}
				else
				{
					_resultDeal.StandartCourse = string.Format("{0:F8}", ratio);
					_resultDeal.invertedCourse = true;
					_resultDeal.strongCoin = true;
				}

			}
			else
			{
				double ratio = Math.Abs(_resultDeal.TotalSellAmount / _resultDeal.TotalBuyAmount);
				if (ratio < 1 || Rating_1_stable.Contains(_resultDeal.BuyItem) /*&& !Exceptions.Contains(_resultDeal.BuyItem)*/)
				{
					ratio = 1 / ratio;
					_resultDeal.StandartCourse = string.Format("{0:F8}", ratio);
					_resultDeal.invertedCourse = true;
					_resultDeal.strongCoin = true;
				}
				else
				{
					_resultDeal.StandartCourse = string.Format("{0:F8}", ratio);
					_resultDeal.invertedCourse = false;
					_resultDeal.strongCoin = false;
				}

			}
		}

		public void FamousCoinsInicialization(ObservableCollection<CoinUIModel> _coins)
		{
			foreach (CoinUIModel coin in _coins)
			{
				if (coin.CoinNotes.Contains("R_Other"))
				{
					Other.Add(coin.ShortName);
				}
				if (coin.CoinNotes.Contains("R_0"))
				{
					Rating_0.Add(coin.ShortName);
				}
				if (coin.CoinNotes.Contains("R_1"))
				{
					Rating_1_stable.Add(coin.ShortName);
				}
				if (coin.CoinNotes.Contains("R_2"))
				{
					Rating_2.Add(coin.ShortName);
				}
			}
		}
	}
}
