using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.Windows.EditContinuePosition;
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
using MyMiniLedger.DAL.Config;
using MyMiniLedger.DAL.Services;
using System.Drawing;
using System.Globalization;
using System.Text.Json;
using Dapper;
using MyMiniLedger.WPF.ViewTools;
using Microsoft.VisualBasic;
using System.Runtime.Serialization;
using System.Text.Encodings.Web;
using static System.Windows.Forms.Design.AxImporter;
using Microsoft.VisualBasic.Devices;

namespace MyMiniLedger.WPF.Windows.PairDealCreationEdit
{
	/// <summary>
	/// Interaction logic for PairDealCreationEdit.xaml
	/// </summary>
	public partial class PairDealCreationEdit : Window
	{
		const string DealSignature = "Парная сделка_";
		int tempDealNumber = 0;

		Config configData = new Config();
		NumberFilter numFilter = new NumberFilter();
		PairDealCreationEditModel model = new PairDealCreationEditModel();
		bool courseWasInverted = false;
		bool flagLock = true;
		public PairDealCreationEdit
			(
				ObservableCollection<PositionUIModel> _mainPositionsSourse,
				ObservableCollection<CategoryUIModel> _mainCategories,
				ObservableCollection<KindUIModel> _mainKinds,
				ObservableCollection<CoinUIModel> _mainCoins,
				ObservableCollection<StatusUIModel> _mainStatuses
			)

		{
			configData = Config.GetFromConfig();
			model = new PairDealCreationEditModel()
			{
				MAINPOSITIONSCOLLECTION = _mainPositionsSourse,
				Categories = _mainCategories,
				Kinds = _mainKinds,
				Coins = _mainCoins,
				Statuses = _mainStatuses
			};
			model.SetStringCaregories(model.StringCategories, model.Categories);
			model.SetStringKinds(model.StringKinds, model.Kinds);
			model.SetStringCoins(model.WhatToBuy, model.Coins);
			model.SetStringCoins(model.WhatToSell, model.Coins);
			model.SelectedCategory = model.StringCategories[0];
			model.SelectedKind = model.Kinds.FirstOrDefault(c => c.Category.Category == model.StringCategories[0]).Kind;

			InitializeComponent();
			DataContext = model;
			comboWhatBuySell_SelectionChanged(null, null);
			RadioButton_CulcMethod_Checked(null, null);

			//NumberDealInitialization();
			model.DealsInicialization();
			if (model.ActiveDeals.Count > 0)
			tempDealNumber = model.ActiveDeals.Max(d => d.DealNumber);
			text_DealNameNumber.Text = (++tempDealNumber).ToString();

		}

		//private void NumberDealInitialization()
		//{
		//	List<PairDealModel> dealsList = new List<PairDealModel>();
		//	foreach (var position in model.MAINPOSITIONSCOLLECTION)
		//	{
		//		if (position.Tag.Contains(DealSignature))
		//		{
		//			try
		//			{
		//				string substr = position.Tag;
		//				substr = substr.Replace($"{DealSignature}", "");
		//				PairDealModel temp = JsonSerializer.Deserialize<PairDealModel>(substr);
		//				if (temp.DealNumber > tempDealNumber)
		//				{
		//					tempDealNumber = temp.DealNumber;
		//				}

		//				if ((position.Status.StatusName == "Open" || position.Status.StatusName == "Открыта") && !dealsList.Any(n => n.DealNumber == temp.DealNumber) && temp.isOpen == true)
		//				{
		//					dealsList.Add(temp);
		//				}
		//			}
		//			catch (Exception)
		//			{
		//				Console.WriteLine("Ошибка десериализации");
		//			}
		//		}
		//	}
		//	dealsList.Sort();
		//	model.ActiveDeals.Clear();
		//	foreach (var deal in dealsList)
		//	{
		//		model.ActiveDeals.Add(deal);
		//	}

		//	text_DealNameNumber.Text = (++tempDealNumber).ToString();
		//}

		//Инициализация конструкта продажи
		private string SellConstructInitialization(int _dealNumber, out DateTime dealDate, out PairDealModel tempModel, bool _staticDate = false, int? _zeroParenKey = -1, string _status_1 = "Open", string _status_2 = "Открыта")
		{

			dealDate = _staticDate == false ? DateTime.Now : model.SelectedDeal.DealOpenTime;
			int newId = model.MAINPOSITIONSCOLLECTION.Max(m => m.Id) + 1;
			int newPosKey = model.MAINPOSITIONSCOLLECTION.Max(p => p.PositionKey) + 1;

			tempModel = new PairDealModel()
			{
				DealNumber = _dealNumber,
				BuyItem = comboWhatBuy.SelectedValue.ToString(),
				SellItem = comboWhatSell.SelectedValue.ToString(),
				DealOpenTime = dealDate,
				//Для корректной записи данных по сделке нужно передавать номер головы комплексной позиции
				ParentZeroKey = _zeroParenKey == -1 ? newPosKey : _zeroParenKey
			};
			JsonSerializerOptions options = new JsonSerializerOptions
			{
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			};
			string posData = DealSignature + JsonSerializer.Serialize(tempModel, options);

			model.SelectedPosition.Id = newId;
			model.SelectedPosition.PositionKey = newPosKey;
			model.SelectedPosition.OpenDate = dealDate.ToString(CultureInfo.CurrentUICulture);
			model.SelectedPosition.Kind = model.Kinds.FirstOrDefault(k => k.Kind == model.DefaultKindToSell);
			model.SelectedPosition.Income = 0.ToString(CultureInfo.CurrentUICulture);
			model.SelectedPosition.Expense = tb_HowManySell.Text;
			model.SelectedPosition.Coin = model.Coins.FirstOrDefault(c => c.ShortName == comboWhatSell.SelectedValue);
			model.SelectedPosition.Status = model.Statuses.FirstOrDefault(s => s.StatusName == _status_1 || s.StatusName == _status_2);
			model.SelectedPosition.Tag = posData;
			model.SelectedPosition.Notes = $"Парная позиция №{tempModel.DealNumber} Продажа {tempModel.SellItem} Покупка {tempModel.BuyItem} Позиция создана в редакторе позиций";
			return posData;
		}

		//Инициализация конструкта покупки
		private void BuyConstructInitialization(DateTime _dealDate, string _posData, string _status_1 = "Open", string _status_2 = "Открыта")
		{
			model.SelectedPosition.OpenDate = _dealDate.ToString(CultureInfo.CurrentUICulture);
			model.SelectedPosition.Kind = model.Kinds.FirstOrDefault(k => k.Kind == model.DefaultKindToBuy);
			model.SelectedPosition.Income = tb_HowManyBuy.Text;
			model.SelectedPosition.Expense = 0.ToString(CultureInfo.CurrentUICulture);
			model.SelectedPosition.Coin = model.Coins.FirstOrDefault(c => c.ShortName == comboWhatBuy.SelectedValue);
			model.SelectedPosition.Status = model.Statuses.FirstOrDefault(s => s.StatusName == _status_1 || s.StatusName == _status_2);
			model.SelectedPosition.Tag = _posData;
			model.SelectedPosition.Notes = $"Позиция создана в редакторе позиций";
		}


		//Создание новой сделки
		private void bt_DoNewDeal_Click(object sender, RoutedEventArgs e)
		{
			model.SelectedPosition = new PositionUIModel();
			string posData = SellConstructInitialization(tempDealNumber, out DateTime dealDate, out PairDealModel tempModel);
			//Продажа
			model.InsertPosition.Execute(null);
			BuyConstructInitialization(dealDate, posData);
			//Поркупка
			model.AddComplexPosition.Execute(null);

			model.ActiveDeals.Add(tempModel);
			text_DealNameNumber.Text = (++tempDealNumber).ToString();
			model.SelectedDeal = tempModel;
		}

		//Продолжение существующей сделки
		private void bt_ContinueOpenDeal_Click(object sender, RoutedEventArgs e)
		{
			string posData = SellConstructInitialization(model.SelectedDeal.DealNumber, out DateTime dealDate, out PairDealModel tempModel, true, model.SelectedDeal.ParentZeroKey);
			//Продажа
			model.SelectedPosition.ZeroParrentKey = model.SelectedDeal.ParentZeroKey;
			model.SelectedPositionsInitialization(model.SelectedPositions);
			model.AddComplexPosition.Execute(null);

			//Покупка
			BuyConstructInitialization(dealDate, posData);
			model.AddComplexPosition.Execute(null);

		}

		//Создание новой сделки и её закрытие
		private void bt_DoNewDealAndClose_Click(object sender, RoutedEventArgs e)
		{
			model.SelectedPosition = new PositionUIModel();
			string posData = SellConstructInitialization(tempDealNumber, out DateTime dealDate, out PairDealModel tempModel);
			//Продажа
			model.InsertPosition.Execute(null);
			//Покупка
			BuyConstructInitialization(dealDate, posData, "Closed", "Закрыта");
			model.SelectedPosition.CloseDate = dealDate.ToString(CultureInfo.CurrentUICulture);
			model.SelectedPosition.ZeroParrentKey = tempModel.ParentZeroKey;
			model.AddComplexPosition.Execute(null);

			text_DealNameNumber.Text = (++tempDealNumber).ToString();
			model.ReWriteDeals(tempModel, DealSignature);
		}

		//Продолжение существующей сделки и её закрытие
		private void bt_ContinueDealAndClose_Click(object sender, RoutedEventArgs e)
		{
			string posData = SellConstructInitialization(model.SelectedDeal.DealNumber, out DateTime dealDate, out PairDealModel tempModel, true, model.SelectedDeal.ParentZeroKey);
			//Продажа
			model.SelectedPosition.ZeroParrentKey = model.SelectedDeal.ParentZeroKey;
			model.SelectedPositionsInitialization(model.SelectedPositions);
			model.AddComplexPosition.Execute(null);

			//Покупка
			BuyConstructInitialization(dealDate, posData, "Closed", "Закрыта");
			model.SelectedPosition.CloseDate = dealDate.ToString(CultureInfo.CurrentUICulture);
			model.SelectedPosition.ZeroParrentKey = model.SelectedDeal.ParentZeroKey;
			model.AddComplexPosition.Execute(null);

			model.ActiveDeals.RemoveAt(model.ActiveDeals.IndexOf(model.ActiveDeals.FirstOrDefault(i => i.DealNumber == model.SelectedDeal.DealNumber)));
			model.ReWriteDeals(tempModel, DealSignature);
		}

		private void RadioButton_CulcMethod_Checked(object sender, RoutedEventArgs e)
		{
			if (tb_HowManyBuy != null && tb_DefaultFee != null && tb_CalculatedCourse != null)
			{
				if (rb_valueCalcMethod.IsChecked == true)
				{
					tb_HowManyBuy.IsReadOnly = false;
					rb_toBuy.IsEnabled = false;
					rb_toSell.IsEnabled = false;
					tb_DefaultFee.IsEnabled = false;
					tb_CalculatedCourse.IsEnabled = false;
					checBox_CourseInverter.IsEnabled = false;
				}
				else
				{
					tb_HowManyBuy.IsReadOnly = true;
					rb_toBuy.IsEnabled = true;
					rb_toSell.IsEnabled = true;
					tb_DefaultFee.IsEnabled = true;
					tb_CalculatedCourse.IsEnabled = true;
					checBox_CourseInverter.IsEnabled = true;
				}
			}
		}

		private void tb_DefaultFee_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			numFilter.textBoxPreviewTextInputFilter(sender, e);
		}
		private void tb_DefaultFee_TextChanged(object sender, TextChangedEventArgs e)
		{
			numFilter.textBoxTextChangedFilter(sender, e);
			if (rb_toBuy != null)
			{
				if (rb_toBuy.IsChecked == true)
				{
					model.CurrentBuyFee = tb_DefaultFee.Text;
				}
				else
				{
					model.CurrentSellFee = tb_DefaultFee.Text;
				}
				tb_CalculatedCourse_TextChanged(sender, e);
			}
		}

		private void tb_CalculatedCourse_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			numFilter.textBoxPreviewTextInputFilter(sender, e);
		}
		private void tb_CalculatedCourse_TextChanged(object sender, TextChangedEventArgs e)
		{
			numFilter.textBoxTextChangedFilter(sender, e);
			if (((TextBox)sender).IsFocused && flagLock == true)
			{			
				HowManyBuyCalculate();
			}
		}
		private void HowManyBuyCalculate()
		{
			CultureInfo currCulture = CultureInfo.CurrentCulture;

			bool canCalculate = Double.TryParse(tb_CalculatedCourse.Text, NumberStyles.Float, currCulture, out double course);
			bool canBuyCount = Double.TryParse(tb_HowManyBuy.Text, NumberStyles.Float, currCulture, out double buyCount);

			if (canCalculate && canBuyCount)
			{
				if (checBox_CourseInverter.IsChecked == true)
				{
					course = 1 / course;
					if (course == double.PositiveInfinity) { course = 0; }

					courseWasInverted = true;
					
					flagLock = false;
					var reversCourse = 1/course;
					if (reversCourse == double.PositiveInfinity) { reversCourse = 0; }

					tb_CalculatedCourse.Text = reversCourse.ToString(CultureInfo.CurrentUICulture);
					flagLock = true;
					
				}
				if (checBox_CourseInverter.IsChecked == false && courseWasInverted)
				{
					course = 1 / course;
					courseWasInverted = false;
					tb_CalculatedCourse.Text = course.ToString(CultureInfo.CurrentUICulture);
				}

				double sellCount = Double.Parse(tb_HowManySell.Text, NumberStyles.Float, currCulture);
				double feeTobuy = Double.Parse(tb_BuyFee.Text, NumberStyles.Float, currCulture);
				double feeTosell = Double.Parse(tb_SellFee.Text, NumberStyles.Float, currCulture);
				//Если комисси я берется с покупки - для целевой сумм требуется докинуть на продажу
				if (rb_toBuy.IsChecked == true)
				{
					buyCount = course * sellCount;
					var totalFee = buyCount * feeTobuy / 100;
					buyCount = buyCount - totalFee;
					//Console.WriteLine(buyCount);
					tb_HowManyBuy.Text = buyCount.ToString(CultureInfo.CurrentUICulture);
				}
				//Если комиссия берется с продажи - конечная сумма покупки будет меньше
				else
				{
					sellCount = sellCount - (feeTosell * sellCount / 100);
					buyCount = sellCount * course;
					tb_HowManyBuy.Text = buyCount.ToString(CultureInfo.CurrentUICulture);
				}
			}
		}

		private void tb_HowManyBuy_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			numFilter.textBoxPreviewTextInputFilter(sender, e);
		}


		private void tb_HowManyBuy_TextChanged(object sender, TextChangedEventArgs e)
		{
			numFilter.textBoxTextChangedFilter(sender, e);
			if (rb_valueCalcMethod.IsChecked == true)
			{
				comboWhatBuySell_SelectionChanged(null, null);
			}

		}

		private void tb_HowManySell_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			numFilter.textBoxPreviewTextInputFilter(sender, e);
		}
		private void tb_HowManySell_TextChanged(object sender, TextChangedEventArgs e)
		{
			numFilter.textBoxTextChangedFilter(sender, e);
			if (rb_valueCalcMethod.IsChecked == true)
			{
				comboWhatBuySell_SelectionChanged(null, null);
			}
			if (rb_courseCalcMethod.IsChecked == true)
			{
				tb_CalculatedCourse_TextChanged(sender, e);
			}


		}

		private void comboWhatBuySell_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DifferentPairControl();

			if (comboWhatBuy != null & comboWhatSell != null)
			{
				if (comboWhatBuy.SelectedValue != null && comboWhatSell.SelectedValue != null && !tb_CalculatedCourse.IsFocused && !tb_DefaultFee.IsFocused)
				{
					text_PairName.Text = $"{comboWhatBuy.SelectedValue}/{comboWhatSell.SelectedValue}";
					tb_CalculatedCourse.Text = "none";
					if (Double.Parse(tb_HowManySell.Text, CultureInfo.InvariantCulture) != 0)
					{
						tb_CalculatedCourse.Text = (Double.Parse(tb_HowManyBuy.Text, CultureInfo.InvariantCulture) / Double.Parse(tb_HowManySell.Text, CultureInfo.InvariantCulture)).ToString("N6", CultureInfo.CurrentUICulture);
					}
				}
				if (comboWhatBuy.SelectedValue != null && comboWhatSell.SelectedValue != null)
				{
					model.BuyItemName = comboWhatBuy.SelectedValue.ToString();
					model.SellItemName = comboWhatSell.SelectedValue.ToString();
				}
			}
		}

		private void DifferentPairControl()
		{
			if (comboWhatBuy != null & comboWhatSell != null)
			{
				if (comboWhatSell.SelectedValue == comboWhatBuy.SelectedValue && comboWhatBuy.SelectedIndex != comboWhatBuy.Items.Count - 1)
				{
					comboWhatBuy.SelectedIndex = comboWhatBuy.SelectedIndex + 1;
				}
				else if (comboWhatSell.SelectedValue == comboWhatBuy.SelectedValue && comboWhatBuy.SelectedIndex != 0 && comboWhatBuy.SelectedIndex != -1)
				{
					comboWhatBuy.SelectedIndex = comboWhatBuy.SelectedIndex - 1;
				}
			}
		}

		private void checBox_CourseInverter_Click(object sender, RoutedEventArgs e)
		{
			HowManyBuyCalculate();
		}


		//Выбор сделки
		private void Row_DealDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			string dealNumber = (dg_ActiveDeals.Columns[0].GetCellContent(row) as TextBlock).Text;
			string buyItem = (dg_ActiveDeals.Columns[1].GetCellContent(row) as TextBlock).Text;
			string sellItem = (dg_ActiveDeals.Columns[2].GetCellContent(row) as TextBlock).Text;
			string date = (dg_ActiveDeals.Columns[3].GetCellContent(row) as TextBlock).Text.ToString();

			model.SelectedDeal = model.ActiveDeals.FirstOrDefault(d => d.DealNumber.ToString() == dealNumber);
			text_DealNameNumber.Text = model.SelectedDeal.DealNumber.ToString();

			comboWhatSell.SelectedValue = model.SelectedDeal.SellItem;
			comboWhatBuy.SelectedValue = model.SelectedDeal.BuyItem;

			//foreach (var item in model.MAINPOSITIONSCOLLECTION)
			//{
			//	if (item.Tag.Contains(dealNumber) && item.Tag.Contains(buyItem) && item.Tag.Contains(sellItem))
			//	{
			//		string temp = item.Tag;
			//		temp = temp.Replace(DealSignature, "");
			//		var deal = JsonSerializer.Deserialize<PairDealModel>(temp);
			//		if (deal.DealOpenTime.ToString("dd.MM.yyyy HH:mm:ss") == date)
			//		{
			//			model.SelectedDeal = deal;
			//			text_DealNameNumber.Text = deal.DealNumber.ToString();
			//		}
			//	}
			//}
		}

	}
}
