using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.BLL.Models
{
    public class AdditionalPositionDataClass
    {
		public int? ZeroParrentKey { get; set; }
		public int? PerrentKey { get; set; }
        public AdditionalPositionDataClass(){}
        public AdditionalPositionDataClass(int? _zeroParrentKey, int? _perrentKey)
        {
            ZeroParrentKey = _zeroParrentKey;
            PerrentKey = _perrentKey;
        }
    }
	public class PositionBLLModel
	{
        public int Id { get; set; }
        public int PositionKey { get; set; }
        public string? OpenDate { get; set; }
        public string? CloseDate { get; set; }
        public KindBLLModel Kind { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Saldo { get; set; }
        public CoinBLLModel Coin { get; set; }
        public StatusBLLModel Status { get; set; }
        public string? Tag { get; set; }
        public string? Notes { get; set; }
        public AdditionalPositionDataClass additionalPositionDataBLL { get; set; }
	}
}
