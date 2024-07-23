using MyMiniLedger.BLL.Models;
using MyMiniLedger.DAL.Interfaces;
using MyMiniLedger.DAL.Models;
using MyMiniLedger.DAL.SQL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.BLL.Context
{
    public class ListOfPositions
    {


        private readonly IReadAll<DAL.Models.PositionModel> _sourceForRead;
        private readonly IReadById<DAL.Models.PositionModel> _sourceForReadById;
        private readonly ICreate<DAL.Models.PositionModel> _sourceForInsert;
        private readonly IUpdate<DAL.Models.PositionModel> _sourceForUpdate;
        private readonly IDeleteHard<DAL.Models.PositionModel> _sourceForDelete;


        TableCategories tempCatTable = new TableCategories();
        TableKinds tempKindsTable = new TableKinds();
        TableStatuses tempStatusesTable = new TableStatuses();
        TableCoins tempCoinsTable = new TableCoins();

        public ListOfPositions()
        {
            _sourceForRead = new TablePositions();
            _sourceForReadById = new TablePositions();
            _sourceForInsert = new TablePositions();
            _sourceForUpdate = new TablePositions();
            _sourceForDelete = new TablePositions();

        }

        //Получение всех данных
        public IEnumerable<PositionBLLModel> GetAll()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
            IEnumerable<CategoryModel> tempCategories = tempCatTable.GetAll();
            IEnumerable<KindModel> tempKinds = tempKindsTable.GetAll();
            IEnumerable<StatusModel> tempStatuses = tempStatusesTable.GetAll();
            IEnumerable<CoinModel> tempCoins = tempCoinsTable.GetAll();

            IEnumerable<PositionModel> result = _sourceForRead.GetAll();

            List<PositionBLLModel> enumerable = new List<PositionBLLModel>();

            foreach (var item in result)
            {
                enumerable.Add(Mappers.MapperBL.MapPositionDALToPositionBLL(item, tempKinds, tempCoins, tempStatuses, tempCategories));
            }
            return enumerable.AsEnumerable();
        }

        //Получение всех позиций по ZeroParentKey
        public IEnumerable<PositionBLLModel> GetAllByZeroParent(int _zeroParrentKey)
        {
            IEnumerable<CategoryModel> tempCategories = tempCatTable.GetAll();
            IEnumerable<KindModel> tempKinds = tempKindsTable.GetAll();
            IEnumerable<StatusModel> tempStatuses = tempStatusesTable.GetAll();
            IEnumerable<CoinModel> tempCoins = tempCoinsTable.GetAll();

            IEnumerable<PositionModel> result = _sourceForRead.GetAll();

            List<PositionBLLModel> enumerable = new List<PositionBLLModel>();

            foreach (var item in result)
            {
                PositionBLLModel temp = new PositionBLLModel();
                temp = Mappers.MapperBL.MapPositionDALToPositionBLL(item, tempKinds, tempCoins, tempStatuses, tempCategories);
                if (temp.additionalPositionDataBLL.ZeroParrentKey == _zeroParrentKey)
                {
                    enumerable.Add(temp);
                }
            }
            return enumerable.AsEnumerable();
        }

        //Получение данных по Id
        public PositionBLLModel GetById(int id, string t = "Id")
        {
            IEnumerable<CategoryModel> tempCategories = tempCatTable.GetAll();
            IEnumerable<KindModel> tempKinds = tempKindsTable.GetAll();
            IEnumerable<StatusModel> tempStatuses = tempStatusesTable.GetAll();
            IEnumerable<CoinModel> tempCoins = tempCoinsTable.GetAll();
            PositionModel result = _sourceForReadById.GetById(id, t);
            return Mappers.MapperBL.MapPositionDALToPositionBLL(result, tempKinds, tempCoins, tempStatuses, tempCategories);
        }

        //Вставка данных с учетом максимальной позиции. Если позиция не передается, она расчитывается автоматически
        public void Insert(PositionBLLModel entity, int posKey = 0)
        {
            int max;
            var positions = GetAll();
            if (positions.Count() > 0)
            {
                max = positions.Max(maxPos => maxPos.PositionKey);
            }
            else
            {
                max = 0;
            }

            if (posKey == 0)
            {
                entity.PositionKey = max + 1;
            }
            //Расчет сальдо
            entity.Saldo = entity.Income - entity.Expense;
            //Запись в БД
            _sourceForInsert.Insert(Mappers.MapperBL.MapPositionBLLToPositionDAL(entity));
        }

        //Изменение данных
        public void Update(PositionBLLModel entity)
        {
            //Расчет сальдо
            entity.Saldo = entity.Income - entity.Expense;
            //Запись в БД
            _sourceForUpdate.Update(Mappers.MapperBL.MapPositionBLLToPositionDAL(entity));
        }

        //Реализовать удаление с записью Deleted

        //      //Полное удаление
        //      public async Task DeleteAsync(PositionBLLModel entity)
        //{
        //	await _sourceForDelete.DeleteHardAsync(Mappers.MapperBL.MapPositionBLLToPositionDAL(entity));
        //}

        //Полное удаление
        public void Delete(PositionBLLModel entity)
        {
            _sourceForDelete.DeleteHard(Mappers.MapperBL.MapPositionBLLToPositionDAL(entity));
        }
    }
}
