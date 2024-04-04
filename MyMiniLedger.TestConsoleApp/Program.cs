//#define ConnectionStringTest
//#define ServicesTestReadAll
//#define ServicesTestReadById
#define ServicesTestInsert
//#define ServicesTestUpdate

using Azure;
using MyMiniLedger.DAL.Config;
using MyMiniLedger.DAL.Models;
using MyMiniLedger.DAL.Services;
using MyMiniLedger.DAL.SQL;
using static Azure.Core.HttpHeader;

//Обернуть в на вышестоящие уровни
DataConfig.Init("config.json");

TableCategories tc = new TableCategories();
TableCoins tcoins = new TableCoins();
TableKinds tkinds = new TableKinds();
TablePositions tpos = new TablePositions();




#if ConnectionStringTest

string res = Config.GetFromConfig(@"Resources\config.json").ToString();
Console.WriteLine(res);

#endif
#if ServicesTestReadAll
//Категории
//var data1 = tc.GetAllAsync().Result;
////var tableCategories = await SQLService<CategoryModel>.GetAllAsync("Categories");
//foreach (var category in data1)
//{
//	Console.WriteLine(category.Category);
//}

////Монеты
//var data2 = tcoins.GetAllAsync().Result;
////var tableCategories = await SQLService<CategoryModel>.GetAllAsync("Categories");
//foreach (var category in data2)
//{
//	Console.WriteLine(category.FullName + " " + category.ShortName + " " + category.CoinNotes);
//}

//var data3 = tkinds.GetAllAsync().Result;
//foreach (var kind in data3)
//{
//	Console.WriteLine(kind.CategoryId + " " + kind.Kind + " " + kind.Id);
//}

//var data4 = tpos.GetAllAsync().Result;
//foreach (var pos in data4)
//{
//	Console.WriteLine($"{pos.Id}   {pos.PositionKey}	{pos.OpenDate}	{pos.CloseDate}   {pos.KindId}	{pos.Income}   {pos.Expense}   {pos.Saldo}   {pos.CoinId}  {pos.StatusId}   {pos.Tag}	{pos.Notes}");
//}

#endif
#if ServicesTestReadById
//var data = tc.GetByIdAsync(1).Result;
////var tableCategories = await SQLService<CategoryModel>.GetAllAsync("Categories");
//    Console.WriteLine(data.Category);

//var data2 = tcoins.GetByIdAsync(1).Result;
//Console.WriteLine(data2.CoinNotes);

//var data3 = tkinds.GetByIdAsync(1).Result;
//Console.WriteLine(data3.CategoryId + " " + data3.Kind);

var data4 = tpos.GetByIdAsync(1).Result;
Console.WriteLine($"{data4.Id}   {data4.PositionKey}	{data4.OpenDate}	{data4.CloseDate}   {data4.KindId}	{data4.Income}   {data4.Expense}   {data4.Saldo}   {data4.CoinId}  {data4.StatusId}   {data4.Tag}	{data4.Notes}");

#endif
#if ServicesTestInsert
//var testcategory = new CategoryModel() { Category = "ТестВставки2" };
//await tc.InsertAsync(testcategory);

//var testcoin = new CoinModel() { ShortName = "Kaspa", FullName = "КАСПА", CoinNotes = "Цифровое серебро может быть"};
//await tcoins.InsertAsync(testcoin);


//var data = tc.GetAllAsync().Result;
//foreach (var category in data)
//{
//	Console.WriteLine(category.Category);
//}

//var data2 = tcoins.GetAllAsync().Result;
//foreach (var category in data2)
//{
//	Console.WriteLine(category.FullName + " " + category.ShortName + " " + category.CoinNotes);
//}

//var testkind = new KindModel() { CategoryId = 10, Kind = "ТЕСТ" };
//await tkinds.InsertAsync(testkind);

//var data3 = tkinds.GetAllAsync().Result;
//foreach (var category in data3)
//{
//	Console.WriteLine(category.CategoryId + " " + category.Kind);
//}

var testpos = new PositionModel() { PositionKey = 100, OpenDate = DateTime.Now, CloseDate = DateTime.Now, KindId = 1, Income = 201.5 , Expense = 0, Saldo = 200, CoinId = 2, StatusId = 4, Tag = "Тест", Notes = "Тест" };
await tpos.InsertAsync(testpos);

#endif

#if ServicesTestUpdate
//var testcategory = new CategoryModel() { Category = "Тест Изменения Записи!!!!!!!!!!!!!!!!", Id = 18 };
//await tc.UpdateAsync(testcategory);

//var testcoin = new CoinModel() { ShortName = "Kaspa", FullName = "КАСПА", CoinNotes = "Цифровым серебром БЫТЬ!", Id = 1002 };
//await tcoins.UpdateAsync(testcoin);

var testKIND = new KindModel() { CategoryId = 9, Kind = "Тест", Id = 1020 };
await tkinds.UpdateAsync(testKIND);

#endif
