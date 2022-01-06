using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConvert.DTO
{
    class Currency
    {
        private string Name;
        private string Code;
        private string targetName;
        private string targetCode;
        private double rate;
        private string updateDate;

        public Currency(string name, string code, string targetName, string targetCode, double rate, string updateDate)
        {
            Name = name;
            Code = code;
            this.targetName = targetName;
            this.targetCode = targetCode;
            this.rate = rate;
            this.updateDate = updateDate;
        }

        public Currency()
        {
        }

        public void setName(string Name)
        {
            this.Name = Name;
        }

        public void setCode(string Code)
        {
            this.Code = Code;
        }

        public string getName()
        {
            return Name;
        }

        public string getCode()
        {
            return Code;
        }

        public void setTargetName(string targetName)
        {
            this.targetName = targetName;
        }

        public void setTargetCode(string targetCode)
        {
            this.targetCode = targetCode;
        }

        public string getTargetName()
        {
            return targetName;
        }

        public string getTargetCode()
        {
            return targetCode;
        }

        public void setRate(double rate)
        {
            this.rate = rate;
        }

        public double getRate()
        {
            return rate;
        }

        public void setUpdateDate(string updateDate)
        {
            this.updateDate = updateDate;
        }

        public string getUpdateDate()
        {
            return updateDate;
        }
    }
}
