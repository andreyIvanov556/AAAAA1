using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAAAA1
{
    public class GetAnalzer// класс для получения данных анализатора
    {
        public string patient { get; set; }// хранит пациента
        public List<Services> services { get; set; }// хранит услуги
        public int progress { get; set; } // хранит процент выполнения услуги 
    }
}
