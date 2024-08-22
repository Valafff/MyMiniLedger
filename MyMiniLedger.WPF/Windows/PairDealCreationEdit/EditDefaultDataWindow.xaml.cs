using MyMiniLedger.WPF.Windows.PairDealCreationEdit;
using MyMiniLedger.WPF.Models;
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
using MyMiniLedger.DAL.Config;
using System.IO;
using System.Text.Json;
using System.Globalization;
using MyMiniLedger.WPF.WindowsModels;
using MyMiniLedger.WPF.ViewTools;

namespace MyMiniLedger.WPF.Windows.PairDealCreationEdit
{
	/// <summary>
	/// Interaction logic for EditDefaultData.xaml
	/// </summary>
	public partial class EditDefaultDataWindow : Window
	{

		Config configData;
		PairDealCreationEditModel Model;
		NumberFilter numFilter = new NumberFilter();
		public EditDefaultDataWindow(ref Config _config, PairDealCreationEditModel _model)
		{
			configData = _config;
			Model = _model;
			InitializeComponent();
			Title = "Редактор значений по умолчанию";

			cb_CategoryToBuy.ItemsSource = Model.StringCategories;
			cb_CategoryToBuy.SelectedIndex = 0;
			cb_CategoryToSell.ItemsSource = Model.StringCategories;
			cb_CategoryToSell.SelectedIndex = 0;

			cb_CategoryToBuy_SelectionChanged(cb_CategoryToBuy, null);
			cb_CategoryToSell_SelectionChanged(cb_CategoryToSell, null);
			textBox_DefaultFee_TextChanged(textBox_DefaultFee, null);
		}

		private void cb_CategoryToBuy_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			List<string> temp = new List<string>();
			foreach (var item in Model.Kinds)
			{
				if (cb_CategoryToBuy.SelectedValue.ToString() == item.Category.Category)
				{
					temp.Add(item.Kind);
				}
			}
			cb_comboKindDataToBuy.ItemsSource = temp;
			cb_comboKindDataToBuy.SelectedIndex = 0;
		}

		private void cb_CategoryToSell_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			List<string> temp = new List<string>();
			foreach (var item in Model.Kinds)
			{
				if (cb_CategoryToSell.SelectedValue.ToString() == item.Category.Category)
				{
					temp.Add(item.Kind);
				}
			}
			cb_comboKindDataToSell.ItemsSource = temp;
			cb_comboKindDataToSell.SelectedIndex = 0;
		}

		private void bt_SaveData_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				//Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
				configData.DefaultFee = Double.Parse(textBox_DefaultFee.Text, CultureInfo.CurrentCulture);
				configData.DefaultKindToBuy = cb_comboKindDataToBuy.SelectedValue.ToString();
				configData.DefaultKindToSell = cb_comboKindDataToSell.SelectedValue.ToString();

				Model.DefaultFeel = configData.DefaultFee.ToString(CultureInfo.CurrentUICulture);
				Model.DefaultKindToBuy = configData.DefaultKindToBuy;
				Model.DefaultKindToSell = configData.DefaultKindToSell;

				using var toFile = new FileStream("config.json", FileMode.Truncate, FileAccess.Write);
				JsonSerializer.Serialize(toFile, configData);
				MessageBox.Show("Настройки сохранены.");
			}
			catch (Exception)
			{
				MessageBox.Show("Настройки не сохранены!", "Ошибка записи", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void textBox_DefaultFee_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			numFilter.textBoxPreviewTextInputFilter(sender, e);
		}

		private void textBox_DefaultFee_TextChanged(object sender, TextChangedEventArgs e)
		{
			numFilter.textBoxTextChangedFilter(sender, e);
		}
	}
}
