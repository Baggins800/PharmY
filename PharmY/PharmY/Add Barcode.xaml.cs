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
    public partial class AddBarcode : Window
    {
        public AddBarcode()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Admin(); //create your new form admin.
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
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "insert into ITEMS ([BARCODE_ID],[NAME]) values (?,?);";
                    cmd.Parameters.AddWithValue("@BARCODE_ID", barcode);
                    cmd.Parameters.AddWithValue("@NAME", edtaddbarcode.Text);
                    cmd.Connection = conn;
                    conn.Open();
                    try { cmd.ExecuteNonQuery(); }
                    catch (Exception enq) { MessageBox.Show(enq.Message); }

                }
        }

        private void btnaddbarcode_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
