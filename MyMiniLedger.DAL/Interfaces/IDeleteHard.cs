namespace MyMiniLedger.DAL.Interfaces
{
	public interface IDeleteHard<T>
	{
		Task DeleteHardAsync(T entity);
	}
}
