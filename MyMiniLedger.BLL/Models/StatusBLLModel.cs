using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.BLL.Models
{
	public class StatusBLLModel
	{
        public int Id { get; set; }
        public string StatusName { get; set; }
        public string? StatusNotes { get; set; }
    }
}
