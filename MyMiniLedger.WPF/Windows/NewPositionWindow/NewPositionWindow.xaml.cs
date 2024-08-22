using Dapper;
using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.WindowsModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CurrentUICulture;
			dp_OpenDate.SelectedDate = DateTime.Now;

			(DataContext as MainWindowModel).UpdateDatePickerEvent += UpdateDatePicker;
		}

		private void cb_Coin_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (((CoinUIModel)cb_Coin.SelectedItem) != null)
			{
				((MainWindowModel)DataContext).PositionConstruct.Coin.Id = ((CoinUIModel)cb_Coin.SelectedItem).Id;
				((MainWindowModel)DataContext).PositionConstruct.Coin.ShortName = ((CoinUIModel)cb_Coin.SelectedItem).ShortName;
				((MainWindowModel)DataContext).PositionConstruct.Coin.FullName = ((CoinUIModel)cb_Coin.SelectedItem).FullName;
				((MainWindowModel)DataContext).PositionConstruct.Coin.CoinNotes = ((CoinUIModel)cb_Coin.SelectedItem).CoinNotes;
			}

		}

		private void tb_Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (((StatusUIModel)cb_Status.SelectedItem) != null)
			{
				((MainWindowModel)DataContext).PositionConstruct.Status.Id = ((StatusUIModel)cb_Status.SelectedItem).Id;
				((MainWindowModel)DataContext).PositionConstruct.Status.StatusName = ((StatusUIModel)cb_Status.SelectedItem).StatusName;
				((MainWindowModel)DataContext).PositionConstruct.Status.StatusNotes = ((StatusUIModel)cb_Status.SelectedItem).StatusNotes;
			}
		}

		private void Button_Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void dp_OpenDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender != null)
			{
				((MainWindowModel)DataContext).PositionConstruct.OpenDate = sender.ToString();
			}
		}

		private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			string str = ((TextBox)sender).Text + e.Text;

			if ((str.IndexOf('0') == 0 && str.IndexOf(',') != 1 && Char.IsDigit(e.Text, 0) && str != "0")
				|| (((TextBox)sender).SelectionStart == 0 && e.Text == "0" && ((TextBox)sender).Text.Length != 0)
				|| (((TextBox)sender).SelectionStart == 0 && !Char.IsDigit(e.Text, 0)))
			{
				if (((TextBox)sender).Text == "0" && e.Text != "0" && Char.IsDigit(e.Text, 0))
				{
					((TextBox)sender).Text = e.Text;
					((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
				}
				e.Handled = true;
				return;
			}
			e.Handled = !double.TryParse(str, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.CurrentUICulture, out double i);
		}
		private void textBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (((TextBox)sender).Text == string.Empty)
			{
				((TextBox)sender).Text = "0";
				((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
			}
		}
		//Работа с заполнением полей income
		private void tb_Income_PreviewTextInput_New(object sender, TextCompositionEventArgs e)
		{
			textBox_PreviewTextInput(sender, e);
		}
		private void tb_Income_TextChanged_New(object sender, TextChangedEventArgs e)
		{
			textBox_TextChanged(sender, e);
		}
		////Работа с заполнением полей expense
		private void tb_Expense_PreviewTextInput_New(object sender, TextCompositionEventArgs e)
		{
			textBox_PreviewTextInput(sender, e);
		}
		private void tb_Expense_TextChanged_New(object sender, TextChangedEventArgs e)
		{
			textBox_TextChanged(sender, e);
		}

		////Работа с заполнением полей income
		////Обработка ввода корректных символов и проверка на правильность написания десятичной дроби
		//private void tb_Income_PreviewTextInput(object sender, TextCompositionEventArgs e)
		//{
		//	if (!Char.IsDigit(e.Text, 0) && !((e.Text == ",") && (tb_Income.Text.IndexOf(',') == -1) && (tb_Income.Text.Length != 0)))
		//	{
		//		e.Handled = true;
		//	}
		//	else if (tb_Income.Text.IndexOf('0') == 0 && tb_Income.Text.IndexOf(',') != 1 && Char.IsDigit(e.Text, 0))
		//	{
		//		tb_Income.Text = e.Text;
		//		tb_Income.SelectionStart = 1;
		//		e.Handled = true;
		//	}
		//	else if (tb_Income.SelectionStart == 0 && e.Text == ",")
		//	{
		//		e.Handled = true;
		//	}
		//	else if (tb_Income.SelectionStart == 0 && e.Text == "0" && tb_Income.Text.Length != 0)
		//	{
		//		e.Handled = true;
		//	}
		//}
		////Отлов space
		//private void tb_Income_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		//{
		//	if (e.Key == Key.Space)
		//	{
		//		e.Handled = true;
		//	}
		//}
		////Перевод 59,960.17 -> 59960,17
		//private void tb_Income_TextChanged(object sender, TextChangedEventArgs e)
		//{
		//	if (tb_Income.Text.IndexOf(',') != -1 && tb_Income.Text.IndexOf('.') != -1)
		//	{
		//		tb_Income.Text = tb_Income.Text.Remove(tb_Income.Text.IndexOf(','), 1);
		//	}
		//	if (tb_Income.Text.IndexOf(',') == -1 && tb_Income.Text.IndexOf('.') != -1)
		//	{
		//		tb_Income.Text = tb_Income.Text.Replace(".", ",");
		//	}
		//	if (tb_Income.Text == "")
		//	{
		//		tb_Income.Text = "0";
		//		tb_Income.SelectionStart = 1;
		//	}
		//}
		////Работа с заполнением полей expense
		//private void tb_Expense_PreviewTextInput(object sender, TextCompositionEventArgs e)
		//{
		//	if (!Char.IsDigit(e.Text, 0) && !((e.Text == ",") && (tb_Expense.Text.IndexOf(',') == -1) && (tb_Expense.Text.Length != 0)))
		//	{
		//		e.Handled = true;
		//	}
		//	else if (tb_Expense.Text.IndexOf('0') == 0 && tb_Expense.Text.IndexOf(',') != 1 && Char.IsDigit(e.Text, 0))
		//	{
		//		tb_Expense.Text = e.Text;
		//		tb_Expense.SelectionStart = 1;
		//		e.Handled = true;
		//	}
		//	else if (tb_Expense.SelectionStart == 0 && e.Text == ",")
		//	{
		//		e.Handled = true;
		//	}
		//	else if (tb_Expense.SelectionStart == 0 && e.Text == "0" && tb_Expense.Text.Length != 0)
		//	{
		//		e.Handled = true;
		//	}
		//}

		//private void tb_Expense_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		//{
		//	if (e.Key == Key.Space)
		//	{
		//		e.Handled = true;
		//	}
		//}

		//private void tb_Expense_TextChanged(object sender, TextChangedEventArgs e)
		//{
		//	if (tb_Expense.Text.IndexOf(',') != -1 && tb_Expense.Text.IndexOf('.') != -1)
		//	{
		//		tb_Expense.Text = tb_Expense.Text.Remove(tb_Expense.Text.IndexOf(','), 1);
		//	}
		//	if (tb_Expense.Text.IndexOf(',') == -1 && tb_Expense.Text.IndexOf('.') != -1)
		//	{
		//		tb_Expense.Text = tb_Expense.Text.Replace(".", ",");
		//	}
		//	if (tb_Expense.Text == "")
		//	{
		//		tb_Expense.Text = "0";
		//		tb_Expense.SelectionStart = 1;
		//	}
		//}

		void UpdateDatePicker()
		{
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CurrentUICulture;
            dp_OpenDate.SelectedDate = DateTime.Today;
		}

	}
}



		
