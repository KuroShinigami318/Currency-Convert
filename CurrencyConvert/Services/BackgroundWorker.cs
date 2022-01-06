using CurrencyConvert.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.EntityClient;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace CurrencyConvert.Services
{
    class BackgroundWorker
    {
        Control control;
        String url;
        List<Entities.Currency> currencies; 

        public BackgroundWorker(Control control)
        {
            currencies = new List<Entities.Currency> ();
            this.control = control;
            using (var context = new MyContext())
            {
                if (!context.setting.Any())
                {
                    var setting = new Entities.Setting { url = "http://www.floatrates.com/daily/usd.xml", isFirstTime = true };
                    context.setting.Add(setting);
                    context.SaveChanges();
                }
            }
        }

        internal string GetHost(string url)
        {
            url = url.ToLower(); Uri tmp = new Uri(url); return tmp.Host;
        }

        internal int GetPort(string url)
        {
            Uri tmp = new Uri(url); return tmp.Port;
        }


        public void getList(ArrayList list, DataTable dtCurrency, ComboBox from, ComboBox to, Label label, Boolean check, Button ExRate, bool isUpdate)
        {
            //TcpClient client = new TcpClient();
            try
            {
                /*client.Connect(GetHost(url), GetPort(url));
                StreamWriter OutStream = new StreamWriter(client.GetStream()); StreamReader InpStream = new StreamReader(client.GetStream());
                OutStream.WriteLine("GET " + url + " HTTP/1.0"); OutStream.WriteLine("Content-Type:text/xml"); OutStream.WriteLine("Content-Language:en"); OutStream.WriteLine();

                OutStream.Flush();
                String header = ""; string s;
                while (null != (s = InpStream.ReadLine()))
                {
                    if (s.Equals("")) break;
                    header += s;
                }

                int c;
                String xml = "";
                while (true)
                {
                    c = InpStream.Read(); if (c == -1) break;
                    xml += (char)c;
                }
                client.Close();
                */
                using (var context = new MyContext())
                {
                    var setting = context.setting.First();
                    bool isFirstTime = setting.isFirstTime;

                    if (isFirstTime || isUpdate)
                    {
                        url = setting.url;
                        if (isUpdate)
                        {
                            list.Clear();
                            var currencies = context.currencies;
                            context.currencies.AttachRange(currencies);
                            context.currencies.RemoveRange(currencies);
                            context.SaveChanges();
                        }
                        Xml_Reader xmlReader = new Xml_Reader(url);
                        xmlReader.updateList(list, currencies);
                        setting.isFirstTime = false;
                        context.setting.Update(setting);
                        context.currencies.AddRangeAsync(currencies);
                        context.SaveChanges();
                    }
                    else
                    {
                        var currencies = context.currencies;
                        foreach (Entities.Currency currency in currencies)
                        {
                            Currency currency1 = new Currency(currency.Name, currency.Code, currency.targetName, currency.targetCode, currency.rate, currency.updateDate);
                            list.Add(currency1);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show("Connection error due to Network problem! Please try again later.");
                control.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ExRate.IsEnabled = true;
                }));
            }
            
            for (int i = 0; i < list.Count; i++)
            {
                Currency currency = (Currency)list[i];
                dtCurrency.Rows.Add(currency.getTargetName(), currency);
            }
            check = true;
            control.Dispatcher.BeginInvoke(new Action(() =>
            {
                from.IsEnabled = true;
                to.IsEnabled = true;
                if (from.Items.Count > 0)
                    from.SelectedIndex = 0;
                else return;
                if (to.Items.Count > 0)
                    to.SelectedIndex = 0;
                else return;
                Currency toCur = (Currency)((DataRowView)to.SelectedItem).Row[1];
                Currency fromCur = (Currency)((DataRowView)from.SelectedItem).Row[1];
                double rate = toCur.getRate() / fromCur.getRate();
                label.Content = "1 " + fromCur.getTargetCode() + " = " + Convert.ToDecimal(rate).ToString("N1") + " " + toCur.getTargetCode() + "\nUpdate Date: " + fromCur.getUpdateDate();
                ExRate.IsEnabled = true;
            }));
            // Close the response.
            //response.Close();
        }
    }
}
