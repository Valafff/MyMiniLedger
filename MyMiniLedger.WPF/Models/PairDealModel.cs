using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public int? ParentZeroKey {get; set; }
		public bool? isOpen { get; set; } = true;
		//Резерв
		public string? AdditionalData { get; set; }



		public object Clone() => MemberwiseClone();

		public int CompareTo(PairDealModel? other)
		{
			return this.DealNumber.CompareTo(other.DealNumber);
		}
	}
}
