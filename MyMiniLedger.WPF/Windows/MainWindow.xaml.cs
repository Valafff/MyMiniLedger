using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.WindowsModels;
using System.Windows;
using System.Windows.Controls;

namespace MyMiniLedger.WPF
{

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			// Не получилось забиндить через VM
			datePiker_Start.SelectedDate = new DateTime(DateTime.Now.Year, 1, 1);
			datePiker_End.SelectedDate = DateTime.Now;

			//// Не получилось забиндить через VM
			//List<string> searchTypes = new List<string> { "Номер позиции", "Категория", "Вид", "Валюта", "Тег", "Статус" };
			//cb_Search_MenuItem.ItemsSource = searchTypes;
			//cb_Search_MenuItem.SelectedIndex = 0;
			//(this.DataContext as MainWindowModel).cf.
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		//Закрытие всех окон через событие
		private void Window_Closed(object sender, EventArgs e)
		{
			foreach (Window windows in App.Current.Windows)
			{
				windows.Close();
			}
		}

		private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			((MainWindowModel)DataContext).SelectedPosition = (PositionUIModel)(((DataGrid)sender).CurrentItem);
		}
	}

}