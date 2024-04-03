using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.DAL.Models
{
	public class KindModel
	{
        public int Id { get; set; }
        public CategoryModel Category { get; set; }
        public string Kind { get; set; }
    }
}
