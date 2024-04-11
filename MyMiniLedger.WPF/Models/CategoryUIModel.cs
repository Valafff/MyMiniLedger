
namespace MyMiniLedger.WPF.Models
{
	// BaseNotify реализует интерфейс INotifyPropertyChanged
	public class CategoryUIModel : BaseNotify, ICloneable
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

		//Для определения наличия связей к привязанной таблицк
		private int _categoryid;
		public int RefNumber
		{
			get => _categoryid;
			set => SetField(ref _categoryid, value);
		}

		public object Clone() => MemberwiseClone();
	}
}
