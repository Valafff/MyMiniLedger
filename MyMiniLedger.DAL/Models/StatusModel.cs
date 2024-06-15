using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.DAL.Models
{
	public class StatusModel
	{
        public int Id { get; set; }
		public string StatusName { get; set; }
		public string? StatusNotes { get; set; }
    }
}
