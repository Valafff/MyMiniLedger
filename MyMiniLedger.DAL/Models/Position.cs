using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.DAL.Models
{
	public class Position
	{
        public  int Id { get; set; }
        public int PositionKey { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public Kind Kind { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Saldo { get; set; }
        public Coin Coin { get; set; }
        public Status Status { get; set; }
        public string Tag { get; set; }
        public string Notes { get; set; }
    }
}
