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
	public class ListOfKinds
	{

		private readonly IReadAll<DAL.Models.KindModel> _sourceForRead;
		private readonly IReadById<DAL.Models.KindModel> _sourceForReadById;
		private readonly ICreate<DAL.Models.KindModel> _sourceForInsert;
		private readonly IUpdate<DAL.Models.KindModel> _sourceForUpdate;
		private readonly IDeleteHard<DAL.Models.KindModel> _sourceForDelete;

		TableCategories tempCatTable = new TableCategories();

		public ListOfKinds()
		{
			_sourceForRead = new TableKinds();
			_sourceForReadById = new TableKinds();
			_sourceForInsert = new TableKinds();
			_sourceForUpdate = new TableKinds();
			_sourceForDelete = new TableKinds();
		}

		////Получение всех данных
		//public async IAsyncEnumerable<KindBLLModel> GetAllAsync()
		//{

		//	IEnumerable<CategoryModel> temp = await tempCatTable.GetAllAsync();
		//	IEnumerable<KindModel> result = await _sourceForRead.GetAllAsync();

		//	foreach (var item in result)
		//	{
		//		yield return Mappers.MapperBL.MapKindDALToKindBLL(item, temp);
		//	}
		//}

		//Получение всех данных
		public async Task <IEnumerable<KindBLLModel>> GetAllAsync()
		{

			IEnumerable<CategoryModel> temp = await tempCatTable.GetAllAsync();
			IEnumerable<KindModel> result = await _sourceForRead.GetAllAsync();

			List<KindBLLModel> tempResult = new List<KindBLLModel>();
			foreach (var item in result)
			{
				tempResult.Add( Mappers.MapperBL.MapKindDALToKindBLL(item, temp));
			}
			return tempResult;
		}

		//Получение данных по Id
		public async Task<KindBLLModel> GetByIdAsync(int id, string t = "Id")
		{
			IEnumerable<CategoryModel> temp = await tempCatTable.GetAllAsync();
			KindModel result = await _sourceForReadById.GetByIdAsync(id, t);
			return Mappers.MapperBL.MapKindDALToKindBLL(result, temp);
		}

		//Вставка данных
		public async Task InsertAsync(KindBLLModel entity)
		{
			await _sourceForInsert.InsertAsync(Mappers.MapperBL.MapKindBLLToKindDAL(entity));
		}

		//Изменение данных
		public async Task UpdateAsync(KindBLLModel entity)
		{
			await _sourceForUpdate.UpdateAsync(Mappers.MapperBL.MapKindBLLToKindDAL(entity));
		}

		//Реализовать удаление с записью Deleted

		//Реализовать полное удаление
		public async Task DeleteAsync(KindBLLModel entity)
		{
			await _sourceForDelete.DeleteHardAsync(Mappers.MapperBL.MapKindBLLToKindDAL(entity));
		}
	}
}
