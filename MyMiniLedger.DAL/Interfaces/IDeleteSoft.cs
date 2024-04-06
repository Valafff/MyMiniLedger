namespace MyMiniLedger.DAL.Interfaces
{
	public interface IDeleteSoft<T>
	{
		Task DeleteSoftAsync(T entity);
	}
}
