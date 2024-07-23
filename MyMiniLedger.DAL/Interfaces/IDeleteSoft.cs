namespace MyMiniLedger.DAL.Interfaces
{
	//public interface IDeleteSoft<T>
	//{
	//	Task DeleteSoftAsync(T entity);
	//}

    public interface IDeleteSoft<T>
    {
        void DeleteSoft(T entity);
    }
}
