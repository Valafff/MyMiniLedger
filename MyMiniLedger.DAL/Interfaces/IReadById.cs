namespace MyMiniLedger.DAL.Interfaces
{
    //Принимает число и назвние таблицы в которой будет происходить поиск 
    public interface IReadById<T>
    {
        T GetById(int id, string tablename);
    }
}
