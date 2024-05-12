using Dapper;
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

namespace MyMiniLedger.WPF.Windows.EditContinuePosition
{
	/// <summary>
	/// Interaction logic for EditContinuePositionWindow.xaml
	/// </summary>
	public partial class EditContinuePositionWindow : Window
	{
		public EditContinuePositionWindow(MainWindowModel _mainWindowModel)
		{
			InitializeComponent();
			DataContext = _mainWindowModel;
			dp_OpenDate.SelectedDate = DateTime.Now;
		}

		private void dp_OpenDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void ComboBox_Category_SelectionChanged_EditContinueWindow(object sender, SelectionChangedEventArgs e)
		{

		}

		private void cb_Kind_SelectionChanged_EditContinueWindow(object sender, SelectionChangedEventArgs e)
		{

		}
		private void cb_Kind_TextChanged_EditContinueWindow(object sender, TextChangedEventArgs e)
		{
			//if (cb_Kind.Text != "" & cb_Kind.Text != null)
			//{
			//	foreach (var item in ((MainWindowModel)DataContext).TempKinds)
			//	{
			//		if (item.Kind == cb_Kind.Text)
			//		{
			//			return;
			//		}
			//	}
			//	((MainWindowModel)DataContext).TempKinds.Clear();
			//	foreach (var item in ((MainWindowModel)DataContext).Kinds.AsList())
			//	{
			//		((MainWindowModel)DataContext).TempKinds.Add(item);

			//	}
			//}
		}

		private void tb_Income_PreviewTextInput_EditContinueWindow(object sender, TextCompositionEventArgs e)
		{

		}

		private void tb_Income_PreviewKeyDown_EditContinueWindow(object sender, KeyEventArgs e)
		{

		}

		private void tb_Income_TextChanged_EditContinueWindow(object sender, TextChangedEventArgs e)
		{

		}

		private void tb_Expense_PreviewTextInput_EditContinueWindow(object sender, TextCompositionEventArgs e)
		{

		}

		private void tb_Expense_PreviewKeyDown_EditContinueWindow(object sender, KeyEventArgs e)
		{

		}

		private void tb_Expense_TextChanged_EditContinueWindow(object sender, TextChangedEventArgs e)
		{

		}

		private void cb_Coin_SelectionChanged_MainWindow(object sender, SelectionChangedEventArgs e)
		{

		}

		private void tb_Status_SelectionChanged_EditContinueWindow(object sender, SelectionChangedEventArgs e)
		{

		}
	}
}
