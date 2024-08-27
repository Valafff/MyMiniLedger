using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyMiniLedger.WPF.Models
{
	public class PairDealModel : BaseNotify, ICloneable, IComparable<PairDealModel>
	{
		private int _dealNumber;
		public int DealNumber
		{
			get => _dealNumber;
			set => SetField(ref _dealNumber, value);
		}

		private string _buyItem;
		public string BuyItem
		{
			get => _buyItem;
			set => SetField(ref _buyItem, value);
		}

		private string _sellItem;
		public string SellItem
		{
			get => _sellItem;
			set => SetField(ref _sellItem, value);
		}

		private DateTime _dealOpenTime;
		public DateTime DealOpenTime
		{
			get => _dealOpenTime;
			set => SetField(ref _dealOpenTime, value);
		}

		private string _dealName;
		public string DealName
		{
			get => _dealName;
			set => SetField(ref _dealName, value);
		}

		private string _dealNotes;
		public string DealNotes
		{
			get => _dealNotes;
			set => SetField(ref _dealNotes, value);
		}

		//Расчетные значения
		private double _totalBuyAmount;
		[JsonIgnore]
		public double TotalBuyAmount
		{
			get => _totalBuyAmount;
			set => SetField(ref _totalBuyAmount, value);
		}

		private double _totalSellAmount;
		[JsonIgnore]
		public double TotalSellAmount
		{
			get => _totalSellAmount;
			set => SetField(ref _totalSellAmount, value);
		}

		private double _sellToBuyCourse;
		[JsonIgnore]
		public double SellToBuyCourse
		{
			get => _sellToBuyCourse;
			set => SetField(ref _sellToBuyCourse, value);
		}

		private double _buyToSellCourse;
		[JsonIgnore]
		public double BuyToSellCourse
		{
			get => _buyToSellCourse;
			set => SetField(ref _buyToSellCourse, value);
		}

		private string _standartCourse;
		[JsonIgnore]
		public string StandartCourse
		{
			get => _standartCourse;
			set => SetField(ref _standartCourse, value);
		}

		private double _courseNow;
		[JsonIgnore]
		public double CourseNow
		{
			get => _courseNow;
			set => SetField(ref _courseNow, value);
		}

		private string _percentDifference;
		[JsonIgnore]
		public string PercentDifference
		{
			get => _percentDifference;
			set => SetField(ref _percentDifference, value);
		}

		private string _valueDifference;
		[JsonIgnore]
		public string ValueDifference
		{
			get => _valueDifference;
			set => SetField(ref _valueDifference, value);
		}

		private string _totalValueProfit;
		[JsonIgnore]
		public string TotalValueProfit
		{
			get => _totalValueProfit;
			set => SetField(ref _totalValueProfit, value);
		}


		//Служебные значения
		public int? ParentZeroKey {get; set; }
		public bool? isOpen { get; set; } = true;
		[JsonIgnore]
		public bool? strongCoin {  get; set; } = false;
		[JsonIgnore]
		public bool? invertedCourse { get; set; } = false;

		//Резерв
		public string? AdditionalData { get; set; }



		public object Clone() => MemberwiseClone();

		public int CompareTo(PairDealModel? other)
		{
			return this.DealNumber.CompareTo(other.DealNumber);
		}
	}
}
