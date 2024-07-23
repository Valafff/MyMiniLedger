namespace MyMiniLedger.DAL.Interfaces
{
    //public interface ICreate<T>
    //{
    //      Task InsertAsync(T entity);
    //   }
    public interface ICreate<T>
    {
        void Insert(T entity);
    }
}
