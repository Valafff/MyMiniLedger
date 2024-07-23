namespace MyMiniLedger.DAL.Interfaces
{
    //public interface IReadAll<T>
    //{
    //	Task <IEnumerable<T>> GetAllAsync();
    //}
    public interface IReadAll<T>
    {
        IEnumerable<T> GetAll();
    }
}
