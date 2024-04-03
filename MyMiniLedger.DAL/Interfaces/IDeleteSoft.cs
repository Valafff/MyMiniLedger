namespace MyMiniLedger.DAL.Interfaces
{
	internal interface IDeleteSoft<T>
	{
		void DeleteSoft(T entity);
	}
}
