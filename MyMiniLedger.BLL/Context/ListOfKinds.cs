using MyMiniLedger.BLL.Models;
using MyMiniLedger.DAL.Interfaces;
using MyMiniLedger.DAL.Models;
using MyMiniLedger.DAL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.BLL.Context
{
	public class ListOfKinds
	{

		private readonly IReadAll<DAL.Models.KindModel> _sourceForRead;
		private readonly IReadById<DAL.Models.KindModel> _sourceForReadById;
		private readonly ICreate<DAL.Models.KindModel> _sourceForInsert;
		private readonly IUpdate<DAL.Models.KindModel> _sourceForUpdate;
		private readonly IDeleteHard<DAL.Models.KindModel> _sourceForDelete;

		TableCategories tempCatTable = new TableCategories();

		public ListOfKinds()
		{
			_sourceForRead = new TableKinds();
			_sourceForReadById = new TableKinds();
			_sourceForInsert = new TableKinds();
			_sourceForUpdate = new TableKinds();
			_sourceForDelete = new TableKinds();
		}

        //Получение всех данных
        public IEnumerable<KindBLLModel> GetAll()
        {

            IEnumerable<CategoryModel> temp = tempCatTable.GetAll();
            IEnumerable<KindModel> result = _sourceForRead.GetAll();

            List<KindBLLModel> tempResult = new List<KindBLLModel>();
            foreach (var item in result)
            {
                tempResult.Add(Mappers.MapperBL.MapKindDALToKindBLL(item, temp));
            }
            return tempResult;
        }

        //Получение данных по Id
        public KindBLLModel GetById(int id, string t = "Id")
        {
            IEnumerable<CategoryModel> temp =  tempCatTable.GetAll();
            KindModel result = _sourceForReadById.GetById(id, t);
            return Mappers.MapperBL.MapKindDALToKindBLL(result, temp);
        }

        //Вставка данных
        public void Insert(KindBLLModel entity)
        {
            _sourceForInsert.Insert(Mappers.MapperBL.MapKindBLLToKindDAL(entity));
        }

        //Изменение данных
        public void Update(KindBLLModel entity)
        {
            _sourceForUpdate.Update(Mappers.MapperBL.MapKindBLLToKindDAL(entity));
        }

        //Реализовать удаление с записью Deleted

        //      //Реализовать полное удаление
        //      public async Task DeleteAsync(KindBLLModel entity)
        //{
        //	await _sourceForDelete.DeleteHardAsync(Mappers.MapperBL.MapKindBLLToKindDAL(entity));
        //}

        //Реализовать полное удаление
        public void Delete(KindBLLModel entity)
        {
            _sourceForDelete.DeleteHard(Mappers.MapperBL.MapKindBLLToKindDAL(entity));
        }
    }
}
