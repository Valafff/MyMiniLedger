//#define ConnectionStringTest
//#define ServicesTestReadAll
//#define ServicesTestReadById
#define ServicesTestInsert
//#define ServicesTestUpdate
//#define TestBLLModel
#define OnOffDAL
//#define TestBLLContext

#if OnOffDAL
using Dapper;
using ImportedClasses;
using MyMiniLedger.BLL.Context;
using MyMiniLedger.BLL.Mappers;
using MyMiniLedger.BLL.Models;
using MyMiniLedger.DAL.Config;
using MyMiniLedger.DAL.Models;
using MyMiniLedger.DAL.Services;
using MyMiniLedger.DAL.SQL;

//Обернуть на вышестоящие уровни
DataConfig.Init("config.json", "123");

//подключение к старой БД
DB_Provider provider = new DB_Provider();
List<impoetedPosition> oldPositions = new List<impoetedPosition>();
provider.openConnection();
oldPositions = provider.GetAllPositions();
provider.closeConnection();

//Сервисы
TableCategories tc = new TableCategories();
TableCoins tcoins = new TableCoins();
TableKinds tkinds = new TableKinds();
TablePositions tpos = new TablePositions();
TableStatuses tst = new TableStatuses();

List<CategoryModel> listCategories = new List<CategoryModel>();
List<CoinModel> listCoins = new List<CoinModel>();
List<KindModel> listKinds = new List<KindModel>();
List<PositionModel> listPositions = new List<PositionModel>();
List<StatusModel> listStatuses = new List<StatusModel>();

foreach (var oldPos in oldPositions)
{
    if (!listCategories.Any(c => c.Category == oldPos.category))
    {
        CategoryModel temp = new CategoryModel() { Category = oldPos.category };
        listCategories.Add(temp);
        tc.Insert(temp);
    }
    if (!listCoins.Any(c => c.ShortName == oldPos.currCoin))
    {
        CoinModel temp = new CoinModel() { ShortName = oldPos.currCoin };
        listCoins.Add(temp);
        tcoins.Insert(temp);
    }
    if (!listKinds.Any(k => k.Kind == oldPos.kind))
    {
        List<CategoryModel> tempCats = tc.GetAll().ToList();
        KindModel temp = new KindModel() { CategoryId = tempCats.First(c => c.Category == oldPos.category).Id, Kind = oldPos.kind };
        listKinds.Add(temp);
        tkinds.Insert(temp);
    }
}

listCategories.Clear();
listCoins.Clear();
listKinds.Clear();
//listCategories = tc.GetAll().ToList();
listCoins = tcoins.GetAll().ToList();
listKinds = tkinds.GetAll().ToList();

//Запись в новую БД
foreach (var oldPos in oldPositions)
{
    PositionModel tempPos = new PositionModel()
    {
        PositionKey = (int)oldPos.myKey,
        OpenDate = oldPos.openDate.ToString("dd.MM.yyyy H:mm:ss"),
        CloseDate = oldPos.closeDate.Year != 9999 ? oldPos.closeDate.ToString("dd.MM.yyyy H:mm:ss") : string.Empty,
        KindId = listKinds.First(k => k.Kind == oldPos.kind).Id,
        Income = oldPos.income,
        Expense = oldPos.expense,
        Saldo = oldPos.saldo,
        CoinId = listCoins.First(c => c.ShortName == oldPos.currCoin).Id,
        StatusId = oldPos.status == "закрыта" ? 1 : 2,
        Tag = oldPos.tag,
        Notes = oldPos.notes,
        AdditionalPositionData = string.Empty
    };
    tpos.Insert(tempPos);
}












#if ConnectionStringTest

string res = Config.GetFromConfig(@"config.json").ToString();
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

var data4 = await tpos.GetAllAsync();


foreach (var pos in data4)
{
	Console.WriteLine($"{pos.Id}   {pos.PositionKey}	{pos.OpenDate}	{pos.CloseDate}   {pos.KindId}	{pos.Income}   {pos.Expense}   {pos.Saldo}   {pos.CoinId}  {pos.StatusId}   {pos.Tag}	{pos.Notes}");
}

#endif
#if ServicesTestReadById
//var data = tc.GetByIdAsync(1).Result;
////var tableCategories = await SQLService<CategoryModel>.GetAllAsync("Categories");
//Console.WriteLine(data.Category);

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

//var testpos = new PositionModel() { PositionKey = 100, OpenDate = DateTime.Now, CloseDate = DateTime.Now, KindId = 1, Income = 555.321M , Expense = 5.00001M, Saldo = 555.9874655555M, CoinId = 2, StatusId = 4, Tag = "Тест15", Notes = "Тест15" };
//await tpos.InsertAsync(testpos);

#endif

#if ServicesTestUpdate
//var testcategory = new CategoryModel() { Category = "Тест Изменения Записи!!!!!!!!!!!!!!!!", Id = 18 };
//await tc.UpdateAsync(testcategory);

//var testcoin = new CoinModel() { ShortName = "Kaspa", FullName = "КАСПА", CoinNotes = "Цифровым серебром БЫТЬ!", Id = 1002 };
//await tcoins.UpdateAsync(testcoin);

var testKIND = new KindModel() { CategoryId = 9, Kind = "Тест", Id = 1020 };
await tkinds.UpdateAsync(testKIND);

#endif

#if TestBLLModel

//var data = tc.GetByIdAsync(1).Result;

//CategoryBLLModel categoryModelBLL = MapperBL.MapCategoryDALToCategoryBLL(data);

//Console.WriteLine(categoryModelBLL.Id + "\t" + categoryModelBLL.Category);

var data = tpos.GetAllAsync().Result;
var kindsDAL = tkinds.GetAllAsync().Result;
var coinsDAL = tcoins.GetAllAsync().Result;
var statusesDAL = tst.GetAllAsync().Result;
var categoriesDAL = tc.GetAllAsync().Result;

//foreach (var item in categoriesDAL)
//{
//	Console.WriteLine(item.Id + " " + item.Category);
//}

//var singleKind = tkinds.GetByIdAsync(1).Result;
//Console.WriteLine("\n"+singleKind.Id +" "+singleKind.CategoryId +" "+ singleKind.Kind);

//var kindBLL = MapperBL.MapKindDALToKindBLL(singleKind, categoriesDAL);

//Console.WriteLine("\n" + kindBLL.Id +"  "+ (kindBLL.Category).Category + "  " + kindBLL.Kind);

List<PositionBLLModel> LPOSBLL = new List<PositionBLLModel>();
foreach (var model in data)
{
	var temp = MapperBL.MapPositionDALToPositionBLL(model, kindsDAL, coinsDAL, statusesDAL, categoriesDAL);
	LPOSBLL.Add(temp);
}

foreach (var pos in LPOSBLL)
{
	Console.WriteLine($"{pos.Id}   {pos.PositionKey}	{pos.OpenDate}	{pos.CloseDate}   {pos.Kind.Kind}	{pos.Income}   {pos.Expense}   {pos.Saldo}   {pos.Coin.ShortName}  {pos.Status.StatusName}   {pos.Tag}	{pos.Notes}");
}

#endif

#endif

#if TestBLLContext

using MyMiniLedger.BLL;
using MyMiniLedger.BLL.Context;
using MyMiniLedger.BLL.Models;




var init = new InitConfigBLL();

//Категории
//var context = new Context().CategoriesTableBL.GetAllAsync();

//var insertCategory = new CategoryBLLModel() { Category = $"Тест BLL abc {DateTime.Now}" };
//await new Context().CategoriesTableBL.InsertAsync(insertCategory);

//var updateCategory = new CategoryBLLModel() { Id = 15, Category = $"Исправленная категория 15 {DateTime.Now}" };
//await new Context().CategoriesTableBL.UpdateAsync(updateCategory);

//await foreach (var item in context)
//{
//	Console.WriteLine($"{item.Id}  {item.Category}");
//}

//var contextById = new Context().CategoriesTableBL.GetByIdAsync(15).Result;
//Console.WriteLine($"\n{contextById.Id}  {contextById.Category}");

////Монеты
//await foreach (var item in new Context().CoinsTableBL.GetAllAsync())
//{
//	Console.WriteLine($"{item.Id}  {item.ShortName}  {item.FullName}  {item.CoinNotes}");
//}

////Виды
//await foreach (var item in new Context().KindsTableBL.GetAllAsync())
//{
//	Console.WriteLine($"{item.Id}  {item.Category.Category}  {item.Kind}");
//}

////Статусы
//await foreach (var item in new Context().StatusesTableBL.GetAllAsync())
//{
//	Console.WriteLine($"{item.Id}  {item.StatusName}  {item.StatusNotes}");
//}

//Позиции
//Для IAsyncEnumerable
var con = new Context().PositionsTableBL.GetAllAsync().Result;
foreach (var item in con)
{
	Console.WriteLine($"{item.Id}  {item.PositionKey}  {item.OpenDate}  {item.CloseDate}  {item.Kind.Kind}  {item.Income}  {item.Expense} {item.Saldo}  {item.Coin.ShortName}  {item.Status.StatusName}  {item.Tag}  {item.Notes}");
}


#endif
