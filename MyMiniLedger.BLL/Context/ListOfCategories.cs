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
	public class ListOfCategories
	{

		private readonly IReadAll<DAL.Models.CategoryModel> _sourceForRead;
		private readonly IReadById<DAL.Models.CategoryModel> _sourceForReadById;
		private readonly ICreate<DAL.Models.CategoryModel> _sourceForInsert;
		private readonly IUpdate<DAL.Models.CategoryModel> _sourceForUpdate;
		private readonly IDeleteHard<DAL.Models.CategoryModel> _sourceForDelete;

		public ListOfCategories()
		{
			_sourceForRead = new TableCategories();
			_sourceForReadById = new TableCategories();
			_sourceForInsert = new TableCategories();
			_sourceForUpdate = new TableCategories();
			_sourceForDelete = new TableCategories();
		}

		////Получение всех данных
		//public async IAsyncEnumerable<CategoryBLLModel> GetAllAsync()
		//{
		//	var result = await _sourceForRead.GetAllAsync();
		//	foreach (var item in result)
		//	{
		//		yield return Mappers.MapperBL.MapCategoryDALToCategoryBLL(item);
		//	}
		//}


		//Получение всех данных
		public async Task<IEnumerable<CategoryBLLModel>> GetAllAsync()
		{
			var result = await _sourceForRead.GetAllAsync(); 
			List<CategoryBLLModel> enumerable = new List<CategoryBLLModel>();

			foreach (var item in result)
			{
				enumerable.Add( Mappers.MapperBL.MapCategoryDALToCategoryBLL(item));
			}
			return enumerable.AsEnumerable();
		}

		//Получение данных по Id
		public async Task <CategoryBLLModel> GetByIdAsync(int id, string t = "Id")
		{
			var result =  await _sourceForReadById.GetByIdAsync(id, t);
			return Mappers.MapperBL.MapCategoryDALToCategoryBLL(result);
		}

		//Вставка данных
		public async Task InsertAsync(CategoryBLLModel entity)
		{
			await _sourceForInsert.InsertAsync(Mappers.MapperBL.MapCategoryBLLToCategoryDAL(entity));
		}

		//Изменение данных
		public async Task UpdateAsync(CategoryBLLModel entity)
		{
			await _sourceForUpdate.UpdateAsync(Mappers.MapperBL.MapCategoryBLLToCategoryDAL(entity));
		}

		//Реализовать удаление с записью Deleted

		//Полное удаление
		public async Task DeleteAsync(CategoryBLLModel entity)
		{
			await _sourceForDelete.DeleteHardAsync(Mappers.MapperBL.MapCategoryBLLToCategoryDAL(entity));
		}
	}
}
