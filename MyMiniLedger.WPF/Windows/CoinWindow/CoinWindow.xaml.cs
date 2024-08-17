using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.WindowsModels;
using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace MyMiniLedger.WPF.Windows.CoinWindow
{
	/// <summary>
	/// Interaction logic for CoinWindow.xaml
	/// </summary>
	public partial class CoinWindow : Window
	{
		MainWindowModel MainWindow;
		public CoinWindow(MainWindowModel _mainWindowModel)
		{
			InitializeComponent();
			MainWindow = _mainWindowModel;
			(DataContext as CoinWindowModel).UpdateCoinsEvent += MainWindow.UpdateCoins;
		}

		private void CoinsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (((CoinUIModel)CoinsList.SelectedItem) != null)
			{
				rb_Other.IsChecked = true;
				tb_EditCoinName.Text = ((CoinUIModel)CoinsList.SelectedItem).ShortName;
				tb_EditCoinFullName.Text = ((CoinUIModel)CoinsList.SelectedItem).FullName;
				tb_EditCoinNotes.Text = ((CoinUIModel)CoinsList.SelectedItem).CoinNotes;
				(DataContext as CoinWindowModel).SelectedCoin.Id = ((CoinUIModel)CoinsList.SelectedItem).Id;
				(DataContext as CoinWindowModel).SelectedCoin.RefNumber = ((CoinUIModel)CoinsList.SelectedItem).RefNumber;
			}
		}

		private void ButtonExit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void rb_Fiat_Checked(object sender, RoutedEventArgs e)
		{
			if (tb_EditCoinNotes.Text.Contains("crypto"))
			{
				tb_EditCoinNotes.Text = tb_EditCoinNotes.Text.Replace("crypto", "fiat");
			}
			else if (!tb_EditCoinNotes.Text.Contains("fiat"))
			{
				tb_EditCoinNotes.Text = tb_EditCoinNotes.Text.Insert(0, "fiat");
			}
		}

		private void rb_Crypto_Checked(object sender, RoutedEventArgs e)
		{
			if (tb_EditCoinNotes.Text.Contains("fiat"))
			{
				tb_EditCoinNotes.Text = tb_EditCoinNotes.Text.Replace("fiat", "crypto");
			}
			else if(!tb_EditCoinNotes.Text.Contains("crypto"))
			{
				tb_EditCoinNotes.Text = tb_EditCoinNotes.Text.Insert(0, "crypto");
			}
		}

		private void rb_Other_Checked(object sender, RoutedEventArgs e)
		{
			if (tb_EditCoinNotes.Text.Contains("fiat"))
			{
				tb_EditCoinNotes.Text = tb_EditCoinNotes.Text.Replace("fiat", "");
			}
			if (tb_EditCoinNotes.Text.Contains("crypto"))
			{
				tb_EditCoinNotes.Text = tb_EditCoinNotes.Text.Replace("crypto", "");
			}
			else
			{
				tb_EditCoinNotes.Text = tb_EditCoinNotes.Text.Insert(0, "");
			}
		}


		////Проблема - данные о новой монете добовляются до ее обновления - нет реального Id из бд 
		//private void bt_AddNewCoin_Click(object sender, RoutedEventArgs e)
		//{
		//	if (((CoinWindowModel)DataContext).TempCoin != null)
		//	{
		//		MainWindow.Coins.Add(((CoinWindowModel)DataContext).TempCoin);
		//	}
		//}

		//private void bt_DelCoin_Click(object sender, RoutedEventArgs e)
		//{
		//	if (((CoinWindowModel)DataContext).SelectedCoin != null)
		//	{
		//		//MainWindow.Coins.Remove(((CoinWindowModel)DataContext).SelectedCoin);
		//		MainWindow.Coins = MainWindow.CoinsInicialization();
		//	}
		//}



	}
}
