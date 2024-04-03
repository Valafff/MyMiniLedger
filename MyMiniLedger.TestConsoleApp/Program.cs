//#define ConnectionStringTest
#define ServicesTest

using MyMiniLedger.DAL.Config;
using MyMiniLedger.DAL.Models;
using MyMiniLedger.DAL.Services;


#if ConnectionStringTest

string res = Config.GetFromConfig(@"Resources\config.json").ToString();
Console.WriteLine(res);

#endif

#if ServicesTest

var tableCategories = await SQLService<CategoryModel>.GetAllAsync("Categories");
foreach (var category in tableCategories)
{
    Console.WriteLine(category.Category);
}
#endif
