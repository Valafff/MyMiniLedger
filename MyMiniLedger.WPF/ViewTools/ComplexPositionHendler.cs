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
            int tempZeroParrentKey = 0;
            int tempParrentKey = 0;

                await Console.Out.WriteLineAsync($"{_editPosition.ZeroParrentKey}");
			    await Console.Out.WriteLineAsync($"{_editPosition.ParrentKey}");

			//await _context.PositionsTableBL.UpdateAsync(Mappers.UIMapper.MapPositionUIToPositionBLL(_editPosition));

			Console.WriteLine("Hello");
        }

    }
}
