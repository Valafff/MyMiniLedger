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
    public class TableCategories : ICreate<CategoryModel>, IUpdate<CategoryModel>, IReadAll<CategoryModel>, IReadById<CategoryModel>, IDeleteHard<CategoryModel>
    {

        public IEnumerable<CategoryModel> GetAll()
        {
            return SQLService<CategoryModel>.GetAll("Categories");
        }

        //public async Task<CategoryModel> GetByIdAsync(int id, string t = "Id")
        //{
        //	return await SQLService<CategoryModel>.GetByNumber("Categories", t, id);
        //}
        public CategoryModel GetById(int id, string t = "Id")
        {
            return  SQLService<CategoryModel>.GetByNumber("Categories", t, id);
        }

        //public async Task InsertAsync(CategoryModel entity)
        //{
        //	string sql = $"insert into Categories (Category) values (N'{entity.Category}') ";
        //	await SQLService<CategoryModel>.UpdateInsertDeleteAsync(sql);
        //}

        public void  Insert(CategoryModel entity)
        {
            //string sql = $"insert into Categories (Category) values (N'{entity.Category}') ";
            string sql = $"insert into Categories (Category) values ('{entity.Category}')";
            SQLService<CategoryModel>.UpdateInsertDelete(sql);
        }

        //public async Task UpdateAsync(CategoryModel entity)
        //{
        //	string sql = $"update Categories set Category = N'{entity.Category}' where  id = {entity.Id}";
        //	await SQLService<CategoryModel>.UpdateInsertDeleteAsync(sql);
        //}

        public void Update(CategoryModel entity)
        {
            //string sql = $"update Categories set Category = N'{entity.Category}' where  id = {entity.Id}";
            string sql = $"update Categories set Category = '{entity.Category}' where  id = {entity.Id}";
            SQLService<CategoryModel>.UpdateInsertDelete(sql);
        }

        //public async Task DeleteHardAsync(CategoryModel entity)
        //{
        //	string sql = $"delete from Categories where Id = {entity.Id}";
        //	await SQLService<CategoryModel>.UpdateInsertDeleteAsync(sql);
        //}

        public void DeleteHard(CategoryModel entity)
        {
            string sql = $"delete from Categories where Id = {entity.Id}";
            SQLService<CategoryModel>.UpdateInsertDelete(sql);
        }

    }
}
