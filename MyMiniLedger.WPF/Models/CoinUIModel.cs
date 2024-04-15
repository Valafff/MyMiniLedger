
namespace MyMiniLedger.WPF.Models
{
	// BaseNotify реализует интерфейс INotifyPropertyChanged
	public class CoinUIModel: BaseNotify, ICloneable
    {
		private int _id;
		public int Id 
		{ 
			get => _id; 
			set => SetField(ref _id, value);
		}

		private string _shortName;
		public string ShortName
		{
			get => _shortName;
			set => SetField(ref _shortName, value);
		}

		private string _fullName;
		public string FullName
		{
			get => _fullName;
			set => SetField(ref _fullName, value);
		}

		private string? _coinNotes;
		public string? CoinNotes
		{
			get => _coinNotes;
			set => SetField(ref _coinNotes, value);
		}

		//Для определения наличия связей к привязанной таблице виды
		private int _refumber;
		public int RefNumber
		{
			get => _refumber;
			set => SetField(ref _refumber, value);
		}

		public object Clone() => MemberwiseClone();
	}
}
