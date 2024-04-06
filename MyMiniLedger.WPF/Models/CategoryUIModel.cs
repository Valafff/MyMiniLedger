
namespace MyMiniLedger.WPF.Models
{
	// BaseNotify реализует интерфейс INotifyPropertyChanged
	public class CategoryUIModel : BaseNotify
    {
		private int _id;
		public int Id 
		{ 
			get => _id;
			set => SetField(ref _id, value); 
		}

		private string _category;
		public string Category 
		{ 
			get => _category; 
			set => SetField(ref _category, value);
		}

	}
}
