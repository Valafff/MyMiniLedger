using MyMiniLedger.WPF.Models;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyMiniLedger.WPF.Windows.NewPositionWindow
{
    /// <summary>
    /// Interaction logic for NewPositionWindow.xaml
    /// </summary>
    public partial class NewPositionWindow : Window
    {
        public NewPositionWindow(MainWindowModel _mainWindowModel)
        {
			InitializeComponent();
			DataContext = _mainWindowModel;
			dp_OpenDate.SelectedDate = DateTime.Now;
		}

		private void ComboBox_Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((MainWindowModel)DataContext).PositionConstruct.Kind.Category.Category = ((CategoryUIModel)cb_Category.SelectedItem).Category;
			((MainWindowModel)DataContext).PositionConstruct.Kind.Category.Id = ((CategoryUIModel)cb_Category.SelectedItem).Id;
		}

		private void ComboBox_Kind_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((MainWindowModel)DataContext).PositionConstruct.Kind.Kind = ((KindUIModel)cb_Kind.SelectedItem).Kind;
			((MainWindowModel)DataContext).PositionConstruct.Kind.Id = ((KindUIModel)cb_Kind.SelectedItem).Id;
			((MainWindowModel)DataContext).PositionConstruct.Kind.Category = ((KindUIModel)cb_Kind.SelectedItem).Category;
		}

		private void TextBox_Income_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			textBoxinsertNumber(sender, e, tb_Income);
		}

		private void TextBox_Expense_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			textBoxinsertNumber(sender, e, tb_Expense);
		}

		private void cb_Coin_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((MainWindowModel)DataContext).PositionConstruct.Coin.Id = ((CoinUIModel)cb_Coin.SelectedItem).Id;
			((MainWindowModel)DataContext).PositionConstruct.Coin.ShortName = ((CoinUIModel)cb_Coin.SelectedItem).ShortName;
			((MainWindowModel)DataContext).PositionConstruct.Coin.FullName = ((CoinUIModel)cb_Coin.SelectedItem).FullName;
			((MainWindowModel)DataContext).PositionConstruct.Coin.CoinNotes = ((CoinUIModel)cb_Coin.SelectedItem).CoinNotes;
		}

		private void tb_Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((MainWindowModel)DataContext).PositionConstruct.Status.Id = ((StatusUIModel)cb_Status.SelectedItem).Id;
			((MainWindowModel)DataContext).PositionConstruct.Status.StatusName = ((StatusUIModel)cb_Status.SelectedItem).StatusName;
			((MainWindowModel)DataContext).PositionConstruct.Status.StatusNotes = ((StatusUIModel)cb_Status.SelectedItem).StatusNotes;
		}

		private void Button_Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void dp_OpenDate_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			((MainWindowModel)DataContext).PositionConstruct.OpenDate = dp_OpenDate.DisplayDate.ToString();
		}



		void textBoxinsertNumber(object sender, System.Windows.Input.KeyEventArgs e, System.Windows.Controls.TextBox textBox)
		{
			////Замена точки на запятую для ввода с разных раскладок
			//if (e.Key == Key.OemPeriod)
			//{
			//	e.Source = Key.OemComma;
			//}

			//Перевод в digit
			var t = (char)KeyInterop.VirtualKeyFromKey(e.Key);
			if (e.Key == Key.NumPad0)
			{
				t = '0';
			}
			if (e.Key == Key.NumPad1)
			{
				t = '1';
			}
			if (e.Key == Key.NumPad2)
			{
				t = '2';
			}
			if (e.Key == Key.NumPad3)
			{
				t = '3';
			}
			if (e.Key == Key.NumPad4)
			{
				t = '4';
			}
			if (e.Key == Key.NumPad5)
			{
				t = '5';
			}
			if (e.Key == Key.NumPad6)
			{
				t = '6';
			}
			if (e.Key == Key.NumPad7)
			{
				t = '7';
			}
			if (e.Key == Key.NumPad8)
			{
				t = '8';
			}
			if (e.Key == Key.NumPad9)
			{
				t = '9';
			}


			if (!Char.IsDigit(t) && !((e.Key == Key.OemComma) && (textBox.Text.IndexOf(",") == -1) && (textBox.Text.Length != 0)))
			{
				if ((char)KeyInterop.VirtualKeyFromKey(e.Key) != (char)Keys.Back)
				{
					e.Handled = true;
				}
			}
			else if (textBox.Text.IndexOf('0') == 0 && textBox.Text.IndexOf(',') != 1 && t == '0')
			{
				e.Handled = true;
			}
		}


	}
}
