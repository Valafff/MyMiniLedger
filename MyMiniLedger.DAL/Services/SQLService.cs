using Dapper;
using Microsoft.Data.SqlClient;

namespace MyMiniLedger.DAL.Services
{
	public static class SQLService<T>
	{
		private static SqlConnection _DBconnection = DataConfig._DBconnection;


		////Асинхронность

		////Получить всё из таблицы
		//public static async Task<IEnumerable<T>> GetAllAsync(string tableName)
		//{
		//	await _DBconnection.OpenAsync();
		//	string sql = $"select * from {tableName}";
		//	IEnumerable<T> result = await _DBconnection.QueryAsync<T>(sql);
		//	await _DBconnection.CloseAsync();
		//	return result;
		//}


		//Рабочий вариант
		//Получить всё из таблицы
		public static async Task<IEnumerable<T>> GetAllAsync(string tableName)
		{
			_DBconnection.OpenAsync().Wait();
			string sql = $"select * from {tableName}";
			IEnumerable<T> result = _DBconnection.QueryAsync<T>(sql).Result;
			_DBconnection.CloseAsync().Wait();
			return result;
		}

		////Нерабочий вариант на SQL
		////Получить значение по ID
		//public static async Task<T> GetByNumber(string tableName, string columnName, int key)
		//{
		//	await _DBconnection.OpenAsync();
		//	string sql = $"select * from {tableName} where {columnName} = {key}";
		//	T result = await _DBconnection.QuerySingleAsync<T>(sql);
		//	await _DBconnection.CloseAsync();
		//	return result;
		//}

		//Получить значение по ID
		public static async Task<T> GetByNumber(string tableName, string columnName, int key)
		{
			_DBconnection.OpenAsync().Wait();
			string sql = $"select * from {tableName} where {columnName} = {key}";
			T result = _DBconnection.QuerySingleAsync<T>(sql).Result;
			_DBconnection.CloseAsync().Wait();
			return result;
		}

		//////Нерабочий вариант на SQL
		//// Обновление и добавление данных
		//public static async Task UpdateAndInsertAsync(string sql)
		//{
		//	await _DBconnection.OpenAsync();
		//	await _DBconnection.ExecuteAsync(sql);
		//	await _DBconnection.CloseAsync();
		//}

		// Обновление и добавление данных
		public static async Task UpdateInsertDeleteAsync(string sql)
		{
			 _DBconnection.OpenAsync().Wait();
			 _DBconnection.ExecuteAsync(sql).Wait();
			 _DBconnection.CloseAsync().Wait();
		}

	}
}
