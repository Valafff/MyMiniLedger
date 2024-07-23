using Dapper;
//using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace MyMiniLedger.DAL.Services
{
	public static class SQLService<T>
	{
		private static SqliteConnection _DBconnection = DataConfig._DBconnection;

        //Получить всё из таблицы
        public static  IEnumerable<T> GetAll(string tableName)
        {
            _DBconnection.Open();
            string sql = $"select * from {tableName}";
            IEnumerable<T> result =  _DBconnection.Query<T>(sql);
            _DBconnection.Close();
            return result;
        }
    
        //Получить значение по ID
        public static T GetByNumber(string tableName, string columnName, int key)
        {
            _DBconnection.Open();
            string sql = $"select * from {tableName} where {columnName} = {key}";
            T result = _DBconnection.QuerySingle<T>(sql);
            _DBconnection.Close();
            return result;
        }

        // Обновление и добавление данных
        public static void UpdateInsertDelete(string sql)
        {
            _DBconnection.Open();
            _DBconnection.Execute(sql);
            _DBconnection.Close();
        }

    }
}
