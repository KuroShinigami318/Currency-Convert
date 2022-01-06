using CurrencyConvert.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CurrencyConvert.Services
{
    class Xml_Reader
    {
        private string xml;
        private string read;
        private Currency currency;
        private Entities.Currency curEntity;
        private string baseName;
        private string baseCode;
        private string updateDate;

        public Xml_Reader(string xml)
        {
            this.xml = xml ?? throw new ArgumentNullException(nameof(xml));
        }

        public Xml_Reader()
        {
        }

        public void setXml(string xml)
        {
            this.xml = xml;
        }

        public string getXml()
        {
            return xml;
        }

        public void updateList(ArrayList arrayList, List<Entities.Currency> currencies)
        {
            //XmlReader reader = XmlReader.Create(new StringReader(xml));
            XmlTextReader reader = new XmlTextReader(xml);
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        if (reader.Name == "baseCurrency")
                        {
                            currency = new Currency();
                            curEntity = new Entities.Currency();
                        }
                        break;

                    case XmlNodeType.Text: //Display the text in each element.
                        read = reader.Value.Trim();
                        break;

                    case XmlNodeType.EndElement: //Display the end of the element.
                        switch (reader.Name)
                        {
                            case "baseCurrency":
                                currency.setCode(read);
                                curEntity.Code = read;
                                baseCode = read;
                                break;
                            case "baseName":
                                currency.setName(read);
                                curEntity.Name = read;
                                baseName = read;
                                break;
                            case "targetCurrency":
                                currency.setTargetCode(read);
                                curEntity.targetCode = read;
                                break;
                            case "targetName":
                                currency.setTargetName(read);
                                curEntity.targetName = read;
                                break;
                            case "exchangeRate":
                                currency.setRate(double.Parse(read, System.Globalization.CultureInfo.InvariantCulture));
                                curEntity.rate = double.Parse(read, System.Globalization.CultureInfo.InvariantCulture);
                                arrayList.Add(currency);
                                currencies.Add(curEntity);
                                break;
                            case "pubDate":
                                currency.setUpdateDate(read);
                                curEntity.updateDate = read;
                                updateDate = read;
                                break;
                        }
                        break;
                }
            }
            currency = new Currency(baseName, baseCode, baseName, baseCode, 1.0, updateDate);
            curEntity = new Entities.Currency { Name = baseName, Code = baseCode, targetName = baseName, targetCode = baseCode, rate = 1.0, updateDate = updateDate };
            arrayList.Insert(0, currency);
            currencies.Insert(0, curEntity);
        }
    }
}
