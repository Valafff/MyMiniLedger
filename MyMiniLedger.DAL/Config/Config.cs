using System.Text.Json;

namespace MyMiniLedger.DAL.Config
{
	public class Config
	{
        public string? DataSource { get; set; }
		//Изменения 19.08.24
		public double DefaultFee { get; set; }
		public string? DefaultKindToBuy { get; set; }
		public string? DefaultKindToSell { get; set; }

		public override string ToString()
        {
            return DataSource is null ? "" : $"Data Source={DataSource}";
        }

        public static  Config? GetFromConfig(string path = "config.json")
		{
			using var fromFile = new FileStream(path, FileMode.Open, FileAccess.Read);
			var config = JsonSerializer.Deserialize<Config>(fromFile);
			return config;
		}
	}
}
