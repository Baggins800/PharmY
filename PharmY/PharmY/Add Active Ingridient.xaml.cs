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
    /// Interaction logic for Window5.xaml
    /// </summary>
    public partial class AddActiveIngridient : Window
    {
        public AddActiveIngridient()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Admin(); //create your new form admin.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

        private void btnaddactive_Click(object sender, RoutedEventArgs e)
        {
            int barcode = 0;
            if (!Int32.TryParse(edtbarcode.Text, out barcode))
            {
                MessageBox.Show("Please make sure the barcode is a number.");
            }
            else
                using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
                {
                    OleDbCommand add_barcode = new OleDbCommand();
                    add_barcode.CommandType = CommandType.Text;
                    add_barcode.CommandText = "insert into ACTIVE_INGREDIENTS ([INGREDIENT_ID],[BARCODE_ID]) values (?,?);";
                    add_barcode.Parameters.AddWithValue("@INGREDIENT_ID", edtactivename.Text.ToUpper());
                    add_barcode.Parameters.AddWithValue("@BARCODE_ID", barcode);
                    add_barcode.Connection = conn;
                    conn.Open();
                    try { add_barcode.ExecuteNonQuery(); }
                    catch (Exception enq) { MessageBox.Show(enq.Message); }
                }
        }
    }
}
