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


namespace PharmY
{
    /// <summary>
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        public Window3()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Window1(); //create your new form admin.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

        private void btnadd_Click(object sender, RoutedEventArgs e)
        {
            
            int barcode = 0;
            if (!Int32.TryParse(edtaddbarcode.Text, out barcode))
            {
                MessageBox.Show("Please make sure the barcode is a number.");
            }
            else
                using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
                {
                    //conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\Baggins\\Desktop\\PharmY.mdb; Persist Security Info = False; ";
                    OleDbCommand add_barcode = new OleDbCommand();
                    OleDbCommand add_quantity = new OleDbCommand();
                    add_barcode.CommandType = CommandType.Text;
                    add_barcode.CommandText = "insert into ITEMS ([BARCODE_ID],[NAME]) values (?,?);";
                    add_barcode.Parameters.AddWithValue("@BARCODE_ID", barcode);
                    add_barcode.Parameters.AddWithValue("@NAME", edtaddbarcode.Text);
                    add_barcode.Connection = conn;

                    add_quantity.CommandType = CommandType.Text;
                    add_quantity.CommandText = "insert into DATES_ADDED ([BARCODE_ID],[DATE], [QUANTITY]) values (?,?,?);";
                    add_quantity.Parameters.AddWithValue("@BARCODE_ID", barcode);
                    add_quantity.Parameters.AddWithValue("@DATE", "some date");
                    add_quantity.Parameters.AddWithValue("@QUANTITY", 5);
                    add_quantity.Connection = conn;
                    conn.Open();
                    try { add_barcode.ExecuteNonQuery(); }
                    catch (Exception enq) { MessageBox.Show(enq.Message); }
                    try { add_quantity.ExecuteNonQuery(); }
                    catch (Exception enq) { MessageBox.Show(enq.Message); }

                }
        }
    }
}
