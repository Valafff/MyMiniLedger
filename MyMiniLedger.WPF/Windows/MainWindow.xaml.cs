using System.Text;
using System.Windows;
using MyMiniLedger.BLL.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyMiniLedger.WPF.Models;
using System.Collections.ObjectModel;

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
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}

}