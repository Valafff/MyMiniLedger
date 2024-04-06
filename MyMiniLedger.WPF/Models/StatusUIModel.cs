
namespace MyMiniLedger.WPF.Models
{
	// BaseNotify реализует интерфейс INotifyPropertyChanged
	public class StatusUIModel: BaseNotify
    {
		private int _id;
		public int Id 
		{
			get => _id;
			set => SetField(ref _id, value);
		}

		private string _statusName;
		public string StatusName
		{ 
			get => _statusName;
			set => SetField(ref _statusName, value);
		}

		private string _statusNotes;
		public string? StatusNotes
		{
			get => _statusNotes;
			set => SetField(ref _statusNotes, value);
		}
	}
}
