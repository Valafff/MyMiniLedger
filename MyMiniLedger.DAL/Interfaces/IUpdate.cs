namespace MyMiniLedger.DAL.Interfaces
{
	internal interface IUpdate<T>
	{
		void Update(T entity);
	}
}
