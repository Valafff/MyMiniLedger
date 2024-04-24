using MyMiniLedger.WPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

		//Форматирование по дате - редактирует дату закрытия меньше целевой
		public static List<PositionUIModel> EditPosFromTableByDate(List<PositionUIModel> _ps, DateTime targetDate, string newRecord = "")
		{
			List<PositionUIModel> ResultPositionsList = new List<PositionUIModel>();
			foreach (var item in _ps)
			{
				if (Convert.ToDateTime(item.CloseDate) < targetDate)
				{
					item.CloseDate = newRecord;
				}
				ResultPositionsList.Add(item);
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

		//Форматирование с вывода количества знаков после запятой
		public static List<PositionUIModel> EditDecemalPosFromTableByMarker(List<PositionUIModel> _ps, string targetMarker = "crypto", string mask = "0.00000000")
		{
			List<PositionUIModel> ResultPositionsList = new List<PositionUIModel>();
			foreach (var item in _ps)
			{
				if (item.Coin.CoinNotes.Contains(targetMarker))
				{
					item.Income = Math.Round(Convert.ToDecimal(item.Income), 2).ToString(mask);
					item.Expense = Math.Round(Convert.ToDecimal(item.Expense), 2).ToString(mask);
					item.Saldo = Math.Round(Convert.ToDecimal(item.Saldo), 2).ToString(mask);
				}
				ResultPositionsList.Add(item);
			}
			return ResultPositionsList;
		}


		//Добавление Даты закрытия для позиции
		public static DateTime SetCloseDate(string _status = "Close")
		{
			DateTime temp;
			if (_status == "Закрыта" || _status == "Close")
			{
				return temp = DateTime.Now;
			}
			else 
			{
				return temp = new DateTime(1900,  1, 1, 0, 0, 0, 0);
			}
		}


	}
}
