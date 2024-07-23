using MyMiniLedger.BLL.Models;
using MyMiniLedger.DAL.Interfaces;
using MyMiniLedger.DAL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.BLL.Context
{
    public class ListOfCoins
    {

        private readonly IReadAll<DAL.Models.CoinModel> _sourceForRead;
        private readonly IReadById<DAL.Models.CoinModel> _sourceForReadById;
        private readonly ICreate<DAL.Models.CoinModel> _sourceForInsert;
        private readonly IUpdate<DAL.Models.CoinModel> _sourceForUpdate;
        private readonly IDeleteHard<DAL.Models.CoinModel> _sourceForDelete;

        public ListOfCoins()
        {
            _sourceForRead = new TableCoins();
            _sourceForReadById = new TableCoins();
            _sourceForInsert = new TableCoins();
            _sourceForUpdate = new TableCoins();
            _sourceForDelete = new TableCoins();
        }

        //Получение всех данных
        public IEnumerable<CoinBLLModel> GetAll()
        {
            var result = _sourceForRead.GetAll();
            var temp = new List<CoinBLLModel>();
            foreach (var item in result)
            {
                temp.Add(Mappers.MapperBL.MapCoinDALToCoinBLL(item));
            }
            return temp;
        }

        //Получение данных по Id
        public CoinBLLModel GetById(int id, string t = "Id")
        {
            var result = _sourceForReadById.GetById(id, t);
            return Mappers.MapperBL.MapCoinDALToCoinBLL(result);
        }

        //Вставка данных
        public void Insert(CoinBLLModel entity)
        {
            _sourceForInsert.Insert(Mappers.MapperBL.MapCoinBLLToCoinDAL(entity));
        }

        //Изменение данных
        public void Update(CoinBLLModel entity)
        {
            _sourceForUpdate.Update(Mappers.MapperBL.MapCoinBLLToCoinDAL(entity));
        }

        //Реализовать удаление с записью Deleted
        //      //Реализовать полное удаление
        //      public async Task DeleteAsync(CoinBLLModel entity)
        //{
        //	await _sourceForDelete.DeleteHardAsync(Mappers.MapperBL.MapCoinBLLToCoinDAL(entity));
        //}

        //Реализовать полное удаление
        public void Delete(CoinBLLModel entity)
        {
            _sourceForDelete.DeleteHard(Mappers.MapperBL.MapCoinBLLToCoinDAL(entity));
        }
    }
}
