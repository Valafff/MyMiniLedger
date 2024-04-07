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
			//ToDo В файле сделать базовые настройки строк, дат, пользователя 
			var cf = new BLL.InitConfigBLL("config.json");

			_context = new BLL.Context.Context();
			BLL.Context.ListOfPositions tempPos = new BLL.Context.ListOfPositions();
			////.Net 6.0 не поддерживает .ToBlockingEnumerable()
			List<PositionUIModel> tempPositionsAsync = tempPos.GetAllAsync().Result.Select(pos => Mappers.UIMapper.MapPositionBLLToPositionUI(pos)).ToList();

			//Удаление Deleted позиций
			tempPositionsAsync = ViewTools.FormatterPositions.ErasePosFromTableByStatus(tempPositionsAsync, "Deleted");
			//Удаление нулевой даты
			tempPositionsAsync = ViewTools.FormatterPositions.EditPosFromTableByDate(tempPositionsAsync, new DateTime(2000, 01, 01));
			//Переименовывание статусов
			tempPositionsAsync = ViewTools.FormatterPositions.EditPosFromTableByStatus(tempPositionsAsync, "Open", "Открыта");
			tempPositionsAsync = ViewTools.FormatterPositions.EditPosFromTableByStatus(tempPositionsAsync, "Closed", "Закрыта");
			//Отбрасывание 0 после разделителя в зависимости от типа валюты fiat crypto
			tempPositionsAsync = ViewTools.FormatterPositions.EditDecemalPosFromTableByMarker(tempPositionsAsync, "fiat", "0.00");

			Positions = new ObservableCollection<PositionUIModel>(tempPositionsAsync);
		}

	}
}
