namespace MyMiniLedger.DAL.Interfaces
{
    //public interface IUpdate<T>
    //{
    //	Task UpdateAsync(T entity);
    //}

    public interface IUpdate<T>
    {
        void Update(T entity);
    }
}
