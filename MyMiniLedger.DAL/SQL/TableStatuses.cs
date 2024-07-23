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
        public IEnumerable<StatusModel> GetAll()
        {
            return SQLService<StatusModel>.GetAll("Statuses");
        }

        public StatusModel GetById(int id, string t = "Id")
        {
            return SQLService<StatusModel>.GetByNumber("Statuses", t, id);
        }

        public void Insert(StatusModel entity)
        {
            //string sql = $"insert into Statuses (StatusName, StatusNotes) values (N'{entity.StatusName}', N'{entity.StatusNotes}') ";
            string sql = $"insert into Statuses (StatusName, StatusNotes) values ('{entity.StatusName}', '{entity.StatusNotes}') ";
            SQLService<StatusModel>.UpdateInsertDelete(sql);
        }

        public void Update(StatusModel entity)
        {
            //string sql = $"update Statuses set StatusName = N'{entity.StatusName}', StatusNotes = N'{entity.StatusNotes}' where  id = {entity.Id}";
            string sql = $"update Statuses set StatusName = '{entity.StatusName}', StatusNotes = '{entity.StatusNotes}' where  id = {entity.Id}";
            SQLService<StatusModel>.UpdateInsertDelete(sql);
        }
    }
}
