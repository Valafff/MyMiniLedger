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
			if (UserId != null  && Password != null )
			{
				return $"Data Source={ServerName}; Initial Catalog={DBname}; {AuthenticationMethod}; User ID={UserId}; Password={Password};";
				////Работает
				//return @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestLedger;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
				////Работает
				//return @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestLedger;";
				////Работает
				//return @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestLedger; Trusted_Connection=True";		
			}
			else if(AuthenticationMethod != null && AuthenticationMethod != "" && ServerName != null && DBname != null)
			{
				return $"Data Source={ServerName}; Initial Catalog={DBname}; {AuthenticationMethod};";			
			}
			else
            {
				return "";
			}

        }

		public static  Config? GetFromConfig(string path = "config.json")
		{
			using var fromFile = new FileStream(path, FileMode.Open, FileAccess.Read);
			var config = JsonSerializer.Deserialize<Config>(fromFile);
			return config;
		}
	}



}
