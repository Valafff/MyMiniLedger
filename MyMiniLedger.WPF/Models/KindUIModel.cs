
namespace MyMiniLedger.WPF.Models
{
	// BaseNotify реализует интерфейс INotifyPropertyChanged
	public class KindUIModel: BaseNotify, ICloneable, IComparable<KindUIModel>
    {
		public KindUIModel() { }

        public KindUIModel(int id, string kind, CategoryUIModel cat)
        {
            Id = id;
			Kind = kind;
			Category = cat;
        }

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

		//Для определения наличия связей к привязанной таблице позиции
		private int _refumber;
		public int RefNumber
		{
			get => _refumber;
			set => SetField(ref _refumber, value);
		}

		//Глубокое копирование
		public object Clone() => new KindUIModel(this._id, this._kind, new CategoryUIModel() { Id = this._category.Id, Category = this._category.Category });

		public int CompareTo(KindUIModel? other)
		{
			return string.Compare(this.Kind, other.Kind);
		}
	}
}
