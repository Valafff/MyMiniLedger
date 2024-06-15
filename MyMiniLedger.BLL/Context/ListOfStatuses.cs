using MyMiniLedger.BLL.Models;
using MyMiniLedger.DAL.Interfaces;
using MyMiniLedger.DAL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.BLL.Context
{
	public class ListOfStatuses
	{

		private readonly IReadAll<DAL.Models.StatusModel> _sourceForRead;
		private readonly IReadById<DAL.Models.StatusModel> _sourceForReadById;
		private readonly ICreate<DAL.Models.StatusModel> _sourceForInsert;
		private readonly IUpdate<DAL.Models.StatusModel> _sourceForUpdate;

		public ListOfStatuses()
		{
			_sourceForRead = new TableStatuses();
			_sourceForReadById = new TableStatuses();
			_sourceForInsert = new TableStatuses();
			_sourceForUpdate = new TableStatuses();
		}

		////Получение всех данных
		//public async IAsyncEnumerable<StatusBLLModel> GetAllAsync()
		//{
		//	var result = await _sourceForRead.GetAllAsync();
		//	foreach (var item in result)
		//	{
		//		yield return Mappers.MapperBL.MapStatusDALToStatusBLL(item);
		//	}
		//}

		//Получение всех данных
		public async Task<IEnumerable<StatusBLLModel>> GetAllAsync()
		{
			var result = await _sourceForRead.GetAllAsync();
			List<StatusBLLModel> tempResult = new List<StatusBLLModel>();
			foreach (var item in result)
			{
				tempResult.Add(Mappers.MapperBL.MapStatusDALToStatusBLL(item));
			}
			return tempResult;
		}

		//Получение данных по Id
		public async Task<StatusBLLModel> GetByIdAsync(int id, string t = "Id")
		{
			var result = await _sourceForReadById.GetByIdAsync(id, t);
			return Mappers.MapperBL.MapStatusDALToStatusBLL(result);
		}

		//Вставка данных
		public async Task InsertAsync(StatusBLLModel entity)
		{
			await _sourceForInsert.InsertAsync(Mappers.MapperBL.MapStatusBLLToStatusDAL(entity));
		}

		//Изменение данных
		public async Task UpdateAsync(StatusBLLModel entity)
		{
			await _sourceForUpdate.UpdateAsync(Mappers.MapperBL.MapStatusBLLToStatusDAL(entity));
		}

		//Реализовать удаление с записью Deleted

		//Реализовать полное удаление

	}
}
