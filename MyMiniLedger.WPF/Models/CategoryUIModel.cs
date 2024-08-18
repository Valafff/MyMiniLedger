
namespace MyMiniLedger.WPF.Models
{
	// BaseNotify реализует интерфейс INotifyPropertyChanged
	public class CategoryUIModel : BaseNotify, ICloneable, IComparable<CategoryUIModel>
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

		//Для определения наличия связей к привязанной таблице виды
		private int _refumber;
		public int RefNumber
		{
			get => _refumber;
			set => SetField(ref _refumber, value);
		}

		public object Clone() => MemberwiseClone();

		public int CompareTo(CategoryUIModel? other)
		{
			return string.Compare(this.Category, other.Category);
		}
	}
}
