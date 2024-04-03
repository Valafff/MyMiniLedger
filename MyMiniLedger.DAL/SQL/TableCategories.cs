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
	public class TableCategories : ICreate<CategoryModel>, IUpdate<CategoryModel>, IReadAll<CategoryModel>, IReadById<CategoryModel>
	{
		public async Task<IEnumerable<CategoryModel>> GetAllAsync()
		{
			return await SQLService<CategoryModel>.GetAllAsync("Categories");
		}

		public async Task<CategoryModel> GetByIdAsync(int id, string t)
		{
			return await SQLService<CategoryModel>.GetByNumber("Categories", t, id);
		}

		public async Task InsertAsync(CategoryModel entity)
		{
			string sql = $"insert into Categories (Category) values ('{entity.Category}') ";
			await SQLService<CategoryModel>.UpdateAndInsertAsync(sql);
		}

		public async Task UpdateAsync(CategoryModel entity)
		{
			string sql = $"update Categories set Category = {entity.Category} where  id = {entity.Id}";
			await SQLService<CategoryModel>.UpdateAndInsertAsync(sql);
		}
	}


}
