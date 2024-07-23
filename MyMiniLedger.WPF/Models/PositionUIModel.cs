
//using Microsoft.IdentityModel.Tokens;
using MyMiniLedger.WPF.ViewTools;

namespace MyMiniLedger.WPF.Models
{

	// BaseNotify реализует интерфейс INotifyPropertyChanged
	public class PositionUIModel : BaseNotify, ICloneable, IComparable
	{
		public PositionUIModel() { }

		public PositionUIModel(int id, int poskey, string opdate, string cldate, KindUIModel kind, string income, string expense, string saldo, CoinUIModel coin, StatusUIModel status, string? tag, string? note, int? _zeroParrentKey, int? _parrentKey)
		{
			Id = id;
			PositionKey = poskey;
			OpenDate = opdate;
			CloseDate = cldate;
			Kind = kind;
			Income = income;
			Expense = expense;
			Saldo = saldo;
			Coin = coin;
			Status = status;
			Tag = tag;
			Notes = note;
			ZeroParrentKey = _zeroParrentKey;
			ParrentKey = _parrentKey;
		}

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

		private string _openDate;
		public string OpenDate
		{
			get => _openDate;
			set => SetField(ref _openDate, value);
		}

		private string _closeDate;
		public string CloseDate
		{
			get => _closeDate;
			set => SetField(ref _closeDate, value);
		}

		private KindUIModel _kind;
		public KindUIModel Kind
		{
			get => _kind;
			set => SetField(ref _kind, value);
		}

		private string _income;
		public string Income
		{
			get => _income;
			set => SetField(ref _income, value);
		}

		private string _expense;
		public string Expense
		{
			get => _expense;
			set => SetField(ref _expense, value);
		}

		private string _saldo;
		public string Saldo
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

		private int? _zeroParrentKey;
        public int? ZeroParrentKey
		{
			get => _zeroParrentKey;
			set => SetField(ref _zeroParrentKey, value);
		}

		private int? _parrentKey;
		public int? ParrentKey
		{
			get => _parrentKey;
			set => SetField(ref _parrentKey, value);
		}

		public object Clone()
		{
			var cloneKind = new KindUIModel(Kind.Id, Kind.Kind, new CategoryUIModel() { Id = Kind.Category.Id, Category = Kind.Category.Category });
			var cloneCoin = new CoinUIModel() { Id = Coin.Id, ShortName = Coin.ShortName, FullName = Coin.FullName, CoinNotes = Coin.CoinNotes };
			var cloneStatus = new StatusUIModel() { Id = Status.Id, StatusName = Status.StatusName, StatusNotes = Status.StatusNotes };	
			return new PositionUIModel(_id, _positionKey, _openDate, _closeDate, cloneKind, _income, _expense, _saldo, cloneCoin, cloneStatus, _tag, _notes, _zeroParrentKey, _parrentKey);
		}

		public int CompareTo(object? obj)
		{
			//Сортировка от большей позиции к меньшей
            if (obj is PositionUIModel)
            {
				return -1 * PositionKey.CompareTo(((PositionUIModel)obj).PositionKey);
			}
			else throw new ArgumentException("Некорректное значение параметра");
		}
	}
}
