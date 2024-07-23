namespace MyMiniLedger.DAL.Interfaces
{
    public interface ICreate<T>
    {
        void Insert(T entity);
    }
}
