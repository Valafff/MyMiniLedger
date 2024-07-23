using Dapper;
using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.ViewTools;
using MyMiniLedger.WPF.Windows.EditContinuePosition;
using MyMiniLedger.WPF.WindowsModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            datePiker_Start.SelectedDate = new DateTime(DateTime.Now.Year, 1, 1);
			datePiker_End.SelectedDate = DateTime.Now;
			dp_OpenDate.SelectedDate = DateTime.Now;

			(DataContext as MainWindowModel).UpdateCoinsIndexEvent += UpdateSelectedCoin;
			(DataContext as MainWindowModel).UpdateCategoriesIndexEvent += UpdateSelectedCategory;
			(DataContext as MainWindowModel).UpdateKindsIndexEvent += UpdateSelectedKind;
			(DataContext as MainWindowModel).UpdateDatePickerEvent += UpdateDatePicker;
		}

		//Удаление позиции
		private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				(DataContext as MainWindowModel).DeleteSelectedPosDataGrid();
			}
		}

		private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
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

		private void DataGrid_SelectionChanged_MainWindow(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			((MainWindowModel)DataContext).SelectedPosition = (PositionUIModel)(((DataGrid)sender).CurrentItem);

            ComplexPositionBalanceCalculator calculator = new ComplexPositionBalanceCalculator();
			var selectedBalance = calculator.GetTotalBalance((DataContext as MainWindowModel).Positions, ((MainWindowModel)DataContext).SelectedPosition);

			if (selectedBalance == null)
			{
				selectedBalance = new TotalBalance() { Balance = 0, CoinName = "[Валюта]" };
			}
			tb_CurrentIncome.Text = selectedBalance.TotalIncome.ToString();
			tb_CurrentIncome.Foreground = Brushes.Green;
			tb_CurrentExpence.Text = selectedBalance.TotalExpense.ToString();
			tb_CurrentExpence.Foreground = Brushes.Red;
			tb_CurrentBalance.Text = selectedBalance.Balance.ToString();
			if (selectedBalance.Balance > 0)
			{
				tb_CurrentBalance.Foreground = Brushes.Green;
			}
			else if (selectedBalance.Balance < 0)
			{
				tb_CurrentBalance.Foreground = Brushes.Red;
			}
			else
			{
				tb_CurrentBalance.Foreground= Brushes.Black;
			}
			tb_CurrentCoin.Text = selectedBalance.CoinName;

			//Не позваляет DatePicker сползти в некорректный формат
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CurrentUICulture;
        }
		private void dp_OpenDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender != null)
			{
				((MainWindowModel)DataContext).PositionConstruct.OpenDate = sender.ToString();
			}
		}

		private void ComboBox_Category_SelectionChanged_MainWindow(object sender, SelectionChangedEventArgs e)
		{
			((MainWindowModel)DataContext).TempKindsMain.Clear();
			foreach (var item in ((MainWindowModel)DataContext).Kinds.AsList())
			{
				if (cb_Category.SelectedItem != null)
				{
					if (((System.Windows.Controls.ComboBox)sender).SelectedItem.ToString() == item.Category.Category)
					{
						((MainWindowModel)DataContext).TempKindsMain.Add(item);
					}
				}
			}

			if (cb_Kind != null)
			{
				cb_Kind.SelectedIndex = 0;
			}

			if (((MainWindowModel)DataContext).TempKindsMain.Count > 0)
			{
				((MainWindowModel)DataContext).PositionConstruct.Kind = ((MainWindowModel)DataContext).TempKindsMain[0];
			}
		}

		private void cb_Kind_TextChanged_MainWindow(object sender, TextChangedEventArgs e)
		{
			if (cb_Kind.Text != "" & cb_Kind.Text != null)
			{
				foreach (var item in ((MainWindowModel)DataContext).TempKindsMain)
				{
					if (item.Kind == cb_Kind.Text)
					{
						return;
					}
				}
				((MainWindowModel)DataContext).TempKindsMain.Clear();
				foreach (var item in ((MainWindowModel)DataContext).Kinds.AsList())
				{
					((MainWindowModel)DataContext).TempKindsMain.Add(item);

				}
			}
		}

		private void cb_Kind_SelectionChanged_MainWindow(object sender, SelectionChangedEventArgs e)
		{
			if (cb_Kind.SelectedIndex >= 0)
			{
				var temp = ((MainWindowModel)DataContext).TempKindsMain[cb_Kind.SelectedIndex];
				cb_Category.SelectedItem = temp.Category.Category;
			}
		}


		//Работа с заполнением полей income
		//Обработка ввода корректных символов и проверка на правильность написания десятичной дроби
		private void tb_Income_PreviewTextInput_MainWindow(object sender, TextCompositionEventArgs e)
		{
			if (!Char.IsDigit(e.Text, 0) && !((e.Text == ",") && (tb_Income.Text.IndexOf(',') == -1) && (tb_Income.Text.Length != 0)))
			{
				e.Handled = true;
			}
			else if (tb_Income.Text.IndexOf('0') == 0 && tb_Income.Text.IndexOf(',') != 1 && Char.IsDigit(e.Text, 0))
			{
				tb_Income.Text = e.Text;
				tb_Income.SelectionStart = 1;
				e.Handled = true;
			}
			else if (tb_Income.SelectionStart == 0 && e.Text == ",")
			{
				e.Handled = true;
			}
			else if (tb_Income.SelectionStart == 0 && e.Text == "0" && tb_Income.Text.Length != 0)
			{
				e.Handled = true;
			}
		}
		//Отлов space
		private void tb_Income_PreviewKeyDown_MainWindow(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				e.Handled = true;
			}
		}
		//Перевод 59,960.17 -> 59960,17
		private void tb_Income_TextChanged_MainWindow(object sender, TextChangedEventArgs e)
		{
			if (tb_Income.Text.IndexOf(',') != -1 && tb_Income.Text.IndexOf('.') != -1)
			{
				tb_Income.Text = tb_Income.Text.Remove(tb_Income.Text.IndexOf(','), 1);
			}
			if (tb_Income.Text.IndexOf(',') == -1 && tb_Income.Text.IndexOf('.') != -1)
			{
				tb_Income.Text = tb_Income.Text.Replace(".", ",");
			}
			if (tb_Income.Text == "")
			{
				tb_Income.Text = "0";
				tb_Income.SelectionStart = 1;
			}
		}

		//Работа с заполнением полей expense
		private void tb_Expense_PreviewTextInput_MainWindow(object sender, TextCompositionEventArgs e)
		{
			if (!Char.IsDigit(e.Text, 0) && !((e.Text == ",") && (tb_Expense.Text.IndexOf(',') == -1) && (tb_Expense.Text.Length != 0)))
			{
				e.Handled = true;
			}
			else if (tb_Expense.Text.IndexOf('0') == 0 && tb_Expense.Text.IndexOf(',') != 1 && Char.IsDigit(e.Text, 0))
			{
				tb_Expense.Text = e.Text;
				tb_Expense.SelectionStart = 1;
				e.Handled = true;
			}
			else if (tb_Expense.SelectionStart == 0 && e.Text == ",")
			{
				e.Handled = true;
			}
			else if (tb_Expense.SelectionStart == 0 && e.Text == "0" && tb_Expense.Text.Length != 0)
			{
				e.Handled = true;
			}
		}

		private void tb_Expense_PreviewKeyDown_MainWindow(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				e.Handled = true;
			}
		}

		private void tb_Expense_TextChanged_MainWindow(object sender, TextChangedEventArgs e)
		{
			if (tb_Expense.Text.IndexOf(',') != -1 && tb_Expense.Text.IndexOf('.') != -1)
			{
				tb_Expense.Text = tb_Expense.Text.Remove(tb_Expense.Text.IndexOf(','), 1);
			}
			if (tb_Expense.Text.IndexOf(',') == -1 && tb_Expense.Text.IndexOf('.') != -1)
			{
				tb_Expense.Text = tb_Expense.Text.Replace(".", ",");
			}
			if (tb_Expense.Text == "")
			{
				tb_Expense.Text = "0";
				tb_Expense.SelectionStart = 1;
			}
		}

		private void cb_Coin_SelectionChanged_MainWindow(object sender, SelectionChangedEventArgs e)
		{
			if (((CoinUIModel)cb_Coin.SelectedItem) != null)
			{
				((MainWindowModel)DataContext).PositionConstruct.Coin.Id = ((CoinUIModel)cb_Coin.SelectedItem).Id;
				((MainWindowModel)DataContext).PositionConstruct.Coin.ShortName = ((CoinUIModel)cb_Coin.SelectedItem).ShortName;
				((MainWindowModel)DataContext).PositionConstruct.Coin.FullName = ((CoinUIModel)cb_Coin.SelectedItem).FullName;
				((MainWindowModel)DataContext).PositionConstruct.Coin.CoinNotes = ((CoinUIModel)cb_Coin.SelectedItem).CoinNotes;
			}
		}

		private void tb_Status_SelectionChanged_MainWindow(object sender, SelectionChangedEventArgs e)
		{
			if (((StatusUIModel)cb_Status.SelectedItem) != null)
			{
				((MainWindowModel)DataContext).PositionConstruct.Status.Id = ((StatusUIModel)cb_Status.SelectedItem).Id;
				((MainWindowModel)DataContext).PositionConstruct.Status.StatusName = ((StatusUIModel)cb_Status.SelectedItem).StatusName;
				((MainWindowModel)DataContext).PositionConstruct.Status.StatusNotes = ((StatusUIModel)cb_Status.SelectedItem).StatusNotes;
			}
		}

		void UpdateSelectedCoin()
		{
			cb_Coin.SelectedIndex = 0;
		}

		void UpdateSelectedCategory()
		{
			cb_Category.SelectedIndex = 0;
		}

		void UpdateSelectedKind()
		{
			cb_Kind.SelectedIndex = 0;
		}

		void UpdateDatePicker()
		{
			dp_OpenDate.SelectedDate = DateTime.Today;
		}

		private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			EditContinuePositionWindow ecw = new EditContinuePositionWindow((DataContext as MainWindowModel).SelectedPosition,
				(DataContext as MainWindowModel).Positions,
				(DataContext as MainWindowModel).Categories,
				(DataContext as MainWindowModel).Kinds,
				(DataContext as MainWindowModel).Coins,
				(DataContext as MainWindowModel).StatusesForService);
			//Событие обновляющие  список позиций гланого окна
			(ecw.DataContext as EditContinuePositionWindowsModel).UpdateEvent += (DataContext as MainWindowModel).UpdatePositionsCollection;
			ecw.ShowDialog();
		}
	}

}