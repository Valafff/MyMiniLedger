namespace MyMiniLedger.DAL.Interfaces
{
	internal interface IDeleteHard<T>
	{
		Task DeleteHardAsync(T entity);
	}
}
