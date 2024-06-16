﻿using MyMiniLedger.DAL.Interfaces;
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

		public async Task<IEnumerable<PositionModel>> GetAllAsync()
		{
			return await SQLService<PositionModel>.GetAllAsync("Positions");
		}

		public async Task<PositionModel> GetByIdAsync(int id, string t = "Id")
		{
			return await SQLService<PositionModel>.GetByNumber("Positions", t, id);
		}

		public async Task InsertAsync(PositionModel entity)
		{
			string? _additionalPositionData;
			if (entity.AdditionalPositionData == null || entity.AdditionalPositionData == "")
			{
				_additionalPositionData = "NULL";
			}
			else
			{
				_additionalPositionData = $"N'{entity.AdditionalPositionData}'";
			}
			//Нужно для победы над разделителями dot и comma!!!!! https://qna.habr.com/q/1128486
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			string sql = $"insert into Positions (PositionKey, OpenDate, CloseDate, KindId, Income, Expense, Saldo, CoinId, StatusId, Tag, Notes, AdditionalPositionData)" +
				$" values ({entity.PositionKey}, N'{entity.OpenDate}', N'{entity.CloseDate}', {entity.KindId}, {entity.Income}, {entity.Expense}, {entity.Saldo}," +
				$" {entity.CoinId}, {entity.StatusId}, N'{entity.Tag}', N'{entity.Notes}', {_additionalPositionData}) ";
			await SQLService<KindModel>.UpdateInsertDeleteAsync(sql);
		}

		public async Task UpdateAsync(PositionModel entity)
		{
			string? _additionalPositionData;
			if (entity.AdditionalPositionData == null || entity.AdditionalPositionData == "")
			{
				_additionalPositionData = "NULL";
			}
			else
			{
				_additionalPositionData = $"N'{entity.AdditionalPositionData}'";
			}
			//Нужно для победы над разделителями dot и comma!!!!! https://qna.habr.com/q/1128486
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			string sql = $"update Positions set PositionKey = {entity.PositionKey}, OpenDate = N'{entity.OpenDate}', CloseDate = N'{entity.CloseDate}', KindId = {entity.KindId}," +
				$" Income = {entity.Income}, Expense = {entity.Expense}, Saldo = {entity.Saldo}, CoinId = {entity.CoinId}, StatusId = {entity.StatusId}," +
				$" Tag = N'{entity.Tag}', Notes = N'{entity.Notes}', AdditionalPositionData = {_additionalPositionData} where  id = {entity.Id}";
			await SQLService<PositionModel>.UpdateInsertDeleteAsync(sql);
		}

		public async Task DeleteHardAsync(PositionModel entity)
		{
			string sql = $"delete from Positions where Id = {entity.Id}";
			await SQLService<PositionModel>.UpdateInsertDeleteAsync(sql);
		}
	}
}