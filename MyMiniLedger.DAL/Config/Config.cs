using System.Text.Json;

namespace MyMiniLedger.DAL.Config
{
	public class Config
	{
        public string? ServerName { get; set; }
        public string? DBname { get; set; }
        public string? AuthenticationMethod { get; set; }
        public string? UserId { get; set; }
		public string? Password { get; set; }

        public override string ToString()
		{
			if (UserId != null && UserId != "" && Password != null && Password != "")
			{
				return $"Data Source={ServerName}; Initial Catalog={DBname}; User ID={UserId}; Password={Password};";
			}
			else if(AuthenticationMethod != null && AuthenticationMethod != "" && ServerName != null && DBname != null)
			{
				//return $"Data Source={ServerName}; Initial Catalog={DBname}; {AuthenticationMethod};";
				//return $"Server=(localdb)\\DB;Database=TestLedger;Integrated Security=True; Asynchronous Processing=True;";
				//return $"Server=(localdb)\\DB;Database=TestLedger;Integrated Security=True;";

				return @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestLedger;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
			}
			else
            {
				return "";
			}

        }
		//string connectionString = @"Data Source=(localdb)\DB; Initial Catalog=Library; Integrated Security=SSPI;";

		public static  Config? GetFromConfig(string path = "config.json")
		{
			using var fromFile = new FileStream(path, FileMode.Open, FileAccess.Read);
			var config = JsonSerializer.Deserialize<Config>(fromFile);
			return config;
		}
	}



}
