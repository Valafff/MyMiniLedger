namespace MyMiniLedger.DAL.Interfaces
{
	//Принимает число и назвние таблицы в которой будет происходить поиск 
	public interface IReadById<T>
	{
		Task<T> GetByIdAsync(int id, string tablename);
	}
}
