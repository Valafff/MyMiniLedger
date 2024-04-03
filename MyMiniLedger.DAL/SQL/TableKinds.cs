using MyMiniLedger.DAL.Interfaces;
using MyMiniLedger.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.DAL.SQL
{
	public class TableKinds : ICreate<KindModel>, IUpdate<KindModel>, IReadAll<KindModel>, IReadById<KindModel>
	{
		public Task<IEnumerable<KindModel>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task<KindModel> GetByIdAsync(int id, string tablename = "Id")
		{
			throw new NotImplementedException();
		}

		public Task InsertAsync(KindModel entity)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(KindModel entity)
		{
			throw new NotImplementedException();
		}
	}
}
