namespace MyMiniLedger.DAL.Interfaces
{
	internal interface ICreate<T>
	{
        public void Insert(T entity);
    }
}
