using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.DAL.Services
{
	public static class DataConfig
	{
		public static SqlConnection _DBconnection;
		public static void Init(string path)
		{
			string connString = Config.Config.GetFromConfig(path).ToString();
			if (string.IsNullOrEmpty(connString))
			{
				throw new ArgumentException($"Ошибка конфигурационного файла config.json");
			}
			else
			{
				if (_DBconnection == null)
				{
					_DBconnection = new SqlConnection(connString);
				}
			}
		}
	}
}
