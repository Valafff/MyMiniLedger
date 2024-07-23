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
	public class TableKinds : ICreate<KindModel>, IUpdate<KindModel>, IReadAll<KindModel>, IReadById<KindModel>, IDeleteHard<KindModel>
	{
        public IEnumerable<KindModel> GetAll()
        {
            return  SQLService<KindModel>.GetAll("Kinds");
        }

        public KindModel GetById(int id, string t = "Id")
        {
            return SQLService<KindModel>.GetByNumber("Kinds", t, id);
        }

        public void Insert(KindModel entity)
        {
            string sql = $"insert into Kinds (CategoryId, Kind) values ({entity.CategoryId}, '{entity.Kind}') ";
            SQLService<KindModel>.UpdateInsertDelete(sql);
        }

        public void Update(KindModel entity)
        {
            string sql = $"update Kinds set CategoryId = {entity.CategoryId}, Kind = '{entity.Kind}' where  id = {entity.Id}";
            SQLService<KindModel>.UpdateInsertDelete(sql);
        }

        public void DeleteHard(KindModel entity)
        {
            string sql = $"delete from Kinds where Id = {entity.Id}";
            SQLService<CategoryModel>.UpdateInsertDelete(sql);
        }
    }
}
