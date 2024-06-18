using MyMiniLedger.BLL.Context;
using MyMiniLedger.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.WPF.ViewTools
{
	//Обработчик добавления редактирования и удаления комплексных позиций
	class ComplexPositionHendler
	{

		public async void AddComplexPosition(ObservableCollection<PositionUIModel> _selectedPositions, PositionUIModel _editPosition, Context _context)
		{
			int? maxPositionKey = null;

			//await Console.Out.WriteLineAsync(_editPosition.OpenDate);
			//await Console.Out.WriteLineAsync(_editPosition.CloseDate);

			if (_selectedPositions.Count > 1 && _editPosition.ZeroParrentKey == null)
			{
				_editPosition.ZeroParrentKey = _editPosition.PositionKey;
				maxPositionKey = _selectedPositions.Max(p => p.PositionKey);
				_editPosition.ParrentKey = maxPositionKey++;
			}
			else if (_selectedPositions.Count > 1)
			{
				maxPositionKey = _selectedPositions.Max(p => p.PositionKey);
				_editPosition.ParrentKey = maxPositionKey++;
			}
			else if (_selectedPositions.Count == 1)
			{
				_editPosition.ZeroParrentKey = _editPosition.PositionKey;
				_editPosition.ParrentKey = _editPosition.PositionKey;

			}
			await Console.Out.WriteLineAsync($"ZeroParrentKey {_editPosition.ZeroParrentKey}");
			await Console.Out.WriteLineAsync($"ParrentKey {_editPosition.ParrentKey}");
			await Console.Out.WriteLineAsync($"maxParrentKey {maxPositionKey}");

			_editPosition.CloseDate = (ViewTools.FormatterPositions.SetCloseDate(_editPosition.Status.StatusName)).ToString();
			await _context.PositionsTableBL.InsertAsync(Mappers.UIMapper.MapPositionUIToPositionBLL(_editPosition));
		}

	}
}
