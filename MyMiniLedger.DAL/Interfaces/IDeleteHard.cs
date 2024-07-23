namespace MyMiniLedger.DAL.Interfaces
{
    //public interface IDeleteHard<T>
    //{
    //	Task DeleteHardAsync(T entity);
    //}

    public interface IDeleteHard<T>
    {
        void DeleteHard(T entity);
    }
}
