using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.DAL.Models
{
	public class PositionModel
	{
        public  int Id { get; set; }
        public int PositionKey { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public int KindId { get; set; }
        public double Income { get; set; }
        public double Expense { get; set; }
        public double Saldo { get; set; }
        public int CoinId { get; set; }
        public int StatusId { get; set; }
        public string Tag { get; set; }
        public string Notes { get; set; }
    }
}
