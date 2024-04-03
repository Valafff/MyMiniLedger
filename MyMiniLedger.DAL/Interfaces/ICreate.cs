namespace MyMiniLedger.DAL.Interfaces
{
	internal interface ICreate<T>
	{
       Task InsertAsync(T entity);
    }
}
