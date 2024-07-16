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
        const int COINSNUMBERINPOSITION = 2;

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
                            //Требуется усложнить логику с тегами для обработки 3х и более монет
                            temp.AveragePrice = "Курс не определен";
                        }
                    }
                    temp.Balance = temp.TotalIncome - temp.TotalExpense;
                    totalBalances.Add(temp);
                }
                //Если две или одна монеты
                if (coins.Length == COINSNUMBERINPOSITION)
                {
                    totalBalances[0].AveragePrice = (Math.Abs(Math.Round(totalBalances[1].Balance / totalBalances[0].Balance, 8, MidpointRounding.ToEven))).ToString();
                    totalBalances[1].AveragePrice = (Math.Abs(Math.Round( totalBalances[0].Balance / totalBalances[1].Balance, 8, MidpointRounding.ToEven))).ToString();
                }
                //Если монет в позиции 3 и более монет
                else if(coins.Length == COINSNUMBERINPOSITION-1)
                {
                    totalBalances[0].AveragePrice = "1";
                }
                
                return totalBalances;
            }
            catch (Exception)
            {
                Console.WriteLine("Ошибка GetTotalBalances");
                throw;
            }
        }

        //Для одной позиции
        public TotalBalance GetTotalBalnce(ObservableCollection<PositionUIModel> _selectedPositions, PositionUIModel _selectedPosition )
        {
            try
            {
                if (_selectedPosition != null)
                {
                    TotalBalance temp = new TotalBalance() { CoinName = _selectedPosition.Coin.ShortName };
                    foreach (var position in _selectedPositions)
                    {
                        if (position.Coin.ShortName == _selectedPosition.Coin.ShortName)
                        {
                            temp.TotalIncome += Double.Parse(position.Income);
                            temp.TotalExpense += Double.Parse(position.Expense);
                        }
                    }
                    temp.Balance = temp.TotalIncome - temp.TotalExpense;
                    return temp;
                }
                return null;
            }
            catch (Exception)
            {
                Console.WriteLine("Ошибка GetTotalBalnce");
                throw;
            }

        }

    }
}
