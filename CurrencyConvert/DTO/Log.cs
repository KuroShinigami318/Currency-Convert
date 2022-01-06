using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConvert.DTO
{
    public class Log
    {
        public Log(string baseName, double value, string targetName, double result)
        {
            this.baseName = baseName;
            this.value = value;
            this.targetName = targetName;
            this.result = result;
        }

        public string baseName { get; set; }

        public double value { get; set; }

        public string targetName { get; set; }

        public double result { get; set; }
    }
}
