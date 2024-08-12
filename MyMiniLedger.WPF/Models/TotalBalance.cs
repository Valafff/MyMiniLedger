using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.WPF.Models
{
	public class TotalBalance : BaseNotify, ICloneable
	{
		private string _coinName;
		public string CoinName
		{
			get => _coinName;
			set => SetField(ref _coinName, value);
		}

		private string _cointype;
		public string Cointype
		{
			get => _cointype;
			set => SetField(ref _cointype, value);
		}

		private double _totalIncome;
		public double TotalIncome
		{
			get => _totalIncome;
			set => SetField(ref _totalIncome, value);
		}

		private double _totalExpense;
		public double TotalExpense
		{
			get => _totalExpense;
			set => SetField(ref _totalExpense, value);
		}

		private double _balance;
		public double Balance
		{
			get => _balance;
			set => SetField(ref _balance, value);
		}

		private string _averagePrice;
		public string AveragePrice
		{
			get => _averagePrice;
			set => SetField(ref _averagePrice, value);
		}

		private string? _currentCourseToUsd;
		public string? CurrentCourseToUsd
		{
			get => _currentCourseToUsd;
			set => SetField(ref _currentCourseToUsd, value);
		}


		public object Clone() => MemberwiseClone();
	}
}
