
namespace MyMiniLedger.WPF.Models
{
	// BaseNotify реализует интерфейс INotifyPropertyChanged
	public class PositionUIModel: BaseNotify
    {
		private int _id;
		public int Id 
		{
			get => _id;
			set => SetField(ref _id, value);
		}

		private int _positionKey;
		public int PositionKey
		{ 
			get => _positionKey;
			set => SetField(ref _positionKey, value);
		}

		private DateTime _openDate;
		public DateTime OpenDate 
		{
			get => _openDate;
			set => SetField(ref _openDate, value);
		}

		private DateTime _closeDate;
		public DateTime CloseDate
		{
			get => _closeDate;
			set => SetField(ref _closeDate, value);
		}

		private KindUIModel _kind;
		public KindUIModel Kind
		{
			get => _kind;
			set => SetField( ref _kind, value);
		}

		private decimal _income;
		public decimal Income
		{
			get => _income;
			set => SetField(ref _income, value);
		}

		private decimal _expense;
		public decimal Expense
		{
			get =>_expense;
			set => SetField(ref _expense, value);
		}

		private decimal _saldo;
		public decimal Saldo
		{
			get => _saldo;
			set => SetField(ref _saldo, value);
		}

		private CoinUIModel _coin;
		public CoinUIModel Coin
		{
			get => _coin;
			set => SetField(ref _coin, value);
		}

		private StatusUIModel _status;
		public StatusUIModel Status
		{
			get => _status;
			set => SetField(ref _status, value);
		}

		private string? _tag;
		public string? Tag
		{
			get => _tag;
			set => SetField(ref _tag, value);
		}

		private string? _notes;
		public string? Notes
		{
			get => _notes;
			set => SetField(ref _notes, value);
		}
	}
}
