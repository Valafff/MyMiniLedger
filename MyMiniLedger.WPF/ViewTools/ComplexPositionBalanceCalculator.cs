using MyMiniLedger.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyMiniLedger.WPF.ViewTools
{
    internal class ComplexPositionBalanceCalculator
    {
        const int CRYPTOSYMBOLSAFTERDELIMETR = 10;
        const int FIATSYMBOLSAFTERDELIMETR = 2;
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
                //BLL.ServicesAPI.Requsts requsts = new BLL.ServicesAPI.Requsts();
                var coins = _selectedPositions.Select(p => new { p.Coin.ShortName, p.Coin.CoinNotes }).Distinct().ToArray();

                foreach (var coin in coins)
                {
                    TotalBalance temp = new TotalBalance();

                    //Блокируется поток на время эксперимента
                    //temp.CurrentCourseToUsd =  requsts.GetCoinCourseToFiatAsync((_selectedPositions.First(c => c.Coin.ShortName == coin)).Coin.FullName, "usd").Result;
                   
                    for (int i = 0; i < _selectedPositions.Count; i++)
                    {
                        if (_selectedPositions[i].Coin.ShortName == coin.ShortName)
                        {
                            temp.CoinName = _selectedPositions[i].Coin.ShortName;
                            temp.TotalIncome += Double.Parse(_selectedPositions[i].Income);
                            temp.TotalExpense += Double.Parse(_selectedPositions[i].Expense);
                            //Требуется усложнить логику с тегами для обработки 3х и более монет
                            temp.AveragePrice = "Отношение не определено";
                        }
                    }
                    if(coin.CoinNotes == "fiat")
                    {
                        temp.TotalIncome = Math.Round(temp.TotalIncome, FIATSYMBOLSAFTERDELIMETR, MidpointRounding.ToEven);
                        temp.TotalExpense = Math.Round(temp.TotalExpense, FIATSYMBOLSAFTERDELIMETR, MidpointRounding.ToEven);
                    }
                    else if (coin.CoinNotes == "crypto")
                    {
                        temp.TotalIncome = Math.Round(temp.TotalIncome, CRYPTOSYMBOLSAFTERDELIMETR, MidpointRounding.ToEven);
                        temp.TotalExpense = Math.Round(temp.TotalExpense, CRYPTOSYMBOLSAFTERDELIMETR, MidpointRounding.ToEven);
                    }

                    temp.Balance = temp.TotalIncome - temp.TotalExpense;
                    totalBalances.Add(temp);
                }

                //Если две или одна монеты
                if (coins.Length == COINSNUMBERINPOSITION)
                {
                    totalBalances[0].AveragePrice = (Math.Abs(Math.Round(totalBalances[1].Balance / totalBalances[0].Balance, CRYPTOSYMBOLSAFTERDELIMETR, MidpointRounding.ToEven))).ToString();
                    totalBalances[1].AveragePrice = (Math.Abs(Math.Round(totalBalances[0].Balance / totalBalances[1].Balance, CRYPTOSYMBOLSAFTERDELIMETR, MidpointRounding.ToEven))).ToString();
                }
                //Если монет в позиции 3 и более монет
                else if (coins.Length == COINSNUMBERINPOSITION - 1)
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
        public TotalBalance GetTotalBalance(ObservableCollection<PositionUIModel> _selectedPositions, PositionUIModel _selectedPosition)
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
