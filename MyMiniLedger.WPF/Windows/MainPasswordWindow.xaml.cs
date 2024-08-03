using MyMiniLedger.WPF.WindowsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MyMiniLedger.DAL;
using MyMiniLedger.DAL.SQL;
using MyMiniLedger.DAL.Services;
using MyMiniLedger.DAL.Config;

namespace MyMiniLedger.WPF.Windows
{
	/// <summary>
	/// Interaction logic for MainPasswordWindow.xaml
	/// </summary>
	public partial class MainPasswordWindow : Window
	{
		StringBuilder Password;
		MainWindow MW;
		bool passOk = false;
		public MainPasswordWindow(ref StringBuilder _password, MainWindow _mw)
		{
			InitializeComponent();
			Password = _password;
			MW = _mw;
		}

		private void bt_Ok_Click(object sender, RoutedEventArgs e)
		{
			Password.Append(PassBox.Password);

			DataConfig.Init("config.json", Password.ToString());
			TablePositions positions = new TablePositions();
			var x = positions.GetAll();
			if (x != null)
			{
				MW.Valid = true;
				passOk = true;
				Close();
			}
			else
			{
				PassBox.Clear();
				MessageBox.Show("Введен неверный пароль", "Ввод пароля", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}

		private void bt_Exit_Click(object sender, RoutedEventArgs e)
		{
			MW.Valid = false;
            Close();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			if (!passOk)
			{
				MW.Valid = false;
			}
		}
	}
}
