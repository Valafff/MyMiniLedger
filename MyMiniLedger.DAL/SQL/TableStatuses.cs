using MyMiniLedger.DAL.Interfaces;
using MyMiniLedger.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.DAL.SQL
{
	internal class TableStatuses : ICreate<StatusModel>, IUpdate<StatusModel>, IReadAll<StatusModel>, IReadById<StatusModel>
	{
		public Task<IEnumerable<StatusModel>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task<StatusModel> GetByIdAsync(int id, string tablename = "Id")
		{
			throw new NotImplementedException();
		}

		public Task InsertAsync(StatusModel entity)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(StatusModel entity)
		{
			throw new NotImplementedException();
		}
	}
}
