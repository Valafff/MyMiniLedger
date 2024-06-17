using Dapper;
using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.WindowsModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace MyMiniLedger.WPF.Windows.EditContinuePosition
{
	public partial class EditContinuePositionWindow : Window
	{
		//Блокировка выполнения событий при инициализации
		bool isLoaded = false;
		public EditContinuePositionWindow(PositionUIModel _selectedPosition, ObservableCollection<PositionUIModel> _mainPositionsSourse)
		{
			EditContinuePositionWindowsModel model = new EditContinuePositionWindowsModel() { MAINPOSITIONSCOLLECTION = _mainPositionsSourse };
			model.SelectedPosition = (PositionUIModel)_selectedPosition.Clone();
			model.OriginalSelectedPosition = (PositionUIModel)_selectedPosition.Clone();
			model.SelectedPositionsInicailization((model.SelectedPositions));
			//Инициализируется в кодбехайнд тк селектед позитион в конструкторе не определена
			model.SelectedCategory = _selectedPosition.Kind.Category.Category;
			model.TempKindInicialization();

			InitializeComponent();

			DataContext = model;
			GetSelectedKind();
			GetSelectedCoin();
			GetSelectedStatus();

			(DataContext as EditContinuePositionWindowsModel).UpdateEvent += ResetColors;

			isLoaded = true;
		}

		private void dp_OpenDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender != null && sender.ToString() != "" && isLoaded)
			{
				DateTime temp = dp_OpenDate.SelectedDate.Value;
				temp += DateTime.Now.TimeOfDay;
				((EditContinuePositionWindowsModel)DataContext).SelectedPosition.OpenDate = temp.ToString();
				tb_CloseDate.Text = DateTime.Now.ToString();
				tb_OpenDate.Background = Brushes.Yellow;
				tb_CloseDate.Background = Brushes.Yellow;
			}
		}

		private void ComboBox_Category_SelectionChanged_EditContinueWindow(object sender, SelectionChangedEventArgs e)
		{
			if (isLoaded)
			{
				tb_Category.Background = Brushes.Yellow;
				tb_Kind.Background = Brushes.Yellow;

			}
		}

		private void tb_Income_PreviewTextInput_EditContinueWindow(object sender, TextCompositionEventArgs e)
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

		private void tb_Income_PreviewKeyDown_EditContinueWindow(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				e.Handled = true;
			}
		}

		private void tb_Income_TextChanged_EditContinueWindow(object sender, TextChangedEventArgs e)
		{
			if (isLoaded)
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
				tb_Saldo.Text = SaldoCalculation();
				tb_IncomeSelectedPos.Background = Brushes.Yellow;
				tb_Saldo.Background = Brushes.Yellow;
			}
		}

		private void tb_Expense_PreviewTextInput_EditContinueWindow(object sender, TextCompositionEventArgs e)
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

		private void tb_Expense_PreviewKeyDown_EditContinueWindow(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
			{
				e.Handled = true;
			}
		}

		private void tb_Expense_TextChanged_EditContinueWindow(object sender, TextChangedEventArgs e)
		{
			if (isLoaded)
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
				tb_Saldo.Text = SaldoCalculation();
				tb_ExpenseSelectedPos.Background = Brushes.Yellow;
				tb_Saldo.Background = Brushes.Yellow;
			}
		}

		private void cb_Coin_SelectionChanged_MainWindow(object sender, SelectionChangedEventArgs e)
		{


			if (isLoaded)
			{
				if (((CoinUIModel)cb_Coin.SelectedItem) != null)
				{
					try
					{
						((EditContinuePositionWindowsModel)DataContext).SelectedPosition.Coin.Id = ((CoinUIModel)cb_Coin.SelectedItem).Id;
						((EditContinuePositionWindowsModel)DataContext).SelectedPosition.Coin.ShortName = ((CoinUIModel)cb_Coin.SelectedItem).ShortName;
						((EditContinuePositionWindowsModel)DataContext).SelectedPosition.Coin.FullName = ((CoinUIModel)cb_Coin.SelectedItem).FullName;
						((EditContinuePositionWindowsModel)DataContext).SelectedPosition.Coin.CoinNotes = ((CoinUIModel)cb_Coin.SelectedItem).CoinNotes;
						tb_Coin.Background = Brushes.Yellow;
					}
					catch (NullReferenceException ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
		}

		private void tb_Status_SelectionChanged_EditContinueWindow(object sender, SelectionChangedEventArgs e)
		{
			if (isLoaded)
			{
				if (((StatusUIModel)cb_Status.SelectedItem) != null)
				{
					try
					{
						((EditContinuePositionWindowsModel)DataContext).SelectedPosition.Status.Id = ((StatusUIModel)cb_Status.SelectedItem).Id;
						((EditContinuePositionWindowsModel)DataContext).SelectedPosition.Status.StatusName = ((StatusUIModel)cb_Status.SelectedItem).StatusName;
						((EditContinuePositionWindowsModel)DataContext).SelectedPosition.Status.StatusNotes = ((StatusUIModel)cb_Status.SelectedItem).StatusNotes;
						tb_Status.Background = Brushes.Yellow;
					}
					catch (NullReferenceException ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
		}

		private void tb_inputTag_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (isLoaded)
			{
				tb_Tag.Background = Brushes.Yellow;
			}
		}

		private void tb_inpunNtes_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (isLoaded)
			{
				tb_Notes.Background = Brushes.Yellow;
			}
		}

		private void ResetColors()
		{
			tb_PosKey.Background = Brushes.White;
			tb_OpenDate.Background = Brushes.White;
			tb_CloseDate.Background = Brushes.White;
			tb_Category.Background = Brushes.White;
			tb_Kind.Background = Brushes.White;
			tb_IncomeSelectedPos.Background = Brushes.White;
			tb_ExpenseSelectedPos.Background = Brushes.White;
			tb_Saldo.Background = Brushes.White;
			tb_Coin.Background = Brushes.White;
			tb_Status.Background = Brushes.White;
			tb_Tag.Background = Brushes.White;
			tb_Notes.Background = Brushes.White;
		}

		private string SaldoCalculation()
		{
			if (double.TryParse(tb_Income.Text, out double r1) && double.TryParse(tb_Expense.Text, out double r2))
			{
				return (r1 - r2).ToString(); ;
			}
			else
			{
				(DataContext as EditContinuePositionWindowsModel).SelectedPosition.Saldo = "Ошибка";
				return "0";
			}
		}

		private void Button_Click_Close(object sender, RoutedEventArgs e)
		{
			Close();
		}


		void GetSelectedKind()
		{
			if (cb_Kind.Items != null && (DataContext as EditContinuePositionWindowsModel).SelectedPosition.Kind != null)
			{
				for (int i = 0; i < cb_Kind.Items.Count; i++)
				{
					if (((KindUIModel)cb_Kind.Items[i]).Kind == (DataContext as EditContinuePositionWindowsModel).SelectedPosition.Kind.Kind)
					{
						cb_Kind.SelectedIndex = i;
						break;
					}
				}
			}

		}
		void GetSelectedCoin()
		{
			for (int i = 0; i < cb_Coin.Items.Count; i++)
			{
				if (((CoinUIModel)cb_Coin.Items[i]).ShortName == (DataContext as EditContinuePositionWindowsModel).SelectedPosition.Coin.ShortName)
				{
					cb_Coin.SelectedIndex = i;
					break;
				}
			}
		}
		void GetSelectedStatus()
		{
			for (int i = 0; i < cb_Status.Items.Count; i++)
			{
				if (((StatusUIModel)cb_Status.Items[i]).StatusName == (DataContext as EditContinuePositionWindowsModel).SelectedPosition.Status.StatusName)
				{
					cb_Status.SelectedIndex = i;
					break;
				}
			}
		}
	}
}
