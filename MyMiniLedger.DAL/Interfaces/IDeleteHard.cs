namespace MyMiniLedger.DAL.Interfaces
{
	internal interface IDeleteHard<T>
	{
		void DeleteHard(T entity);
	}
}
