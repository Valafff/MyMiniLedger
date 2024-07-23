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
        //public async Task<IEnumerable<CoinModel>> GetAllAsync()
        //{
        //	return await SQLService<CoinModel>.GetAllAsync("Coins");
        //}

        public IEnumerable<CoinModel> GetAll()
        {
            return SQLService<CoinModel>.GetAll("Coins");
        }

        //public async Task<CoinModel> GetByIdAsync(int id, string t = "Id")
        //{
        //	return await SQLService<CoinModel>.GetByNumber("Coins", t, id);
        //}

        public CoinModel GetById(int id, string t = "Id")
        {
            return SQLService<CoinModel>.GetByNumber("Coins", t, id);
        }

        //public async Task InsertAsync(CoinModel entity)
        //{
        //	string sql = $"insert into Coins (ShortName, FullName, CoinNotes) values (N'{entity.ShortName}', N'{entity.FullName}', N'{entity.CoinNotes}') ";
        //	await SQLService<CoinModel>.UpdateInsertDeleteAsync(sql);
        //}

        public void Insert(CoinModel entity)
        {
            //string sql = $"insert into Coins (ShortName, FullName, CoinNotes) values (N'{entity.ShortName}', N'{entity.FullName}', N'{entity.CoinNotes}') ";
            string sql = $"insert into Coins (ShortName, FullName, CoinNotes) values ('{entity.ShortName}', '{entity.FullName}', '{entity.CoinNotes}')";
            SQLService<CoinModel>.UpdateInsertDelete(sql);
        }

        //public async Task UpdateAsync(CoinModel entity)
        //{
        //	string sql = $"update Coins set ShortName = N'{entity.ShortName}', FullName = N'{entity.FullName}', CoinNotes = N'{entity.CoinNotes}' where  id = {entity.Id}";
        //	await SQLService<CoinModel>.UpdateInsertDeleteAsync(sql);
        //}

        public void Update(CoinModel entity)
        {
            //string sql = $"update Coins set ShortName = N'{entity.ShortName}', FullName = N'{entity.FullName}', CoinNotes = N'{entity.CoinNotes}' where  id = {entity.Id}";
            string sql = $"update Coins set ShortName = '{entity.ShortName}', FullName = '{entity.FullName}', CoinNotes = '{entity.CoinNotes}' where  id = {entity.Id}";
            SQLService<CoinModel>.UpdateInsertDelete(sql);
        }

        //public async Task DeleteHardAsync(CoinModel entity)
        //{
        //	string sql = $"delete from Coins where Id = {entity.Id}";
        //	await SQLService<CoinModel>.UpdateInsertDeleteAsync(sql);
        //}

        public void DeleteHard(CoinModel entity)
        {
            string sql = $"delete from Coins where Id = {entity.Id}";
            SQLService<CoinModel>.UpdateInsertDelete(sql);
        }
    }
}
