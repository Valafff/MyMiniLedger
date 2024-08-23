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

		//public LambdaCommand TakeNewDeal { get; set; }
		//public LambdaCommand TakeNewDealAndClose { get; set; }
		//public LambdaCommand ContinueDealAndClose { get; set; }

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
				//Console.WriteLine(BuyCheck);
				//Console.WriteLine(SellCheck);
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

			ContinueDealExecuteTest = new LambdaCommand(execute => {}, canExecute => SelectedDeal.DealNumber != 0);
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
	}
}
