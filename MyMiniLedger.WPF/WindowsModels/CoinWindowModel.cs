using MyMiniLedger.BLL.Context;
using MyMiniLedger.BLL.Models;
using MyMiniLedger.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.WPF.WindowsModels
{
	public delegate void UpdateCoinsDelegate();

	public class CoinWindowModel : BaseNotify
	{
		//При срабатывании события происходит выполнение метода в MainWindowModel там-же какскадом срабатывает выбор 0го индекса в комбобоксе
		public event UpdateCoinsDelegate UpdateCoinsEvent;

		private readonly Context _context;

		private string _title = "Окно редактирования валют/монет";
		public string Title
		{
			get => _title;
			set => SetField(ref _title, value);
		}

		private CoinUIModel? _selectedCoin;
		public CoinUIModel? SelectedCoin
		{
			get => _selectedCoin;
			set => SetField(ref _selectedCoin, value);
		}

		public ObservableCollection<CoinUIModel>? Coins { get; set; }
		public ObservableCollection<PositionUIModel>? Positions { get; set; }

		public CoinUIModel TempCoin { get; set; }

		public LambdaCommand AddToCoin { get; set; }
		public LambdaCommand DeleteCoin { get; set; }
		public LambdaCommand UpdateCoin { get; set; }

		public CoinWindowModel()
		{

			SelectedCoin = new CoinUIModel();

			_context = new BLL.Context.Context();

			////Инициализация позиций для определения связей
			BLL.Context.ListOfPositions tempPos = new BLL.Context.ListOfPositions();
			List<PositionBLLModel> tempPosAsync = tempPos.GetAllAsync().Result.ToList();

			////Инициализация монет
			BLL.Context.ListOfCoins tempCoin = new BLL.Context.ListOfCoins();
			List<CoinUIModel> tempCoinsAsync = tempCoin.GetAllAsync().Result.Select(c => Mappers.UIMapper.MapCoinBLLToCoinUI(c)).ToList();

			//Определение количества ссылок по Id
			for (int i = 0; i < tempCoinsAsync.Count; i++)
			{
				for (int j = 0; j < tempPosAsync.Count; j++)
				{
					if (tempCoinsAsync[i].Id == tempPosAsync[j].Coin.Id)
					{
						tempCoinsAsync[i].RefNumber++;
					}
				}
			}

			Coins = new ObservableCollection<CoinUIModel>(tempCoinsAsync);

			//Добавление монеты
			AddToCoin = new LambdaCommand(
				async execute =>
				{
					_selectedCoin.RefNumber = 0;
					await _context.CoinsTableBL.InsertAsync(Mappers.UIMapper.MapCoinUIToCoinBLL(_selectedCoin));
					
					var updatedCoin = (tempCoin.GetAllAsync().Result.Select(c => Mappers.UIMapper.MapCoinBLLToCoinUI(c)).ToList()).Where(t => t.ShortName == _selectedCoin.ShortName);
					var temp = _selectedCoin.Clone();
					((CoinUIModel)temp).Id = updatedCoin.First().Id;
					Coins.Add((CoinUIModel)temp);
					TempCoin = (CoinUIModel)temp;
					//Выполнение события(подпись на событие в CoinWindow.xaml.cs
					UpdateCoinsEvent();
				},
				canExecute => SelectedCoin is not null && SelectedCoin.ShortName != null && Coins.Any(c => c.ShortName == _selectedCoin.ShortName) == false
				);

			//	//Полное удаление монеты
			DeleteCoin = new LambdaCommand(async execute =>
			{
				await _context.CoinsTableBL.DeleteAsync(Mappers.UIMapper.MapCoinUIToCoinBLL(_selectedCoin));

				IEnumerable<CoinUIModel>?  TempDeletedCoin = Coins.Where(c => c.Id == _selectedCoin.Id);

				Coins.Remove(TempDeletedCoin.First());
				_selectedCoin.Id = 0;
				_selectedCoin.ShortName = null;
				_selectedCoin.FullName = null;
				_selectedCoin.CoinNotes = null;
				UpdateCoinsEvent();
			},
			canExecute => SelectedCoin is not null && SelectedCoin.ShortName != null && _selectedCoin.Id != 0 && _selectedCoin.RefNumber == 0);

			//	//Редактирование монеты
			UpdateCoin = new LambdaCommand(async execute =>
			{
				await _context.CoinsTableBL.UpdateAsync(Mappers.UIMapper.MapCoinUIToCoinBLL(_selectedCoin));
				
				var t = Coins.Where(c => c.Id == _selectedCoin.Id);
				Coins.Remove(t.First());
				var temp = _selectedCoin.Clone();
				Coins.Add((CoinUIModel)temp);
				UpdateCoinsEvent();
			},
			canExecute => SelectedCoin is not null && _selectedCoin.ShortName != null && SelectedCoin.Id != 0 && Coins.Any(c => c.ShortName == _selectedCoin.ShortName) == false);
		}

	}
}
