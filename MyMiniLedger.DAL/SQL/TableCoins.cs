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
    public class TableCoins : ICreate<CoinModel>, IUpdate<CoinModel>, IReadAll<CoinModel>, IReadById<CoinModel>, IDeleteHard<CoinModel>
    {
        public IEnumerable<CoinModel> GetAll()
        {
            return SQLService<CoinModel>.GetAll("Coins");
        }

        public CoinModel GetById(int id, string t = "Id")
        {
            return SQLService<CoinModel>.GetByNumber("Coins", t, id);
        }

        public void Insert(CoinModel entity)
        {
            string sql = $"insert into Coins (ShortName, FullName, CoinNotes) values ('{entity.ShortName}', '{entity.FullName}', '{entity.CoinNotes}')";
            SQLService<CoinModel>.UpdateInsertDelete(sql);
        }

        public void Update(CoinModel entity)
        {
            string sql = $"update Coins set ShortName = '{entity.ShortName}', FullName = '{entity.FullName}', CoinNotes = '{entity.CoinNotes}' where  id = {entity.Id}";
            SQLService<CoinModel>.UpdateInsertDelete(sql);
        }

        public void DeleteHard(CoinModel entity)
        {
            string sql = $"delete from Coins where Id = {entity.Id}";
            SQLService<CoinModel>.UpdateInsertDelete(sql);
        }
    }
}
