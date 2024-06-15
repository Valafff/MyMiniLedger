using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.BLL.Models
{
	public class KindBLLModel
	{
        public int Id { get; set; }
        public CategoryBLLModel Category { get; set; }
        public string Kind { get; set; }
    }
}
