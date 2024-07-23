namespace MyMiniLedger.DAL.Interfaces
{
    public interface IReadAll<T>
    {
        IEnumerable<T> GetAll();
    }
}
