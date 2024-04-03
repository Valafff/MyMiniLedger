namespace MyMiniLedger.DAL.Interfaces
{
	internal interface IReadAll<T>
	{
		public IEnumerable<T> GetAll();
	}
}
