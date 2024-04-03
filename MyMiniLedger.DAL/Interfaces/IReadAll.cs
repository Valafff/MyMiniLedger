namespace MyMiniLedger.DAL.Interfaces
{
	internal interface IReadAll<T>
	{
		Task <IEnumerable<T>> GetAllAsync();
	}
}
