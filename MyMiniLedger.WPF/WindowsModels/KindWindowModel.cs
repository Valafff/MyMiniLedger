using MyMiniLedger.BLL.Context;
using MyMiniLedger.BLL.Models;
using MyMiniLedger.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyMiniLedger.WPF.WindowsModels
{
	public delegate void UpdateKindDelegate();

	public class KindWindowModel : BaseNotify
	{
		//При срабатывании события происходит выполнение метода в MainWindowModel там-же какскадом срабатывает выбор 0го индекса в комбобоксе
		public event UpdateKindDelegate UpdateKindEvent;

		private readonly Context _context;

		private string _title = "Окно редактирования видов позиций";
		public string Title
		{
			get => _title;
			set => SetField(ref _title, value);
		}

		private KindUIModel? _selectedKind;
		public KindUIModel? SelectedKind
		{
			get => _selectedKind;
			set => SetField(ref _selectedKind, value);
		}

		//Для вложенного объекта
		private CategoryUIModel? _selectedCategory;
		public CategoryUIModel? SelectedCategory
		{
			get => _selectedCategory;
			set => SetField(ref _selectedCategory, value);
		}



		public ObservableCollection<KindUIModel>? Kinds { get; set; }
		public ObservableCollection<PositionUIModel> Positions { get; set; }

		//Для правильной инициализации комбобокса
		public ObservableCollection<CategoryUIModel>? Categories { get; set; }

		public LambdaCommand AddToKind { get; set; }
		public LambdaCommand DeleteKind { get; set; }
		public LambdaCommand UpdateKind { get; set; }

		public KindWindowModel()
		{

			SelectedKind = new KindUIModel();
			SelectedCategory = new CategoryUIModel();

			_context = new BLL.Context.Context();

			////Инициализация позиций для определения связей
			BLL.Context.ListOfPositions tempPos = new BLL.Context.ListOfPositions();
			//List<PositionBLLModel> tempPosAsync = tempPos.GetAll().Result.ToList();
			List<PositionBLLModel> tempPosAsync = tempPos.GetAll().ToList();

			////Инициализация видов
			BLL.Context.ListOfKinds tempKind = new BLL.Context.ListOfKinds();
			//List<KindUIModel> tempKindsAsync = tempKind.GetAll().Result.Select(k => Mappers.UIMapper.MapKindBLLToKindUI(k)).ToList();
			List<KindUIModel> tempKindsAsync = tempKind.GetAll().Select(k => Mappers.UIMapper.MapKindBLLToKindUI(k)).ToList();

			//Инициализация категорий Для правильной инициализации комбобокса
			BLL.Context.ListOfCategories tempCat = new BLL.Context.ListOfCategories();
			List<CategoryUIModel> tempCategoriesAsync = tempCat.GetAll().Result.Select(cat => Mappers.UIMapper.MapCategoryBLLToCategoryUI(cat)).ToList();
			Categories = new ObservableCollection<CategoryUIModel>(tempCategoriesAsync);

			//Определение количества ссылок по Id
			for (int i = 0; i < tempKindsAsync.Count; i++)
			{
				for (int j = 0; j < tempPosAsync.Count; j++)
				{
					if (tempKindsAsync[i].Id == tempPosAsync[j].Kind.Id)
					{
						tempKindsAsync[i].RefNumber++;
					}
				}
			}

			Kinds = new ObservableCollection<KindUIModel>(tempKindsAsync);


			//Добавление видв
			AddToKind = new LambdaCommand(
				async execute =>
				{
					//Добавление
					_selectedKind.Category = SelectedCategory;
					//await _context.KindsTableBL.Insert(Mappers.UIMapper.MapKindUIToKindBLL(_selectedKind));
					_context.KindsTableBL.Insert(Mappers.UIMapper.MapKindUIToKindBLL(_selectedKind));

					//Обновление списка UI
					//var updatedKind = (tempKind.GetAll().Result.Select(k => Mappers.UIMapper.MapKindBLLToKindUI(k)).ToList()).Where(t => t.Kind == _selectedKind.Kind);
					var updatedKind = (tempKind.GetAll().Select(k => Mappers.UIMapper.MapKindBLLToKindUI(k)).ToList()).Where(t => t.Kind == _selectedKind.Kind);
					var temp = _selectedKind.Clone();
					((KindUIModel)temp).Id = updatedKind.First().Id;
					Kinds.Add((KindUIModel)temp);
					UpdateKindEvent();
				},
				canExecute => SelectedKind is not null &&
				SelectedKind.Kind != null &&
				//Подумать над возможностью добавлять одинаковые названия видов при отличающихся категориях
				SelectedKind.Kind != string.Empty
				);

			//Полное удаление вида
			DeleteKind = new LambdaCommand(async execute =>
			{
				//Удаление
				var result = MessageBox.Show("Подтвердите удаление вида", "Удаление вида", MessageBoxButton.YesNo, MessageBoxImage.Warning);
				if (result == MessageBoxResult.Yes)
				{
					_context.KindsTableBL.Delete(Mappers.UIMapper.MapKindUIToKindBLL(SelectedKind));
					//Обновление списка UI
					var t = Kinds.Where(t => t.Id == SelectedKind.Id);
					Kinds.Remove(t.First());
					SelectedKind.Id = 0;
					SelectedKind.Kind = null;
					UpdateKindEvent();
				}
			},
			canExecute => SelectedKind is not null && SelectedKind.Kind != null && SelectedKind.Id != 0 && SelectedKind.RefNumber == 0);

			//Редактирование вида
			UpdateKind = new LambdaCommand(async execute =>
			{
				_context.KindsTableBL.Update(Mappers.UIMapper.MapKindUIToKindBLL(SelectedKind));
				foreach (var kind in Kinds)
				{
					if (kind.Id == SelectedKind.Id)
					{
						kind.Kind = SelectedKind.Kind;
						kind.RefNumber = SelectedKind.RefNumber;
					}
				}
				UpdateKindEvent();
			},
			canExecute => DeleteKind is not null && SelectedKind.Kind != null && SelectedKind.Id != 0 && SelectedKind.Kind != string.Empty);
		}
	}
}
