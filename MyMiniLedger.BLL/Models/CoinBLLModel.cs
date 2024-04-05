using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.BLL.Models
{
	public class CoinBLLModel
	{
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string? CoinNotes { get; set; }
    }
}
