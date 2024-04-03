namespace MyMiniLedger.DAL.Interfaces
{
	internal interface IReadById<T>
	{
		public T GetById(int id);
	}
}
