using MyMiniLedger.DAL.Interfaces;
using MyMiniLedger.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.DAL.SQL
{
	public class TableCoins : ICreate<CoinModel>, IUpdate<CoinModel>, IReadAll<CoinModel>, IReadById<CoinModel>
	{
		public Task<IEnumerable<CoinModel>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task<CoinModel> GetByIdAsync(int id, string tablename = "Id")
		{
			throw new NotImplementedException();
		}

		public Task InsertAsync(CoinModel entity)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(CoinModel entity)
		{
			throw new NotImplementedException();
		}
	}
}
