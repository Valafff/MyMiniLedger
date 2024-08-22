using Microsoft.Data.Sqlite;
using MyMiniLedger.DAL.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.DAL.Services
{
    public static class DataConfig
    {
        public static SqliteConnection _DBconnection;
        public static void Init(string path, string _pass)
        {
            string connString = Config.Config.GetFromConfig(path).ToString();

			var modConnString = new SqliteConnectionStringBuilder(connString)
			{
				Mode = SqliteOpenMode.ReadWriteCreate,
				Password = _pass
			}.ToString();
		
            if (string.IsNullOrEmpty(connString))
            {
                throw new ArgumentException($"Ошибка конфигурационного файла config.json");
            }
            else
            {
                if (_DBconnection == null)
                {
                    _DBconnection = new SqliteConnection(modConnString);
                }
            }
        }

        public static void ResetConnection()
        {
            if (_DBconnection != null)
            {
                _DBconnection.Dispose();
                _DBconnection = null;
            }
        }
	}
}
