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
		public double dopusk {  get; init; }	 

		public FamousCoins()
		{
			Rating_0 = new List<string>() { "USD" };
			Rating_1 = new List<string>() { "USDT", "USDC" };
			Rating_2 = new List<string>() { "BTC" };
			Other = new List<string>() { "RUB" };
			dopusk = 0.05;
		}

		public void CheckCourse(ref PairDealModel _resultDeal)
		{
			//if (Math.Round(Math.Abs(_resultDeal.SellToBuyCourse * _resultDeal.TotalBuyAmount), 6) >= (Math.Round((Math.Abs(_resultDeal.TotalSellAmount)), 6) * (1 - dopusk)) && Math.Round(Math.Abs(_resultDeal.SellToBuyCourse * _resultDeal.TotalBuyAmount), 6) <= (Math.Round((Math.Abs(_resultDeal.TotalSellAmount)), 6) * (1 + dopusk)))
			//{
			//	_resultDeal.StandartCourse = string.Format("{0:F8}", Math.Abs(_resultDeal.TotalSellAmount / _resultDeal.TotalBuyAmount));
			//}
			//else
			//	_resultDeal.StandartCourse = string.Format("{0:F8}", Math.Abs(1 / (_resultDeal.TotalSellAmount / _resultDeal.TotalBuyAmount)));
			//_resultDeal._invertedCourse = true;

			if (Math.Round(Math.Abs(_resultDeal.SellToBuyCourse * _resultDeal.TotalBuyAmount), 6) == Math.Round((Math.Abs(_resultDeal.TotalSellAmount)), 6))
			{
				_resultDeal.StandartCourse = string.Format("{0:F8}", Math.Abs(_resultDeal.TotalSellAmount / _resultDeal.TotalBuyAmount));
			}
			else
				_resultDeal.StandartCourse = string.Format("{0:F8}", Math.Abs(1 / (_resultDeal.TotalSellAmount / _resultDeal.TotalBuyAmount)));
			_resultDeal._invertedCourse = true;
		}
	}
}
