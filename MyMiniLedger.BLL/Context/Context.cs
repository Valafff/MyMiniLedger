using MyMiniLedger.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.BLL.Context
{
	//Класс для работы с методами BL
	public class Context
	{
        public ListOfCategories CategoriesTableBL { get; }
		public ListOfCoins CoinsTableBL { get; }


        public Context()
        {
			CategoriesTableBL = new ListOfCategories();
			CoinsTableBL = new ListOfCoins();

		}
    }



}
