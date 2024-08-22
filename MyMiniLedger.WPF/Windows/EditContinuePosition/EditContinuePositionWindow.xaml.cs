﻿using Dapper;
using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.ViewTools;
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
		NumberFilter numFilter = new NumberFilter();
		public EditContinuePositionWindow(PositionUIModel _selectedPosition,
			ObservableCollection<PositionUIModel> _mainPositionsSourse,
			ObservableCollection<CategoryUIModel> _mainCategories,
			ObservableCollection<KindUIModel> _mainKinds,
			ObservableCollection<CoinUIModel> _mainCoins,
			ObservableCollection<StatusUIModel> _mainStatuses)

		{
			EditContinuePositionWindowsModel model = new EditContinuePositionWindowsModel()
			{
				MAINPOSITIONSCOLLECTION = _mainPositionsSourse,
				Categories = _mainCategories,
				Kinds = _mainKinds,
				Coins = _mainCoins,
				Statuses = _mainStatuses
			};
			model.SelectedPosition = (PositionUIModel)_selectedPosition.Clone();
			model.DublecateSelectedPosition = (PositionUIModel)_selectedPosition.Clone();

			model.SetStringCaregories(model.StringCategories, model.Categories);
			model.SetStringKinds(model.StringKinds, model.Kinds, model.SelectedPosition.Kind.Category);
			model.SelectedPositionsInitialization(model.SelectedPositions);
			model.SetStringCoins(model.StringCoins, model.Coins);
			model.SetStringStatusesAndTranslation(model.Statuses, model.StringStatuses);
			model.SelectedOpenDate = model.SelectedPosition.OpenDate;
			model.SelectedCloseDate = model.SelectedPosition.CloseDate;
			model.SelectedCategory = model.SelectedPosition.Kind.Category.Category;
			model.SelectedKind = model.SelectedPosition.Kind.Kind;
			model.SelectedCoin = model.SelectedPosition.Coin.ShortName;
			model.SelectedStatus = model.SelectedPosition.Status.StatusName;
			InitializeComponent();
			DataContext = model;
			GetBalancesInUI();

			(DataContext as EditContinuePositionWindowsModel).UpdateEvent += ResetColors;
			(DataContext as EditContinuePositionWindowsModel).UpdateEvent += GetBalancesInUI;
			isLoaded = true;
		}


		//Работа с заполнением полей income
		private void tb_Income_PreviewTextInput_EditContinueWindow(object sender, TextCompositionEventArgs e)
		{
			numFilter.textBoxPreviewTextInputFilter(sender, e);
		}
		private void tb_Income_TextChanged_EditContinueWindow(object sender, TextChangedEventArgs e)
		{
			numFilter.textBoxTextChangedFilter(sender, e);
		}
		////Работа с заполнением полей expense
		private void tb_Expense_PreviewTextInput_EditContinueWindow(object sender, TextCompositionEventArgs e)
		{
			numFilter.textBoxPreviewTextInputFilter(sender, e);
		}
		private void tb_Expense_TextChanged_EditContinueWindow(object sender, TextChangedEventArgs e)
		{
			numFilter.textBoxTextChangedFilter(sender, e);
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

		//private string SaldoCalculation()
		//{
		//	if (double.TryParse(tb_Income.Text, out double r1) && double.TryParse(tb_Expense.Text, out double r2))
		//	{
		//		return (r1 - r2).ToString(); ;
		//	}
		//	else
		//	{
		//		(DataContext as EditContinuePositionWindowsModel).SelectedPosition.Saldo = "Ошибка";
		//		return "0";
		//	}
		//}

		private void GetBalancesInUI()
		{
			sp_BalanceInfo.Children.Clear();
			
			if (DataContext != null)
			{
				ComplexPositionBalanceCalculator calculator = new ComplexPositionBalanceCalculator();
				(DataContext as EditContinuePositionWindowsModel).TotalBalances = calculator.GetTotalBalances((DataContext as EditContinuePositionWindowsModel).SelectedPositions);

				foreach (var balance in (DataContext as EditContinuePositionWindowsModel).TotalBalances)
				{

					StackPanel temp_sp = new StackPanel();
					temp_sp.Orientation = Orientation.Horizontal;
					TextBlock textBloc = new TextBlock { Text = $"Баланс: {balance.CoinName}\t" };
					temp_sp.Children.Add(textBloc);
					TextBlock textBlockBalance = new TextBlock { Text = $"{balance.Balance} {balance.CoinName}" };
					textBlockBalance.FontWeight = FontWeights.Bold;
					if (balance.Balance <= 0)
					{
						textBlockBalance.Foreground = Brushes.Red;
					}
					else
					{
						textBlockBalance.Foreground = Brushes.Green;
					}
					temp_sp.Children.Add(textBlockBalance);
					TextBlock averagePriceText = new TextBlock {Text = $"\tСредняя отношение сделки: {balance.AveragePrice}" };
					temp_sp.Children.Add(averagePriceText);
                    sp_BalanceInfo.Children.Add(temp_sp);
                    TextBlock course = new TextBlock() {Text = $"Текущий курс: {balance.CurrentCourseToUsd}" };
					sp_BalanceInfo.Children.Add(course);
				}
			}
				
		}

		private void Button_Click_Close(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
