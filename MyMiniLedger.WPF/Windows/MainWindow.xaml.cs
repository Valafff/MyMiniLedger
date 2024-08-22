using Dapper;
using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.ViewTools;
using MyMiniLedger.WPF.Windows;
using MyMiniLedger.WPF.Windows.EditContinuePosition;
using MyMiniLedger.WPF.WindowsModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using MyMiniLedger.DAL.Config;
using System.IO;
using System.Text.Json;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Globalization;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MyMiniLedger.BLL.Models;
using System.Windows.Data;


namespace MyMiniLedger.WPF
{
	public partial class MainWindow : Window
	{
		public bool Valid = false;
		NumberFilter numFilter = new NumberFilter();
		public MainWindow()
		{
			InitializeComponent();
			StringBuilder pass = new StringBuilder();
			MainPasswordWindow passForm = new MainPasswordWindow(ref pass, this);
			passForm.ShowDialog();
			if (!Valid)
			{
				Close();
			}
			else
			{
				MainWindowModel MainModel = new MainWindowModel(pass.ToString());
				DataContext = MainModel;
				datePiker_Start.SelectedDate = new DateTime(DateTime.Now.Year, 1, 1);
				datePiker_End.SelectedDate = DateTime.Now;
				dp_OpenDate.SelectedDate = DateTime.Now;

				(DataContext as MainWindowModel).UpdateCoinsIndexEvent += UpdateSelectedCoin;
				(DataContext as MainWindowModel).UpdateCategoriesIndexEvent += UpdateSelectedCategory;
				(DataContext as MainWindowModel).UpdateKindsIndexEvent += UpdateSelectedKind;
				(DataContext as MainWindowModel).UpdateDatePickerEvent += UpdateDatePicker;
			}

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
			//При обновлении позиций UpdatePositionsCollection в модели, старые позиции очищаются. Выполняется проверка, чтобы не нарваться на null
			if (((MainWindowModel)DataContext).Positions != null && ((MainWindowModel)DataContext).Positions.Count() != 0)
			{
				((MainWindowModel)DataContext).SelectedPosition = (PositionUIModel)(((DataGrid)sender).CurrentItem);

				ComplexPositionBalanceCalculator calculator = new ComplexPositionBalanceCalculator();
				var selectedBalance = calculator.GetTotalBalance((DataContext as MainWindowModel).Positions, ((MainWindowModel)DataContext).SelectedPosition);

				if (selectedBalance == null)
				{
					selectedBalance = new TotalBalance() { Balance = 0, CoinName = "[Валюта]" };
				}
				if (selectedBalance.Cointype.Contains("crypto"))
				{
					tb_CurrentIncome.Text = selectedBalance.TotalIncome.ToString("N10", new CultureInfo("ru-RU"));
					tb_CurrentIncome.Foreground = Brushes.Green;
					tb_CurrentExpence.Text = selectedBalance.TotalExpense.ToString("N10", new CultureInfo("ru-RU"));
					tb_CurrentExpence.Foreground = Brushes.Red;
					tb_CurrentBalance.Text = selectedBalance.Balance.ToString("N10", new CultureInfo("ru-RU"));

					tb_IncomeInfo.Text = Double.Parse(tb_IncomeInfo.Text).ToString("N10", new CultureInfo("ru-RU"));
					tb_ExpenseInfo.Text = Double.Parse(tb_ExpenseInfo.Text).ToString("N10", new CultureInfo("ru-RU"));
					tb_SaldoInfo.Text = Double.Parse(tb_SaldoInfo.Text).ToString("N10", new CultureInfo("ru-RU"));
				}
				else
				{
					tb_CurrentIncome.Text = selectedBalance.TotalIncome.ToString("N2", new CultureInfo("ru-RU"));
					tb_CurrentIncome.Foreground = Brushes.Green;
					tb_CurrentExpence.Text = selectedBalance.TotalExpense.ToString("N2", new CultureInfo("ru-RU"));
					tb_CurrentExpence.Foreground = Brushes.Red;
					tb_CurrentBalance.Text = selectedBalance.Balance.ToString("N2", new CultureInfo("ru-RU"));

					tb_IncomeInfo.Text = Double.Parse(tb_IncomeInfo.Text).ToString("N2", new CultureInfo("ru-RU"));
					tb_ExpenseInfo.Text = Double.Parse(tb_ExpenseInfo.Text).ToString("N2", new CultureInfo("ru-RU"));
					tb_SaldoInfo.Text = Double.Parse(tb_SaldoInfo.Text).ToString("N2", new CultureInfo("ru-RU"));
				}

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
					tb_CurrentBalance.Foreground = Brushes.Black;
				}
				tb_CurrentCoin.Text = selectedBalance.CoinName;
			}

			//Не позволяет DatePicker сползти в некорректный формат
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CurrentUICulture;

		}
		private void dp_OpenDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender != null)
			{
				((MainWindowModel)DataContext).PositionConstruct.OpenDate = sender.ToString();
			}
		}

		//Работа с заполнением полей income
		private void tb_Income_PreviewTextInput_MainWindow(object sender, TextCompositionEventArgs e)
		{
			numFilter.textBoxPreviewTextInputFilter(sender, e);
		}
		private void tb_Income_TextChanged_MainWindow(object sender, TextChangedEventArgs e)
		{
			numFilter.textBoxTextChangedFilter(sender, e);
		}
		////Работа с заполнением полей expense
		private void tb_Expense_PreviewTextInput_MainWindow(object sender, TextCompositionEventArgs e)
		{
			numFilter.textBoxPreviewTextInputFilter(sender, e);
		}
		private void tb_Expense_TextChanged_MainWindow(object sender, TextChangedEventArgs e)
		{
			numFilter.textBoxTextChangedFilter(sender, e);
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

		private void MenuItem_Click_DB_Path_editing(object sender, RoutedEventArgs e)
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

		private void MenuItem_Click_newBackup(object sender, RoutedEventArgs e)
		{
			try
			{
				string sourcePath = Config.GetFromConfig().DataSource;
				string targetPath = string.Empty;
				var dialog = new CommonOpenFileDialog();
				dialog.IsFolderPicker = true;
				CommonFileDialogResult result = dialog.ShowDialog();
				if (result == CommonFileDialogResult.Ok)
				{
					targetPath = dialog.FileName + $"\\MyMiniLedger_backup_{DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss")}.db";
					File.Copy(sourcePath, targetPath);
					MessageBox.Show("Резервная копия создана", "Резервная копия", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception)
			{
				Console.WriteLine("Не удалось создать резервную копию БД");
			}
		}

		//Корректировка сортировки т.к. основная часть полей datagrid - string
		private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
		{
			e.Handled = true;
			if (e.Column.Header.ToString() == "Номер поз.")
			{
				if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.PositionKey ascending
						select item);
					e.Column.SortDirection = ListSortDirection.Ascending;
				}
				else
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.PositionKey descending
						select item);
					e.Column.SortDirection = ListSortDirection.Descending;
				}
			}

			if (e.Column.Header.ToString() == "Дата откр. поз.")
			{
				if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby DateTime.Parse(item.OpenDate) ascending
						select item);
					e.Column.SortDirection = ListSortDirection.Ascending;
				}
				else
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby DateTime.Parse(item.OpenDate) descending
						select item);
					e.Column.SortDirection = ListSortDirection.Descending;
				}
			}

			if (e.Column.Header.ToString() == "Дата закр. поз.") //ToDo Сделать корректную сортировку с пустыми полями
			{
				if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.CloseDate ascending
						select item);
					e.Column.SortDirection = ListSortDirection.Ascending;
				}
				else
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.CloseDate descending
						select item);
					e.Column.SortDirection = ListSortDirection.Descending;
				}
			}

			if (e.Column.Header.ToString() == "Категория")
			{
				if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.Kind.Category.Category ascending
						select item);
					e.Column.SortDirection = ListSortDirection.Ascending;
				}
				else
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.Kind.Category.Category descending
						select item);
					e.Column.SortDirection = ListSortDirection.Descending;
				}
			}

			if (e.Column.Header.ToString() == "Вид")
			{
				if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.Kind.Kind ascending
						select item);
					e.Column.SortDirection = ListSortDirection.Ascending;
				}
				else
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.Kind.Kind descending
						select item);
					e.Column.SortDirection = ListSortDirection.Descending;
				}
			}

			if (e.Column.Header.ToString() == "Приход")
			{
				if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby Double.Parse(item.Income) ascending
						select item);
					e.Column.SortDirection = ListSortDirection.Ascending;
				}
				else
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby Double.Parse(item.Income) descending
						select item);
					e.Column.SortDirection = ListSortDirection.Descending;
				}
			}

			if (e.Column.Header.ToString() == "Расход")
			{
				if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby Double.Parse(item.Expense) ascending
						select item);
					e.Column.SortDirection = ListSortDirection.Ascending;
				}
				else
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby Double.Parse(item.Expense) descending
						select item);
					e.Column.SortDirection = ListSortDirection.Descending;
				}
			}

			if (e.Column.Header.ToString() == "Сальдо")
			{
				if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby Double.Parse(item.Saldo) ascending
						select item);
					e.Column.SortDirection = ListSortDirection.Ascending;
				}
				else
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby Double.Parse(item.Saldo) descending
						select item);
					e.Column.SortDirection = ListSortDirection.Descending;
				}
			}

			if (e.Column.Header.ToString() == "Валюта")
			{
				if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.Coin.ShortName ascending
						select item);
					e.Column.SortDirection = ListSortDirection.Ascending;
				}
				else
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.Coin.ShortName descending
						select item);
					e.Column.SortDirection = ListSortDirection.Descending;
				}
			}

			if (e.Column.Header.ToString() == "Статус")
			{
				if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.Status.StatusName ascending
						select item);
					e.Column.SortDirection = ListSortDirection.Ascending;
				}
				else
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.Status.StatusName descending
						select item);
					e.Column.SortDirection = ListSortDirection.Descending;
				}
			}

			if (e.Column.Header.ToString() == "Тэг")
			{
				if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.Tag ascending
						select item);
					e.Column.SortDirection = ListSortDirection.Ascending;
				}
				else
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.Tag descending
						select item);
					e.Column.SortDirection = ListSortDirection.Descending;
				}
			}

			if (e.Column.Header.ToString() == "Примечания")
			{
				if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.Notes ascending
						select item);
					e.Column.SortDirection = ListSortDirection.Ascending;
				}
				else
				{
					mainDataGrid.ItemsSource = new ObservableCollection<PositionUIModel>(
						from item in (DataContext as MainWindowModel).Positions
						orderby item.Notes descending
						select item);
					e.Column.SortDirection = ListSortDirection.Descending;
				}
			}
		}

		private void TabItem_GotFocus(object sender, RoutedEventArgs e)
		{
			ComplexPositionBalanceCalculator calculator = new ComplexPositionBalanceCalculator();
			ObservableCollection<TotalBalance> fiatBalances = new ObservableCollection<TotalBalance>();
			ObservableCollection<TotalBalance> cryptoBalances = new ObservableCollection<TotalBalance>();
			foreach (var item in (DataContext as MainWindowModel).Coins)
			{
				var t = calculator.GetTotalBalanceByCoin((DataContext as MainWindowModel).Positions, item.ShortName, item.CoinNotes);
				if (!t.Cointype.Contains("crypto"))
				{
					fiatBalances.Add(t);
				}
				else
				{
					cryptoBalances.Add(t);
				}

			}
			//Создание datagrid в code-behind
			//var summaryDataGrid = new DataGrid()
			//{
			//	Columns = { new DataGridTextColumn() { Header = "Монета/Валюта", Binding = new Binding("CoinName") },
			//			new DataGridTextColumn() { Header = "Суммарный приход", Binding = new Binding("TotalIncome")},
			//			new DataGridTextColumn() { Header = "Суммарный расход", Binding = new Binding("TotalExpense") },
			//			new DataGridTextColumn() { Header = "Баланс", Binding = new Binding("Balance") }},
			//	AutoGenerateColumns = false,
			//	IsReadOnly = true,
			//	CanUserAddRows = false,
			//	CanUserDeleteRows = false		
			//};

			////Форматирование в code-behind
			//summaryBalance.Binding.StringFormat = "{0:N2}";

			summaryFiatDataGrid.ItemsSource = fiatBalances;
			summaryCryptoDataGrid.ItemsSource= cryptoBalances;
			//BalancesTab.Content = summaryFiatDataGrid;
		}

		//Настройка фильтра поиска
		private void searchTypeSearchCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox typeCombo = new ComboBox();
			if (searchTypeStackPanel.Children.Count == 1 && searchTypeSearchCombo.SelectedIndex != 0)
			{		
				ComboBoxFilterInicialization(searchTypeSearchCombo.SelectedValue.ToString(), ref typeCombo);
				searchTypeStackPanel.Children.Add(typeCombo);
				typeCombo.SelectionChanged += new SelectionChangedEventHandler(searchTempFilter);
				searchTempFilter(typeCombo, null);

			}
			else if (searchTypeStackPanel.Children.Count > 1 && searchTypeSearchCombo.SelectedIndex == 0)
			{
				typeCombo.Name = "timeFilterOnly";
				searchTempFilter(typeCombo, null);
				typeCombo.SelectionChanged -= new SelectionChangedEventHandler(searchTempFilter);
				searchTypeStackPanel.Children.RemoveAt(1);
			}
			else if(searchTypeStackPanel.Children.Count > 1 && searchTypeSearchCombo.SelectedIndex != 0)
			{
				typeCombo.SelectionChanged -= new SelectionChangedEventHandler(searchTempFilter);
				searchTypeStackPanel.Children.RemoveAt(1);
				typeCombo = new ComboBox();
				ComboBoxFilterInicialization(searchTypeSearchCombo.SelectedValue.ToString(), ref typeCombo);
				searchTypeStackPanel.Children.Add(typeCombo);
				typeCombo.SelectionChanged += new SelectionChangedEventHandler(searchTempFilter);
				searchTempFilter(typeCombo, null);
			}


		}

		private void ComboBoxFilterInicialization(string _datatype, ref ComboBox _combo)
		{
			if (_datatype.Contains("По категории"))
			{
				_combo.ItemsSource = (DataContext as MainWindowModel).StringCategories;
				_combo.Name = "catFilter";
				_combo.SelectedIndex = 0;
			}
			if (_datatype.Contains("По виду"))
			{
				List<string> tempKinds = new List<string>();
				foreach (var kind in (DataContext as MainWindowModel).Kinds)
				{
					tempKinds.Add(kind.Kind);
				}
				_combo.ItemsSource = tempKinds;
				_combo.Name = "kindFilter";
				_combo.SelectedIndex = 0;
			}
			if (_datatype.Contains("По валюте"))
			{
				List<string> tempCoins = new List<string>();
				foreach (var coin in (DataContext as MainWindowModel).Coins)
				{
					tempCoins.Add(coin.ShortName);
				}
				_combo.ItemsSource = tempCoins;
				_combo.Name = "coinFilter";
				_combo.SelectedIndex = 0;
			}
		}

		private void searchTempFilter(object sender, SelectionChangedEventArgs e)
		{
			if (((ComboBox)sender).Name == "catFilter")
			{
				(DataContext as MainWindowModel).categoryFilter = ((ComboBox)sender).SelectedValue.ToString();
				(DataContext as MainWindowModel).kindFilter = string.Empty;
				(DataContext as MainWindowModel).coinFilter = string.Empty;

			}
			if (((ComboBox)sender).Name == "kindFilter")
			{
				(DataContext as MainWindowModel).categoryFilter = string.Empty;
				(DataContext as MainWindowModel).kindFilter = ((ComboBox)sender).SelectedValue.ToString();
				(DataContext as MainWindowModel).coinFilter = string.Empty;
			}
			if (((ComboBox)sender).Name == "coinFilter")
			{
				(DataContext as MainWindowModel).categoryFilter = string.Empty;
				(DataContext as MainWindowModel).kindFilter = string.Empty;
				(DataContext as MainWindowModel).coinFilter = ((ComboBox)sender).SelectedValue.ToString();
			}
			if (((ComboBox)sender).Name == "timeFilterOnly")
			{
				(DataContext as MainWindowModel).categoryFilter = string.Empty;
				(DataContext as MainWindowModel).kindFilter = string.Empty;
				(DataContext as MainWindowModel).coinFilter = string.Empty;
			}
        }

	}

}


