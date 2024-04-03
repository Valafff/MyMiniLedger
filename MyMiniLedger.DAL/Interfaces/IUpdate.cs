namespace MyMiniLedger.DAL.Interfaces
{
	internal interface IUpdate<T>
	{
		Task UpdateAsync(T entity);
	}
}
