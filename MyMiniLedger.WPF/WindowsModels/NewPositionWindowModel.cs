using MyMiniLedger.BLL.Context;
using MyMiniLedger.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.WPF.WindowsModels
{
    public class NewPositionWindowModel: BaseNotify
    {

		private readonly Context _context;
		private string _title = "Создание новой позиции";
		public string Title
		{
			get => _title;
			set => SetField(ref _title, value);
		}

		//public ObservableCollection<PositionUIModel> PositionsFromMainWindow { get; set; }
		//public ObservableCollection<CategoryUIModel> Categories { get; set; }
		//public ObservableCollection<CoinUIModel> Coins { get; set; }
		//public ObservableCollection<KindUIModel> Kinds { get; set; }
		//public ObservableCollection<StatusUIModel> Statuses { get; set; }

		public LambdaCommand InsertNewPosition { get; set; }

		public NewPositionWindowModel()
        {
			InsertNewPosition = new LambdaCommand(
				async execute =>
				{
					//	//Добавление
					//	_selectedKind.Category = SelectedCategory;
					//	await _context.KindsTableBL.InsertAsync(Mappers.UIMapper.MapKindUIToKindBLL(_selectedKind));
					//	//Обновление списка UI
					//	var updatedKind = (tempKind.GetAllAsync().Result.Select(k => Mappers.UIMapper.MapKindBLLToKindUI(k)).ToList()).Where(t => t.Kind == _selectedKind.Kind);
					//	var temp = _selectedKind.Clone();
					//	((KindUIModel)temp).Id = updatedKind.First().Id;
					//	Kinds.Add((KindUIModel)temp);

					//},
					//canExecute => SelectedKind is not null &&
					//SelectedKind.Kind != null &&
					////Подумать над возможностью добавлять одинаковые названия видов при отличающихся категориях
					//Kinds.Any(k => k.Kind == _selectedKind.Kind) == false
				}
				);

		}



    }
}
