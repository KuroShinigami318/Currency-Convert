using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConvert.Entities
{
    class Log
    {
        public int id { get; set; }

        public string baseName { get; set; }

        public double value { get; set; }

        public string targetName { get; set; }

        public double result { get; set; }
    }
}
