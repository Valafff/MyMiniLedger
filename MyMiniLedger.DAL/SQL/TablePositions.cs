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
    public class TablePositions : ICreate<PositionModel>, IUpdate<PositionModel>, IReadAll<PositionModel>, IReadById<PositionModel>, IDeleteHard<PositionModel>
    {
        //Для валидации пароля
		public IEnumerable<PositionModel> GetAllForPass()
		{
			return SQLService<PositionModel>.GetAllForPass("Positions");
		}

		public IEnumerable<PositionModel> GetAll()
        {
            return SQLService<PositionModel>.GetAll("Positions");
        }
        public PositionModel GetById(int id, string t = "Id")
        {
            return SQLService<PositionModel>.GetByNumber("Positions", t, id);
        }

        public void Insert(PositionModel entity)
        {
            string? _additionalPositionData;
            if (entity.AdditionalPositionData == null || entity.AdditionalPositionData == "")
            {
                _additionalPositionData = "NULL";
            }
            else
            {
                _additionalPositionData = $"'{entity.AdditionalPositionData}'";
            }

            //Нужно для победы над разделителями dot и comma!!!!! https://qna.habr.com/q/1128486
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            string sql = $"insert into Positions (PositionKey, OpenDate, CloseDate, KindId, Income, Expense, Saldo, CoinId, StatusId, Tag, Notes, AdditionalPositionData)" +
            $" values ({entity.PositionKey}, '{entity.OpenDate}', '{entity.CloseDate}', {entity.KindId}, {entity.Income}, {entity.Expense}, {entity.Saldo}," +
            $" {entity.CoinId}, {entity.StatusId}, '{entity.Tag}', '{entity.Notes}', {_additionalPositionData}) ";
            SQLService<KindModel>.UpdateInsertDelete(sql);
        }

        public void Update(PositionModel entity)
        {
            string? _additionalPositionData;
            if (entity.AdditionalPositionData == null || entity.AdditionalPositionData == "")
            {
                _additionalPositionData = "NULL";
            }
            else
            {
                _additionalPositionData = $"'{entity.AdditionalPositionData}'";
            }
            //Нужно для победы над разделителями dot и comma!!!!! https://qna.habr.com/q/1128486
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            string sql = $"update Positions set PositionKey = {entity.PositionKey}, OpenDate = '{entity.OpenDate}', CloseDate = '{entity.CloseDate}', KindId = {entity.KindId}," +
            $" Income = {entity.Income}, Expense = {entity.Expense}, Saldo = {entity.Saldo}, CoinId = {entity.CoinId}, StatusId = {entity.StatusId}," +
            $" Tag = '{entity.Tag}', Notes = '{entity.Notes}', AdditionalPositionData = {_additionalPositionData} where  id = {entity.Id}";
            SQLService<PositionModel>.UpdateInsertDelete(sql);
        }

        public void DeleteHard(PositionModel entity)
        {
            string sql = $"delete from Positions where Id = {entity.Id}";
            SQLService<PositionModel>.UpdateInsertDelete(sql);
        }




    }
}
