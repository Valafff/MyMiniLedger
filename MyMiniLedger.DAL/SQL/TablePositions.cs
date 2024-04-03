using MyMiniLedger.DAL.Interfaces;
using MyMiniLedger.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.DAL.SQL
{
	public class TablePositions : ICreate<PositionModel>, IUpdate<PositionModel>, IReadAll<PositionModel>, IReadById<PositionModel>
	{
		public IEnumerable<PositionModel> GetAll()
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<PositionModel>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public PositionModel GetById(int id)
		{
			throw new NotImplementedException();
		}

		public Task<PositionModel> GetByIdAsync(int id, string tablename = "Id")
		{
			throw new NotImplementedException();
		}

		public void Insert(PositionModel entity)
		{
			throw new NotImplementedException();
		}

		public Task InsertAsync(PositionModel entity)
		{
			throw new NotImplementedException();
		}

		public void Update(PositionModel entity)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(PositionModel entity)
		{
			throw new NotImplementedException();
		}
	}
}
