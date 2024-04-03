namespace MyMiniLedger.DAL.Interfaces
{
	internal interface IDeleteSoft<T>
	{
		Task DeleteSoftAsync(T entity);
	}
}
