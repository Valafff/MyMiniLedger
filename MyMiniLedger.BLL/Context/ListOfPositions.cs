using MyMiniLedger.BLL.Models;
using MyMiniLedger.DAL.Interfaces;
using MyMiniLedger.DAL.Models;
using MyMiniLedger.DAL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.BLL.Context
{
	public class ListOfPositions
	{

		private readonly IReadAll<DAL.Models.PositionModel> _sourceForRead;
		private readonly IReadById<DAL.Models.PositionModel> _sourceForReadById;
		private readonly ICreate<DAL.Models.PositionModel> _sourceForInsert;
		private readonly IUpdate<DAL.Models.PositionModel> _sourceForUpdate;

		TableCategories tempCatTable = new TableCategories();
		TableKinds tempKindsTable = new TableKinds();
		TableStatuses tempStatusesTable = new TableStatuses();
		TableCoins tempCoinsTable = new TableCoins();

		public ListOfPositions()
		{
			_sourceForRead = new TablePositions();
			_sourceForReadById = new TablePositions();
			_sourceForInsert = new TablePositions();
			_sourceForUpdate = new TablePositions();
		}

		//Получение всех данных
		public async IAsyncEnumerable<PositionBLLModel> GetAllAsync()
		{
			IEnumerable<CategoryModel> tempCategories = await tempCatTable.GetAllAsync();
			IEnumerable<KindModel> tempKinds = await tempKindsTable.GetAllAsync();
			IEnumerable<StatusModel> tempStatuses = await tempStatusesTable.GetAllAsync();
			IEnumerable<CoinModel> tempCoins = await tempCoinsTable.GetAllAsync();

			IEnumerable<PositionModel> result = await _sourceForRead.GetAllAsync();

			foreach (var item in result)
			{
				yield return Mappers.MapperBL.MapPositionDALToPositionBLL(item, tempKinds, tempCoins, tempStatuses, tempCategories);
			}
		}

		//Получение данных по Id
		public async Task<PositionBLLModel> GetByIdAsync(int id, string t = "Id")
		{
			IEnumerable<CategoryModel> tempCategories = await tempCatTable.GetAllAsync();
			IEnumerable<KindModel> tempKinds = await tempKindsTable.GetAllAsync();
			IEnumerable<StatusModel> tempStatuses = await tempStatusesTable.GetAllAsync();
			IEnumerable<CoinModel> tempCoins = await tempCoinsTable.GetAllAsync();
			PositionModel result = await _sourceForReadById.GetByIdAsync(id, t);
			return Mappers.MapperBL.MapPositionDALToPositionBLL(result, tempKinds, tempCoins, tempStatuses, tempCategories);
		}

		//Вставка данных
		public async Task InsertAsync(PositionBLLModel entity)
		{
			await _sourceForInsert.InsertAsync(Mappers.MapperBL.MapPositionBLLToPositionDAL(entity));
		}

		//Изменение данных
		public async Task UpdateAsync(PositionBLLModel entity)
		{
			await _sourceForUpdate.UpdateAsync(Mappers.MapperBL.MapPositionBLLToPositionDAL(entity));
		}

		//Реализовать удаление с записью Deleted

		//Реализовать полное удаление
	}
}
