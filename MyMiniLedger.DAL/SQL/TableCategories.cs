using MyMiniLedger.DAL.Interfaces;
using MyMiniLedger.DAL.Models;
using MyMiniLedger.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.DAL.SQL
{
	public class TableCategories : ICreate<CategoryModel>, IUpdate<CategoryModel>, IReadAll<CategoryModel>, IReadById<CategoryModel>, IDeleteHard<CategoryModel>
	{

        public async Task<IEnumerable<CategoryModel>> GetAllAsync()
		{
			return await SQLService<CategoryModel>.GetAllAsync("Categories");
		}

		public async Task<CategoryModel> GetByIdAsync(int id, string t = "Id")
		{
			return await SQLService<CategoryModel>.GetByNumber("Categories", t, id);
		}

		public async Task InsertAsync(CategoryModel entity)
		{
			string sql = $"insert into Categories (Category) values (N'{entity.Category}') ";
			await SQLService<CategoryModel>.UpdateInsertDeleteAsync(sql);
		}

		public async Task UpdateAsync(CategoryModel entity)
		{
			string sql = $"update Categories set Category = N'{entity.Category}' where  id = {entity.Id}";
			await SQLService<CategoryModel>.UpdateInsertDeleteAsync(sql);
		}

		public async Task DeleteHardAsync(CategoryModel entity)
		{
			string sql = $"delete from Categories where Id = {entity.Id}";
			await SQLService<CategoryModel>.UpdateInsertDeleteAsync(sql);
		}
	}
}
