using MyMiniLedger.DAL.Interfaces;
using MyMiniLedger.DAL.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.BLL.Context
{
	public class ListOfKinds
	{

		private readonly IReadAll<DAL.Models.KindModel> _sourceForRead;
		private readonly IReadById<DAL.Models.KindModel> _sourceForReadById;
		private readonly ICreate<DAL.Models.KindModel> _sourceForInsert;
		private readonly IUpdate<DAL.Models.KindModel> _sourceForUpdate;

		public ListOfKinds()
		{
			_sourceForRead = new TableKinds();
			_sourceForReadById = new TableKinds();
			_sourceForInsert = new TableKinds();
			_sourceForUpdate = new TableKinds();
		}



	}
}
