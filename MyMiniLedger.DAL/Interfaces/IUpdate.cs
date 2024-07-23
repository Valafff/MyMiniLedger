namespace MyMiniLedger.DAL.Interfaces
{
    public interface IUpdate<T>
    {
        void Update(T entity);
    }
}
