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

			//(DataContext as CategoryWindowModel).
		}

		private void CategoriesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			tb_EditCategory.Text = ((CategoryUIModel)CategoriesList.SelectedItem).Category.ToString();
		}

		private void ButtonExit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void tb_EditCategory_TextChanged(object sender, TextChangedEventArgs e)
		{
		
		}

	
	}
}
