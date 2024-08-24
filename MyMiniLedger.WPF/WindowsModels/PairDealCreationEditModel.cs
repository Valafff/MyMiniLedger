using MyMiniLedger.BLL.Context;
using MyMiniLedger.DAL.Config;
using MyMiniLedger.WPF.Models;
using MyMiniLedger.WPF.ViewTools;
using MyMiniLedger.WPF.Windows.PairDealCreationEdit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyMiniLedger.WPF.WindowsModels
{
	public class PairDealCreationEditModel : EditContinuePositionWindowsModel
	{
		const string DealSignature = "Парная сделка_";
		public event UpdateDelegate PairDealEdit_UpdateEvent;
		//Название окна
		private string _titleEditContinuePos = "Создать/продолжить парную сделку";
		public string TitelDealCreation
		{
			get => _titleEditContinuePos;
			set => SetField(ref _titleEditContinuePos, value);
		}

		private bool _buyCheck = true;
		public bool BuyCheck
		{
			get => _buyCheck;
			set => SetField(ref _buyCheck, value);
		}
		private bool _sellCheck = false;
		public bool SellCheck
		{
			get => _sellCheck;
			set => SetField(ref _sellCheck, value);
		}

		private string _buyItemName;
		public string BuyItemName
		{
			get => _buyItemName;
			set => SetField(ref _buyItemName, value);
		}

		private string _sellItemName;
		public string SellItemName
		{
			get => _sellItemName;
			set => SetField(ref _sellItemName, value);
		}

		private string _buyCount = 0.ToString(CultureInfo.CurrentUICulture);
		public string BuyCount
		{
			get => _buyCount;
			set => SetField(ref _buyCount, value);
		}
		private string _sellCount = 0.ToString(CultureInfo.CurrentUICulture);
		public string SellCount
		{
			get => _sellCount;
			set => SetField(ref _sellCount, value);
		}

		private string _currentBuyFee = 0.ToString(CultureInfo.CurrentUICulture);
		public string CurrentBuyFee
		{
			get => _currentBuyFee;
			set => SetField(ref _currentBuyFee, value);
		}
		private string _currentSellFee = 0.ToString(CultureInfo.CurrentUICulture);
		public string CurrentSellFee
		{
			get => _currentSellFee;
			set => SetField(ref _currentSellFee, value);
		}

		private string _defaultKindToBuy = "none";
		public string DefaultKindToBuy
		{
			get => _defaultKindToBuy;
			set => SetField(ref _defaultKindToBuy, value);
		}
		private string _defaultKindToSell = "none";
		public string DefaultKindToSell
		{
			get => _defaultKindToSell;
			set => SetField(ref _defaultKindToSell, value);
		}
		private string _defaultFee = 0.ToString(CultureInfo.CurrentUICulture);
		public string DefaultFeel
		{
			get => _defaultFee;
			set => SetField(ref _defaultFee, value);
		}

		private int _dealNumber = 0;
		public int DealNumber
		{
			get => _dealNumber;
			set => SetField(ref _dealNumber, value);
		}

		private PairDealModel? _selectedDeal;
		public PairDealModel? SelectedDeal
		{
			get => _selectedDeal;
			set => SetField(ref _selectedDeal, value);
		}

		public ObservableCollection<string> WhatToBuy { get; set; }
		public ObservableCollection<string> WhatToSell { get; set; }
		public ObservableCollection<PairDealModel> ActiveDeals { get; set; }


		Config ConfigData = new Config();

		public LambdaCommand EditDefaultData { get; set; }
		public LambdaCommand ChangeRadioButton { get; set; }
		public LambdaCommand ContinueDealExecuteTest {  get; set; }

		public PairDealCreationEditModel()
		{
			ConfigData = Config.GetFromConfig();
			TitleEditContinuePosition = _titleEditContinuePos;
			DefaultKindToBuy = ConfigData.DefaultKindToBuy;
			DefaultKindToSell = ConfigData.DefaultKindToSell;
			DefaultFeel = ConfigData.DefaultFee.ToString(CultureInfo.CurrentUICulture);
			CurrentBuyFee = DefaultFeel;
			SelectedDeal = new PairDealModel();
			WhatToBuy = new ObservableCollection<string>();
			WhatToSell = new ObservableCollection<string>();
			ActiveDeals = new ObservableCollection<PairDealModel>();


			EditDefaultData = new LambdaCommand(execute =>
			{
				EditDefaultDataWindow editWindow = new EditDefaultDataWindow(ref ConfigData, this);
				editWindow.ShowDialog();
			});

			ChangeRadioButton = new LambdaCommand(execute =>
			{
				if (!BuyCheck && SellCheck)
				{
					CurrentSellFee = DefaultFeel.ToString(CultureInfo.CurrentUICulture);
					CurrentBuyFee = 0.ToString(CultureInfo.CurrentUICulture);
				}
				else
				{
					CurrentSellFee = 0.ToString(CultureInfo.CurrentUICulture);
					CurrentBuyFee = DefaultFeel.ToString(CultureInfo.CurrentUICulture);
				}
			});

			ContinueDealExecuteTest = new LambdaCommand(execute => {}, canExecute => SelectedDeal.DealNumber != 0 
			&& ((SelectedDeal.BuyItem == BuyItemName && SelectedDeal.SellItem == SellItemName)
			|| (SelectedDeal.BuyItem == SellItemName && SelectedDeal.SellItem == BuyItemName)));
		}

		public void ReWriteDeals(PairDealModel _tempModel, string _dealSignature)
		{
			_tempModel.isOpen = false;
			JsonSerializerOptions options = new JsonSerializerOptions
			{
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			};
			string posData = _dealSignature + JsonSerializer.Serialize(_tempModel, options);
			foreach (var item in SelectedPositions)
			{
				item.Tag = posData;
				_context.PositionsTableBL.Update(Mappers.UIMapper.MapPositionUIToPositionBLL(item));
				Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentUICulture; //Важен порядок установки до работы с БД работает некорректно. Важно вставлять после записи чтения из БД
				PairDealEdit_UpdateEvent();
			}
		}

		public void DealsInicialization()
		{
			ActiveDeals.Clear();
			//Из всего списка нахожу только сделки
			var PositionsWithDeals = MAINPOSITIONSCOLLECTION.Where(p => p.Tag.Contains(DealSignature));
			var DealsStrings = PositionsWithDeals.Select(s => s.Tag);
			List<PairDealModel> MainDealList = new List<PairDealModel>();
			foreach (var item in DealsStrings)
			{
				MainDealList.Add(JsonSerializer.Deserialize<PairDealModel>(item.Replace(DealSignature, "")));
			}

			//Нахожу только уникальные номера сделок
			var DealNumbers = MainDealList.Select(n => n.DealNumber).Distinct().Reverse();

			//Конечная обработка данных
			foreach (var item in DealNumbers)
			{
				var tempDealsByNumber = MainDealList.Where(d => d.DealNumber == item).ToList();
				PairDealModel resultDeal = new PairDealModel();
				resultDeal.DealNumber = tempDealsByNumber.First().DealNumber;
				resultDeal.DealOpenTime = tempDealsByNumber.First().DealOpenTime;
				resultDeal.BuyItem = tempDealsByNumber.First().BuyItem;
				resultDeal.SellItem = tempDealsByNumber.First().SellItem;
				resultDeal.isOpen = tempDealsByNumber.First().isOpen;
				resultDeal.ParentZeroKey = tempDealsByNumber.First().ParentZeroKey;
				//Расчет данных
				resultDeal.TotalBuyAmount = Math.Round(PositionsWithDeals.Where(z => (z.ZeroParrentKey == resultDeal.ParentZeroKey || z.PositionKey == resultDeal.ParentZeroKey) && z.Coin.ShortName == resultDeal.BuyItem).Sum(p => Double.Parse(p.Saldo, CultureInfo.CurrentCulture)), 8);
				resultDeal.TotalSellAmount = Math.Round(PositionsWithDeals.Where(z => (z.ZeroParrentKey == resultDeal.ParentZeroKey || z.PositionKey == resultDeal.ParentZeroKey) && z.Coin.ShortName == resultDeal.SellItem).Sum(p => Double.Parse(p.Saldo, CultureInfo.CurrentCulture)), 8);
				resultDeal.SellToBuyCourse = Math.Abs(Math.Round(resultDeal.TotalSellAmount / resultDeal.TotalBuyAmount, 6));
				resultDeal.BuyToSellCourse = Math.Abs(Math.Round(resultDeal.TotalBuyAmount / resultDeal.TotalSellAmount, 6));



				//!!!ToDo Сделать список стандартных курсов, добавить их в конфигурационный файл
				if (resultDeal.BuyItem == "USD" || resultDeal.BuyItem == "USDT" || resultDeal.BuyItem == "USDC" || resultDeal.BuyItem == "BTC")
				{
					resultDeal.StandartCourse = resultDeal.SellToBuyCourse.ToString();
				}
				else if (resultDeal.SellItem == "USD" || resultDeal.SellItem == "USDT" || resultDeal.SellItem == "USDC" || resultDeal.SellItem == "BTC")
				{
					resultDeal.StandartCourse = resultDeal.BuyToSellCourse.ToString();
				}
				else
					resultDeal.StandartCourse = "Не определен";

				if (resultDeal.TotalSellAmount >= 0 && resultDeal.TotalBuyAmount >= 0)
				{
					resultDeal.StandartCourse = "Выгоден любой курс";
				}




				//Распределение данных
				if (resultDeal.isOpen == true)
				{
					ActiveDeals.Add(resultDeal);
				}
			}

		}
	}
}
