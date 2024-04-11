using MyMiniLedger.BLL;
using MyMiniLedger.BLL.Context;
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
	public class CategoryWindowModel : BaseNotify
	{
		private readonly Context _context;

		private string _title = "Окно редактированиея категорий";
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
		public LambdaCommand AddToCategory { get; set; }
		public LambdaCommand DeleteCategory { get; set; }

		public CategoryWindowModel()
		{
			SelectedCategory = new CategoryUIModel();
			_context = new BLL.Context.Context();

			//Инициализация категорий
			BLL.Context.ListOfCategories tempCat = new BLL.Context.ListOfCategories();
			List<CategoryUIModel> tempCategoriesAsync = tempCat.GetAllAsync().Result.Select(cat => Mappers.UIMapper.MapCategoryBLLToCategoryUI(cat)).ToList();
			Categories = new ObservableCollection<CategoryUIModel>(tempCategoriesAsync);

			AddToCategory = new LambdaCommand(
				async execute =>
				{
					//MessageBox.Show(SelectedCategory.Category);
					await _context.CategoriesTableBL.InsertAsync(Mappers.UIMapper.MapCategoryUIToCategoryBLL(_selectedCategory));
					var temp = _selectedCategory.Clone();
					Categories.Add((CategoryUIModel)temp);
				},
				canExecute => SelectedCategory is not null && Categories.Any(c => c.Category == _selectedCategory.Category) == false
				);

			
			DeleteCategory = new LambdaCommand(execute => { }, canExecute => SelectedCategory is not null);
		}

	}
}
