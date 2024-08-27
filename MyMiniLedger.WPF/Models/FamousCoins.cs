using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.WPF.Models
{
	//TODO Сделать инициализацию из конфига
	public class FamousCoins
	{
		public List<string> Rating_0 { get; init; }
		public List<string> Rating_1 { get; init; }
		public List<string> Rating_2 { get; init; }
		public List<string> Other { get; init; }

		public FamousCoins()
		{
			Rating_0 = new List<string>() { "USD" };
			Rating_1 = new List<string>() { "USDT", "USDC" };
			Rating_2 = new List<string>() { "BTC" };
			Other = new List<string>() { "RUB" };
		}

		public void CheckCourse(ref PairDealModel _resultDeal, string _keyCoin)
		{
			double sellValue;
			double buyValue;

			sellValue = _resultDeal.TotalSellAmount;
			buyValue = _resultDeal.TotalBuyAmount;

			if (Math.Abs( sellValue / buyValue) >= 1 && _resultDeal.BuyItem != _keyCoin)
			{
				_resultDeal.StandartCourse = string.Format("{0:F8}", Math.Abs(_resultDeal.TotalSellAmount / _resultDeal.TotalBuyAmount));
				_resultDeal.invertedCourse = false;
				_resultDeal.strongCoin = false;
			}
            else if(Math.Abs(sellValue / buyValue) < 1 && _resultDeal.BuyItem == _keyCoin)
            {
				_resultDeal.StandartCourse = string.Format("{0:F8}", Math.Abs(1/(_resultDeal.TotalSellAmount / _resultDeal.TotalBuyAmount)));
				_resultDeal.invertedCourse = true;
				_resultDeal.strongCoin = false;
			}
            else if (Math.Abs(sellValue / buyValue) < 1 && _resultDeal.BuyItem != _keyCoin)
            {
				_resultDeal.StandartCourse = string.Format("{0:F8}", Math.Abs(1 / (_resultDeal.TotalSellAmount / _resultDeal.TotalBuyAmount)));
				_resultDeal.invertedCourse = true;
				_resultDeal.strongCoin = true;
			}
            else /*(Math.Abs(sellValue / buyValue) > 1 && _resultDeal.BuyItem == _keyCoin)*/
            {
				_resultDeal.StandartCourse = string.Format("{0:F8}", Math.Abs(_resultDeal.TotalSellAmount / _resultDeal.TotalBuyAmount));
				_resultDeal.invertedCourse = false;
				_resultDeal.strongCoin = true;
			}
        }
	}
}
