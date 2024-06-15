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
	public class TableStatuses : ICreate<StatusModel>, IUpdate<StatusModel>, IReadAll<StatusModel>, IReadById<StatusModel>
	{
		public async Task<IEnumerable<StatusModel>> GetAllAsync()
		{
			return await SQLService<StatusModel>.GetAllAsync("Statuses");
		}

		public async Task<StatusModel> GetByIdAsync(int id, string t = "Id")
		{
			return await SQLService<StatusModel>.GetByNumber("Statuses", t, id);
		}

		public async Task InsertAsync(StatusModel entity)
		{
			string sql = $"insert into Statuses (StatusName, StatusNotes) values (N'{entity.StatusName}', N'{entity.StatusNotes}') ";
			await SQLService<StatusModel>.UpdateInsertDeleteAsync(sql);
		}

		public async Task UpdateAsync(StatusModel entity)
		{
			string sql = $"update Statuses set StatusName = N'{entity.StatusName}', StatusNotes = N'{entity.StatusNotes}' where  id = {entity.Id}";
			await SQLService<StatusModel>.UpdateInsertDeleteAsync(sql);
		}
	}
}
