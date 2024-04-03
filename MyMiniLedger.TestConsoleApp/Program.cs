using MyMiniLedger.DAL.Config;

string res = Config.GetFromConfig(@"Resources\config.json").ToString();

Console.WriteLine(res);