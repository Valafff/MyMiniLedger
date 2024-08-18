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
using System.IO;
using System.Text.Json;
using Microsoft.Win32;

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
			PassBox.Focus();
			Password = _password;
			MW = _mw;
		}

		private void bt_Ok_Click(object sender, RoutedEventArgs e)
		{
			Password.Clear();
			Password.Append(PassBox.Password);

			////затычка для быстрого входа
			//Password.Append("123");

			DataConfig.ResetConnection();
			DataConfig.Init("config.json", Password.ToString());
			TablePositions positions = new TablePositions();
			var x = positions.GetAllForPass();
			if (x != null)
			{
				MW.Valid = true;
				passOk = true;
				Close();
			}
			else
			{
				PassBox.Clear();
				MessageBox.Show("Введен неверный пароль или ошибка чтения БД", "Ввод пароля", MessageBoxButton.OK, MessageBoxImage.Error);
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

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
                bt_Ok_Click(sender, e);
			}
		}

		private void bt_ResetPathToBD_Click(object sender, RoutedEventArgs e)
		{
			string path = string.Empty;
			OpenFileDialog fd = new OpenFileDialog();
			if (fd.ShowDialog().Value)
			{
				try
				{
					Console.WriteLine(fd.FileName);
					path = fd.FileName;

					var newConfig = Config.GetFromConfig();
					newConfig.DataSource = path;
					using var toFile = new FileStream("config.json", FileMode.Truncate, FileAccess.Write);
					JsonSerializer.Serialize(toFile, newConfig);
					MessageBox.Show("Задан новый путь к БД");
				}
				catch (Exception)
				{
					MessageBox.Show("Путь к файлу БД задан неверно!");
				}
			}
			else
			{
				MessageBox.Show("Путь к файлу БД не указан!");
			}
		}
	}
}
