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
	public class TableKinds : ICreate<KindModel>, IUpdate<KindModel>, IReadAll<KindModel>, IReadById<KindModel>
	{
		public async Task<IEnumerable<KindModel>> GetAllAsync()
		{
			return await SQLService<KindModel>.GetAllAsync("Kinds");
		}

		public async Task<KindModel> GetByIdAsync(int id, string t = "Id")
		{
			return await SQLService<KindModel>.GetByNumber("Kinds", t, id);
		}

		public async Task InsertAsync(KindModel entity)
		{
			string sql = $"insert into Kinds (CategoryId, Kind) values ({entity.CategoryId}, N'{entity.Kind}') ";
			await SQLService<KindModel>.UpdateAndInsertAsync(sql);
		}

		public async Task UpdateAsync(KindModel entity)
		{
			string sql = $"update Kinds set CategoryId = {entity.CategoryId}, Kind = N'{entity.Kind}' where  id = {entity.Id}";
			await SQLService<CategoryModel>.UpdateAndInsertAsync(sql);
		}
	}
}
