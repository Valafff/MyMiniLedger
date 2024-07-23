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
        //public async Task<IEnumerable<KindModel>> GetAllAsync()
        //{
        //	return await SQLService<KindModel>.GetAll("Kinds");
        //}

        public IEnumerable<KindModel> GetAll()
        {
            return  SQLService<KindModel>.GetAll("Kinds");
        }

        //public async Task<KindModel> GetByIdAsync(int id, string t = "Id")
        //{
        //	return await SQLService<KindModel>.GetByNumber("Kinds", t, id);
        //}

        public KindModel GetById(int id, string t = "Id")
        {
            return SQLService<KindModel>.GetByNumber("Kinds", t, id);
        }

        //public async Task InsertAsync(KindModel entity)
        //{
        //	string sql = $"insert into Kinds (CategoryId, Kind) values ({entity.CategoryId}, N'{entity.Kind}') ";
        //	await SQLService<KindModel>.UpdateInsertDeleteAsync(sql);
        //}

        public void Insert(KindModel entity)
        {
            //string sql = $"insert into Kinds (CategoryId, Kind) values ({entity.CategoryId}, N'{entity.Kind}') ";
            string sql = $"insert into Kinds (CategoryId, Kind) values ({entity.CategoryId}, '{entity.Kind}') ";
            SQLService<KindModel>.UpdateInsertDelete(sql);
        }

        //public async Task UpdateAsync(KindModel entity)
        //{
        //	string sql = $"update Kinds set CategoryId = {entity.CategoryId}, Kind = N'{entity.Kind}' where  id = {entity.Id}";
        //	await SQLService<KindModel>.UpdateInsertDeleteAsync(sql);
        //}

        public void Update(KindModel entity)
        {
            //string sql = $"update Kinds set CategoryId = {entity.CategoryId}, Kind = N'{entity.Kind}' where  id = {entity.Id}";
            string sql = $"update Kinds set CategoryId = {entity.CategoryId}, Kind = '{entity.Kind}' where  id = {entity.Id}";
            SQLService<KindModel>.UpdateInsertDelete(sql);
        }

        //public async Task DeleteHardAsync(KindModel entity)
        //{
        //	string sql = $"delete from Kinds where Id = {entity.Id}";
        //	await SQLService<CategoryModel>.UpdateInsertDeleteAsync(sql);
        //}

        public void DeleteHard(KindModel entity)
        {
            string sql = $"delete from Kinds where Id = {entity.Id}";
            SQLService<CategoryModel>.UpdateInsertDelete(sql);
        }
    }
}
