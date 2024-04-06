
namespace MyMiniLedger.WPF.Models
{
	// BaseNotify реализует интерфейс INotifyPropertyChanged
	public class KindUIModel: BaseNotify
    {
		private int _id;
		public int Id 
		{
			get => _id;
			set => SetField(ref _id, value);
		}

		private CategoryUIModel _category;
		public CategoryUIModel Category
		{
			get => _category;
			set => SetField(ref _category, value);
		}

		private string _kind;
		public string Kind
		{
			get => _kind;
			set => SetField(ref _kind, value);
		}
	}
}
