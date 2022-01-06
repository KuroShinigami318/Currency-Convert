using CurrencyConvert.DTO;
using CurrencyConvert.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CurrencyConvert
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        private Boolean check = false;
        private ArrayList currencyList;
        private BackgroundWorker backgroundWorker;
        string url;
        private DataTable dtCurrency;
        private List<Log> history;
        public ObservableCollection<Log> observable { get; set; }

        public MainWindow()
        {
            currencyList = new ArrayList();
            history = new List<Log>();
            dtCurrency = new DataTable();
            backgroundWorker = new BackgroundWorker(this);
            InitializeComponent();
            BindCurrency();
        }

        public static void updateRate(Control control, ComboBox from, ComboBox to, Label label)
        {
            control.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (to.SelectedItem != null && from.SelectedItem != null)
                {
                    Currency toCur = (Currency)((DataRowView)to.SelectedItem).Row[1];
                    Currency fromCur = (Currency)((DataRowView)from.SelectedItem).Row[1];
                    double rate = toCur.getRate() / fromCur.getRate();
                    label.Content = "1 " + fromCur.getTargetCode() + " = " + System.Convert.ToDecimal(rate).ToString("F") + " " + toCur.getTargetCode() + "\nUpdate Date: " + fromCur.getUpdateDate();
                }
            }));
        }

        private void BindCurrency()
        {

            //Add display column in DataTable
            dtCurrency.Columns.Add("Text");

            //Add value column in DataTable
            dtCurrency.Columns.Add("Value", typeof(Currency));

            //Add rows in Datatable with text and value

            //The data to currency Combobox is assigned from datatable
            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;

            //DisplayMemberPath Property is used to display data in Combobox
            cmbFromCurrency.DisplayMemberPath = "Text";

            //SelectedValuePath property is used to set the value in Combobox
            cmbFromCurrency.SelectedValuePath = "Value";

            //SelectedIndex property is used to bind hint in the Combobox. The default value is Select.

            //All properties are set for 'To Currency' Combobox as 'From Currency' Combobox
            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            ExRate.IsEnabled = false;
            cmbFromCurrency.IsEnabled = false;
            cmbToCurrency.IsEnabled = false;
            Thread background = new Thread(unused => backgroundWorker.getList(currencyList, dtCurrency, cmbFromCurrency, cmbToCurrency, txtCurrencyRate, check, ExRate, false));
            background.Start();
        }

        private void updateResult()
        {
            string replaced = txtCurrency.Text.Replace(" ", "").Replace(@".{2,}", ".");
            int index = replaced.IndexOf('.');
            String last = replaced.Substring(index + 1).Replace(@".", "");
            String concat = replaced.Substring(0, index + 1) + last;

            //Create a variable as ConvertedValue with double data type to store currency converted value
            double ConvertedValue = 0;
            if (concat != "")
            {
                if (concat == ".") concat = "0" + concat;
                ConvertedValue = double.Parse(concat, System.Globalization.CultureInfo.InvariantCulture);
            }
            
            txtCurrency.Text = ConvertedValue.ToString("N", System.Globalization.CultureInfo.InvariantCulture);
            //Else if the currency from is not selected or it is default text --SELECT--
            if (cmbFromCurrency.SelectedValue == null)
            {
                //It will show the message
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on From Combobox
                cmbFromCurrency.Focus();
                return;
            }
            //Else if Currency To is not Selected or Select Default Text --SELECT--
            else if (cmbToCurrency.SelectedValue == null)
            {
                //It will show the message
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on To Combobox
                cmbToCurrency.Focus();
                return;
            }
            //If From and To Combobox selected values are same

            else
            {

                //Calculation for currency converter is From Currency value multiply(*) 
                // with amount textbox value and then the total is divided(/) with To Currency value
                Currency toCur = (Currency)((DataRowView)cmbToCurrency.SelectedItem).Row[1];
                Currency fromCur = (Currency)((DataRowView)cmbFromCurrency.SelectedItem).Row[1];
                double result = ConvertedValue * toCur.getRate() / fromCur.getRate();

                //Show in label converted currency and converted currency name.
                lblCurrency.Content = System.Convert.ToDecimal(result).ToString("N") + " " + cmbToCurrency.Text;
                history.Add(new Log(fromCur.getTargetName(), ConvertedValue, toCur.getTargetName(), result));
                observable = new ObservableCollection<Log>(history);
                History.ItemsSource = observable;
            }
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Check amount textbox is Null or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                //If amount textbox is Null or Blank it will show the below message box   
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //After clicking on message box OK sets the Focus on amount textbox
                txtCurrency.Focus();
                return;
            }
            updateResult();
        }

        //Allow only the integer value in TextBox
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regular Expression to add regex add library using System.Text.RegularExpressions;
            Regex regex = new Regex(@"[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Clear button click event
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //ClearControls method  is used to clear all control value
            ClearControls();
        }

        //ClearControls used for clear all controls value
        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "Giá trị chuyển đổi sẽ hiện thị tại đây";
            txtCurrency.Focus();
        }

        //ExchangeRate Button
        private void Rate_Click(object sender, RoutedEventArgs e)
        {
            ExRate.IsEnabled = false;
            cmbFromCurrency.IsEnabled = false;
            cmbToCurrency.IsEnabled = false;
            check = false;
            dtCurrency = new DataTable();
            dtCurrency.Columns.Add("Text");

            //Add value column in DataTable
            dtCurrency.Columns.Add("Value", typeof(Currency));

            //Add rows in Datatable with text and value

            //The data to currency Combobox is assigned from datatable
            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;

            //DisplayMemberPath Property is used to display data in Combobox
            cmbFromCurrency.DisplayMemberPath = "Text";

            //SelectedValuePath property is used to set the value in Combobox
            cmbFromCurrency.SelectedValuePath = "Value";

            //SelectedIndex property is used to bind hint in the Combobox. The default value is Select.

            //All properties are set for 'To Currency' Combobox as 'From Currency' Combobox
            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            Thread background = new Thread(unused => backgroundWorker.getList(currencyList, dtCurrency, cmbFromCurrency, cmbToCurrency, txtCurrencyRate, check, ExRate, true));
            background.Start();
        }
        //Reload Button
        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new MyContext())
            {
                var logs = context.logs;
                context.logs.AttachRange(logs);
                context.logs.RemoveRange(logs);
                context.SaveChanges();
                var his = new List<Entities.Log>();
                foreach (Log log in history)
                {
                    his.Add(new Entities.Log { baseName = log.baseName, value = log.value, targetName = log.targetName, result = log.result });
                }
                context.logs.AddRangeAsync(his);
                context.SaveChanges();
                MessageBox.Show("Save to Database Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void CmbFromCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Run(() => updateRate(this, cmbFromCurrency, cmbToCurrency, txtCurrencyRate));
            if (check) updateResult();
        }

        private async void cmbToCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Run(() => updateRate(this, cmbFromCurrency, cmbToCurrency, txtCurrencyRate));
            if (check) updateResult();
        }

        private void History_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void History_Loaded(object sender, RoutedEventArgs e)
        {
            using (var context = new MyContext())
            {
                var logs = context.logs;
                foreach (Entities.Log log in logs)
                {
                    history.Add(new Log(log.baseName, log.value, log.targetName, log.result));
                }
            }
            observable = new ObservableCollection<Log>(history);
            History.ItemsSource = observable;
            History.DataContext = this;
            fCol.Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
            tCol.Width = new DataGridLength(0.3, DataGridLengthUnitType.Star);
            vCol.Width = new DataGridLength(0.2, DataGridLengthUnitType.Star);
            rCol.Width = new DataGridLength(0.2, DataGridLengthUnitType.Star);
        }

        private void txtInputKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) updateResult();
        }
    }
}
