using Dapper;
using Microsoft.Data.SqlClient;

namespace MyMiniLedger.DAL.Services
{
	public static class SQLService<T>
	{
		private static readonly SqlConnection _DBconnection;
		//Конструктор статического класса
		static SQLService()
		{
			{
				string connString = Config.Config.GetFromConfig("config.json").ToString();
				if (string.IsNullOrEmpty(connString))
				{
					throw new ArgumentException($"Ошибка конфигурационного файла config.json");
				}
				else
				{
					_DBconnection = new SqlConnection(connString);
				}
			}
		}

		//Получить всё из таблицы
		public static async Task<IEnumerable<T>> GetAllAsync(string tableName)
		{
			await _DBconnection.OpenAsync();
			string sql = $"select * from {tableName}";
			IEnumerable<T> result = await _DBconnection.QueryAsync<T>(sql);
			await _DBconnection.CloseAsync();
			return result;
		}

		//Получить значение по ID
		public static async Task<T> GetByNumber(string tableName, string columnName, int key)
		{
			await _DBconnection.OpenAsync();
			string sql = $"select * from {tableName} where {columnName} = {key}";
			T result = await _DBconnection.QuerySingleAsync<T>(sql);
			await _DBconnection.CloseAsync();
			return result;
		}

		// Обновление и добавление данных
		public static async Task UpdateAndInsertAsync(string sql)
		{
			await _DBconnection.OpenAsync();
			await _DBconnection.ExecuteAsync(sql);
			await _DBconnection.CloseAsync();
		}






		//private readonly SqlConnection _DBconnection;
		////Подумать как избавиться от постоянного чтения из конфига
		//public SQLService(string path = "config.json")
		//{
		//	string connString = Config.Config.GetFromConfig(path).ToString();
		//	if (string.IsNullOrEmpty(connString))
		//	{
		//		throw new ArgumentException($"Ошибка конфигурационного файла {path}");
		//	}
		//	else
		//	{
		//		_DBconnection = new SqlConnection(connString);
		//	}
		//}

		////Получить всё из таблицы
		//public async Task<IEnumerable<T>> GetAllAsync(string tableName)
		//{
		//	await _DBconnection.OpenAsync();
		//	string sql = $"select * from {tableName}";
		//	IEnumerable<T> result = await _DBconnection.QueryAsync<T>(sql);
		//	await _DBconnection.CloseAsync();
		//	return result;
		//}
	}
}
