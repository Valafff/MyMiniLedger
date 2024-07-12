﻿using System;
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

		public object Clone() => MemberwiseClone();
	}
}
