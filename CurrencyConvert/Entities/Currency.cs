using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConvert.Entities
{
    class Currency
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string targetName { get; set; }
        public string targetCode { get; set; }
        public double rate { get; set; }
        public string updateDate { get; set; }
    }
}
