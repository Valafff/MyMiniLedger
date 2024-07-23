using MyMiniLedger.WPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyMiniLedger.WPF.ViewTools
{
    public static class FormatterPositions
    {
        //Форматирование  по статусу
        public static List<PositionUIModel> ErasePosFromTableByStatus(List<PositionUIModel> _ps, string _status = "Deleted")
        {
            List<PositionUIModel> ResultPositionsList = new List<PositionUIModel>();
            foreach (var item in _ps)
            {
                if (item.Status.StatusName != _status)
                {
                    ResultPositionsList.Add(item);
                }
            }
            return ResultPositionsList;
        }

        //Форматирование статуса
        public static List<PositionUIModel> EditPosFromTableByStatus(List<PositionUIModel> _ps, string targetStatus = "", string newStatus = "")
        {
            List<PositionUIModel> ResultPositionsList = new List<PositionUIModel>();
            foreach (var item in _ps)
            {
                if (item.Status.StatusName == targetStatus)
                {
                    item.Status.StatusName = newStatus;
                }
                ResultPositionsList.Add(item);
            }
            return ResultPositionsList;
        }

        public static PositionUIModel EditPosFromTableByStatus(PositionUIModel _ps, string targetStatus = "", string newStatus = "")
        {
            if (_ps.Status.StatusName == targetStatus)
            {
                _ps.Status.StatusName = newStatus;
            }
            return _ps;
        }

        //Форматирование с вывода количества знаков после запятой
        public static List<PositionUIModel> EditDecemalPosFromTableByMarker(List<PositionUIModel> _ps, string targetMarker = "crypto", string mask = "0.00000000", int n = 2)
        {
            List<PositionUIModel> ResultPositionsList = new List<PositionUIModel>();
            foreach (var item in _ps)
            {
                if (item.Coin.CoinNotes.Contains(targetMarker))
                {
                    var income = Math.Round(Convert.ToDecimal(item.Income), n);
                    var expense = Math.Round(Convert.ToDecimal(item.Expense), n);
                    var saldo = Math.Round(Convert.ToDecimal(item.Saldo), n);
                    Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CurrentUICulture;
                    item.Income = income.ToString(mask);
                    item.Expense = expense.ToString(mask);
                    item.Saldo = saldo.ToString(mask);
                }
                ResultPositionsList.Add(item);
            }
            return ResultPositionsList;
        }

        public static PositionUIModel EditDecemalPosFromTableByMarker(PositionUIModel _ps, string targetMarker = "crypto", string mask = "0.00000000", int n = 2)
        {
            if (_ps.Coin.CoinNotes.Contains(targetMarker))
            {
                //Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                var income = Math.Round(Convert.ToDecimal(_ps.Income), n);
                var expense = Math.Round(Convert.ToDecimal(_ps.Expense), n);
                var saldo = Math.Round(Convert.ToDecimal(_ps.Saldo), n);
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CurrentUICulture;
                _ps.Income = income.ToString(mask);
                _ps.Expense = expense.ToString(mask);
                _ps.Saldo = saldo.ToString(mask);
            }

            return _ps;
        }

        public static string SetCloseDate(string _status = "Close")
        {
            string temp;
            if (_status == "Закрыта" || _status == "Close")
            {
                return temp = DateTime.Now.ToString("G");
            }
            else
            {
                return string.Empty;
            }
        }


    }
}
