using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.DAL.Models
{
	public class Kind
	{
        public int Id { get; set; }
        public Category Category { get; set; }
        public string Kind { get; set; }
    }
}
