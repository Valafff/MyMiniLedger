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
	public class MainWindowModel : BaseNotify
	{
		private readonly Context _context;

		public ObservableCollection<PositionUIModel> Positions { get; set; }

		public MainWindowModel()
		{
			var cf = new BLL.InitConfigBLL("config.json");

			_context = new BLL.Context.Context();
			BLL.Context.ListOfPositions tempPos = new BLL.Context.ListOfPositions();
			////.Net 6.0 не поддерживает .ToBlockingEnumerable()
			IEnumerable<PositionUIModel> tempPositionsAsync = tempPos.GetAllAsync().Result.Select(pos => Mappers.UIMapper.MapPositionBLLToPositionUI(pos));
			Positions = new ObservableCollection<PositionUIModel>(tempPositionsAsync);

		}

	}
}
