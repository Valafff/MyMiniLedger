using MyMiniLedger.DAL.Config;

namespace MyMiniLedger.DAL.Test
{
	public class ConfigTest
	{
		const string expected = "Data Source=(localdb)\\DB; Initial Catalog=TestLedger; Integrated Security=SSPI;";
		
		[Fact]
		public void ConfigString()
		{


			var config = new Config.Config()
			{
				ServerName = "(localdb)\\DB",
				DBname = "TestLedger",
				AuthenticationMethod = "Integrated Security=SSPI",
				UserId = "",
				Password = ""
			};
			var actual = config.ToString();
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void GetFromConfigTest()
		{
			var config = Config.Config.GetFromConfig();
			var actual = config!.ToString();

			Assert.Equal(expected, actual);
		}

		[Fact]
		public void GetFromConfigTestNegative()
		{
			var config = Config.Config.GetFromConfig("bad_config.json");
			Assert.Null(config?.ServerName);
			Assert.Null(config?.DBname);
			Assert.Null(config?.AuthenticationMethod);
			Assert.Null(config?.UserId);
			Assert.Null(config?.Password);
		}
	}
}