using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.WindowsModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace MyMiniLedger.WPF.Windows.CategoryWindow
{
	/// <summary>
	/// Interaction logic for CategoryWindow.xaml
	/// </summary>
	public partial class CategoryWindow : Window
	{
		public CategoryWindow()
		{
			InitializeComponent();

			//Достучаться до полей WindowModel
			//(DataContext as CategoryWindowModel).
		}

		private void CategoriesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			tb_EditCategory.Text = ((CategoryUIModel)CategoriesList.SelectedItem).Category.ToString();
			(DataContext as CategoryWindowModel).SelectedCategory.Id = ((CategoryUIModel)CategoriesList.SelectedItem).Id;
			(DataContext as CategoryWindowModel).SelectedCategory.RefNumber = ((CategoryUIModel)CategoriesList.SelectedItem).RefNumber;

		}

		private void ButtonExit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}


	
	}
}
