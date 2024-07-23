namespace MyMiniLedger.DAL.Interfaces
{
    public interface IDeleteSoft<T>
    {
        void DeleteSoft(T entity);
    }
}
