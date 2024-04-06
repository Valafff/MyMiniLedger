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
		private readonly BLL.Context.Context _context;

		public ObservableCollection<PositionUIModel> Positions { get; set; }

		public MainWindowModel()
		{
			_context = new BLL.Context.Context();
			BLL.Context.ListOfPositions tempPos = new BLL.Context.ListOfPositions();


			//IAsyncEnumerable<BLL.Models.PositionBLLModel> tempPositions = tempPos.GetAllAsync();
			 var tempPositions = tempPos.GetAllAsync();


			//.Net 6.0 не поддерживает .ToBlockingEnumerable() -> foreach
			foreach (var position in tempPositions)
			{
				PositionUIModel temp = Mappers.UIMapper.MapPositionBLLToPositionUI(position);
				Positions.Add(temp);
			}




		}

	}
}
