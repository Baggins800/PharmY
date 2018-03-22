using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data;
using System.Configuration;
using MahApps.Metro.Controls;

namespace PharmY
{
    /// <summary>
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class AddBarcode 
    {

        private void edtbarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Tab)
            {
                try
                {

                    using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
                    {
                        OleDbCommand select_barcode = new OleDbCommand();
                        select_barcode.CommandType = CommandType.Text;
                        select_barcode.CommandText = "select top 1 NAME from ITEMS where [BARCODE_ID] = ?;";
                        select_barcode.Parameters.AddWithValue("@BARCODE_ID", edtaddbarcode.Text);
                        select_barcode.Connection = conn;
                        conn.Open();
                        using (OleDbDataReader reader = select_barcode.ExecuteReader())
                            if (reader.Read())
                                edtname.Text = reader.GetString(0);

                    }
                }
                catch (Exception enq) { MessageBox.Show(enq.Message); }
                this.edtquantity.Focus();
            }
        }

        public AddBarcode()
        {
            InitializeComponent();
            this.dateadd.SelectedDate = DateTime.Today;
            edtaddbarcode.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Admin(); //create your new form admin.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

        private void btnadd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnaddbarcode_Click(object sender, RoutedEventArgs e)
        {
            int quantity = 0;
            if (!Int32.TryParse(edtquantity.Text, out quantity))
            {
                MessageBox.Show("Please make sure the quantity is a number.");
            }
            else
                using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
                {
                    OleDbCommand add_barcode = new OleDbCommand();
                    OleDbCommand add_quantity = new OleDbCommand();
                    add_barcode.CommandType = CommandType.Text;
                    add_barcode.CommandText = "insert into ITEMS ([BARCODE_ID],[NAME]) values (?,?);";
                    add_barcode.Parameters.AddWithValue("@BARCODE_ID", edtaddbarcode.Text);
                    add_barcode.Parameters.AddWithValue("@NAME", edtname.Text.ToUpper());
                    add_barcode.Connection = conn;

                    add_quantity.CommandType = CommandType.Text;
                    add_quantity.CommandText = "insert into DATES_ADDED ([INVOICED_ID],[BARCODE_ID],[DATE], [QUANTITY], [EXPIRY_DATE]) values (?,?,?,?,?);";
                    add_quantity.Parameters.AddWithValue("@INVOICED_ID", edtinvoice.Text.ToUpper());
                    add_quantity.Parameters.AddWithValue("@BARCODE_ID", edtaddbarcode.Text);
                    add_quantity.Parameters.AddWithValue("@DATE", dateadd.Text);
                    add_quantity.Parameters.AddWithValue("@QUANTITY", quantity);
                    add_quantity.Parameters.AddWithValue("@EXPIRY_DATE", dateexpire.Text);
                    add_quantity.Connection = conn;
                    conn.Open();
                    try
                    {
                        add_barcode.ExecuteNonQuery();
                        MessageBox.Show("Barcode successfully added.");
                    }
                    catch (Exception enq) { /* MessageBox.Show(enq.Message);*/ }

                    try {
                        add_quantity.ExecuteNonQuery();
                        MessageBox.Show("Stock successfully added.");
                    }
                    catch (Exception enq) { MessageBox.Show(enq.Message); }
                    edtaddbarcode.Clear();
                    edtname.Clear();
                    edtquantity.Clear();
                }
        }
    }
}
