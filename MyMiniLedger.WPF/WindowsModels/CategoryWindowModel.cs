using MyMiniLedger.BLL;
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
	public delegate void UpdateCategoryDelegate();

	public class CategoryWindowModel : BaseNotify
	{
		//При срабатывании события происходит выполнение метода в MainWindowModel там-же какскадом срабатывает выбор 0го индекса в комбобоксе
		public event UpdateCategoryDelegate UpdateCategoryEvent;

		private readonly Context _context;

		private string _title = "Окно редактирования категорий";
		public string Title
		{
			get => _title;
			set => SetField(ref _title, value);
		}

		private CategoryUIModel? _selectedCategory;
		public CategoryUIModel? SelectedCategory
		{
			get => _selectedCategory;
			set => SetField(ref _selectedCategory, value);
		}


		public ObservableCollection<CategoryUIModel>? Categories { get; set; }
		public ObservableCollection<KindUIModel> Kinds { get; set; }

		public LambdaCommand AddToCategory { get; set; }
		public LambdaCommand DeleteCategory { get; set; }
		public LambdaCommand UpdateCategory { get; set; }


		public CategoryWindowModel()
		{
			SelectedCategory = new CategoryUIModel();

			_context = new BLL.Context.Context();

			//Инициализация видов для определения связей
			BLL.Context.ListOfKinds tempKind = new BLL.Context.ListOfKinds();
			List<KindBLLModel> tempKindsAsync = tempKind.GetAllAsync().Result.ToList();

			//Инициализация категорий
			BLL.Context.ListOfCategories tempCat = new BLL.Context.ListOfCategories();
			List<CategoryUIModel> tempCategoriesAsync = tempCat.GetAllAsync().Result.Select(cat => Mappers.UIMapper.MapCategoryBLLToCategoryUI(cat)).ToList();

			//Определение количества ссылок по Id
			for (int i = 0; i < tempCategoriesAsync.Count; i++)
			{
				for (int j = 0; j < tempKindsAsync.Count; j++)
				{
					if (tempCategoriesAsync[i].Id == tempKindsAsync[j].Category.Id)
					{
						tempCategoriesAsync[i].RefNumber++;
					}
				}
			}

			Categories = new ObservableCollection<CategoryUIModel>(tempCategoriesAsync);

			

			//Добавление категории
			AddToCategory = new LambdaCommand(
				async execute =>
				{
					_selectedCategory.RefNumber = 0;
					await _context.CategoriesTableBL.InsertAsync(Mappers.UIMapper.MapCategoryUIToCategoryBLL(_selectedCategory));
					var updatedCat = (tempCat.GetAllAsync().Result.Select(cat => Mappers.UIMapper.MapCategoryBLLToCategoryUI(cat)).ToList()).Where(t => t.Category == _selectedCategory.Category);
					
					var temp = _selectedCategory.Clone();
					((CategoryUIModel)temp).Id = updatedCat.First().Id;
					Categories.Add((CategoryUIModel)temp);
					UpdateCategoryEvent();
				},
				canExecute => SelectedCategory is not null && SelectedCategory.Category != null && Categories.Any(c => c.Category == _selectedCategory.Category) == false
				);

			//Полное удаление категории
			DeleteCategory = new LambdaCommand(async execute =>
			{
				await _context.CategoriesTableBL.DeleteAsync(Mappers.UIMapper.MapCategoryUIToCategoryBLL(_selectedCategory));
				var t = Categories.Where(t => t.Id == _selectedCategory.Id);
				Categories.Remove(t.First());
				_selectedCategory.Id = 0;
				SelectedCategory.Category = null;
				UpdateCategoryEvent();
			},
			canExecute => SelectedCategory is not null && SelectedCategory.Category != null && SelectedCategory.Id != 0 && _selectedCategory.RefNumber == 0);

			//Редактирование категории
			UpdateCategory = new LambdaCommand(async execute =>
			{
				await _context.CategoriesTableBL.UpdateAsync(Mappers.UIMapper.MapCategoryUIToCategoryBLL(_selectedCategory));				
				var t = Categories.Where(t => t.Id == _selectedCategory.Id);
				Categories.Remove(t.First());
				var temp = _selectedCategory.Clone();
				Categories.Add((CategoryUIModel)temp);
				UpdateCategoryEvent();
			},
			canExecute => SelectedCategory is not null && SelectedCategory.Category != null && SelectedCategory.Id != 0 && Categories.Any(c => c.Category == _selectedCategory.Category) == false);
		}

	}
}
