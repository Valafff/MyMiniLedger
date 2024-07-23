namespace MyMiniLedger.DAL.Interfaces
{
    //Принимает число и назвние таблицы в которой будет происходить поиск 
    //public interface IReadById<T>
    //{
    //	Task<T> GetByIdAsync(int id, string tablename);
    //}

    public interface IReadById<T>
    {
        T GetById(int id, string tablename);
    }
}
