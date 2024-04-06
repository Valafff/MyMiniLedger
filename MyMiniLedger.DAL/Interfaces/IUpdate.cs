namespace MyMiniLedger.DAL.Interfaces
{
	public interface IUpdate<T>
	{
		Task UpdateAsync(T entity);
	}
}
