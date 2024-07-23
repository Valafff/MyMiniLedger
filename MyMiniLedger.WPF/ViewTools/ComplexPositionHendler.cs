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
		public async void AddComplexPosition(Context _context, ObservableCollection<PositionUIModel> _selectedPositions, PositionUIModel _editPosition)
		{
			int? maxPositionKey = null;

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
            _context.PositionsTableBL.Insert(Mappers.UIMapper.MapPositionUIToPositionBLL(_editPosition));
        }

		public async Task<PositionUIModel> DeleteComplexPosition(Context _context, ObservableCollection<PositionUIModel> _selectedPositions, PositionUIModel _deletingPosition)
		{
			int? rootPosition = _deletingPosition.ZeroParrentKey;
			int? previousPosition = _deletingPosition.ParrentKey;
			int? nextPosition = null;
			PositionUIModel newSelectedPosition = new PositionUIModel();
			foreach (var position in _selectedPositions)
			{
				if (position.ParrentKey == _deletingPosition.PositionKey)
				{
					nextPosition = position.PositionKey;
				}
			}
			//Удаление корневой позиции
			if (rootPosition == null)
			{
				foreach (var position in _selectedPositions)
				{

					if (position.PositionKey == nextPosition)
					{
						position.ZeroParrentKey = null;
						position.ParrentKey = null;
						newSelectedPosition = position;
					}
					else
					{
						position.ZeroParrentKey = nextPosition;
					}
                    _context.PositionsTableBL.Update(Mappers.UIMapper.MapPositionUIToPositionBLL(position));
                }
				_selectedPositions.Remove(_deletingPosition);
                _context.PositionsTableBL.Delete(Mappers.UIMapper.MapPositionUIToPositionBLL(_deletingPosition));
            }
			//Удаление промежуточной или последней позиции
			else
			{
				foreach(var position in _selectedPositions)
				{
					if (position.PositionKey == previousPosition)
					{
						newSelectedPosition = position;
					}
					//Если найдена следующая позиция за удаляемой
					else if (position.ParrentKey == _deletingPosition.PositionKey)
					{
						position.ParrentKey = _deletingPosition.ParrentKey;
                        _context.PositionsTableBL.Update(Mappers.UIMapper.MapPositionUIToPositionBLL(position));
                    }			
				}
				_selectedPositions.Remove(_deletingPosition);
                _context.PositionsTableBL.Delete(Mappers.UIMapper.MapPositionUIToPositionBLL(_deletingPosition));
            }
			return newSelectedPosition;
		}

		public async void DeleteAllComplexPositionsAtRootKey(Context _context, ObservableCollection<PositionUIModel> _selectedPositions)
		{
			foreach (var position in _selectedPositions)
			{
                _context.PositionsTableBL.Delete(Mappers.UIMapper.MapPositionUIToPositionBLL(position));
            }
			_selectedPositions.Clear();
		}

	}
}
