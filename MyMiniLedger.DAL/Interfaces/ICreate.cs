namespace MyMiniLedger.DAL.Interfaces
{
	public interface ICreate<T>
	{
       Task InsertAsync(T entity);
    }
}
