using MyMiniLedger.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.WPF.ViewTools
{
	internal class ComplexPositionBalanceCalculator
	{
		List<TotalBalance> totalBalances;
		public ComplexPositionBalanceCalculator()
		{
			totalBalances = new List<TotalBalance>();
		}

		public List<TotalBalance> GetTotalBalances(ObservableCollection<PositionUIModel> _selectedPositions)
		{
			try
			{
				string[] coins = _selectedPositions.Select(p => p.Coin.ShortName).Distinct().ToArray();

				foreach (var coin in coins)
				{
					TotalBalance temp = new TotalBalance();
					for (int i = 0; i < _selectedPositions.Count; i++)
					{
						if (_selectedPositions[i].Coin.ShortName == coin)
						{
							temp.CoinName = _selectedPositions[i].Coin.ShortName;
							temp.TotalIncome += Double.Parse(_selectedPositions[i].Income);
							temp.TotalExpense += Double.Parse(_selectedPositions[i].Expense);
						}
					}
					temp.Balance = temp.TotalIncome-temp.TotalExpense;
					totalBalances.Add(temp);
				}
				return totalBalances;
			}
			catch (Exception)
			{
                Console.WriteLine("Ошибка GetTotalBalances");
                throw;
			}
			
		}

	}
}
