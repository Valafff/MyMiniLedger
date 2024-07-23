namespace MyMiniLedger.DAL.Interfaces
{
    public interface IDeleteHard<T>
    {
        void DeleteHard(T entity);
    }
}
