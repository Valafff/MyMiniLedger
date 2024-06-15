using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMiniLedger.BLL
{
    //Из BLL передает данные для конфига в DAL
	public class InitConfigBLL
	{
        public  InitConfigBLL(string path = "config.json")
        {
            DAL.Services.DataConfig.Init(path);
        }
    }
}
