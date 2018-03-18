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
    /// Interaction logic for Window5.xaml
    /// </summary>
    public partial class AddActiveIngridient 
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
            if (edtbarcode.Text.Length < 1 && edtactivename.Text.Length < 1)
            {
                MessageBox.Show("Please make sure the barcode and ingredient ID is included.");
            }
            else
            {
                int activeExists = 0;
                using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
                {
                    OleDbCommand count_active = new OleDbCommand();
                    count_active.CommandType = CommandType.Text;
                    count_active.CommandText = "select count (*) from ACTIVE_INGREDIENTS where [INGREDIENT_ID] = ?";
                    count_active.Parameters.AddWithValue("@INGREDIENT_ID", edtactiveid.Text.ToUpper());
                    count_active.Connection = conn;
                    conn.Open();
                    try { activeExists = (int)count_active.ExecuteScalar(); }
                    catch (Exception enq) { MessageBox.Show(enq.Message); }
                }
                using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
                {
                    if (activeExists < 1)
                    {
                        OleDbCommand add_active = new OleDbCommand();
                        add_active.CommandType = CommandType.Text;
                        add_active.CommandText = "insert into ACTIVE_INGREDIENTS ([INGREDIENT_ID], [INGREDIENT_NAME]) values (?,?);";
                        add_active.Parameters.AddWithValue("@INGREDIENT_ID", edtactiveid.Text.ToUpper());
                        add_active.Parameters.AddWithValue("@INGREDIENT_NAME", edtactivename.Text.ToUpper());
                        add_active.Connection = conn;
                        conn.Open();
                        try { add_active.ExecuteNonQuery(); }
                        catch (Exception enq) { MessageBox.Show(enq.Message); }
                    }
                }
                using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
                {
                    OleDbCommand add_active = new OleDbCommand();
                    add_active.CommandType = CommandType.Text;
                    add_active.CommandText = "insert into ITEM_INGREDIENTS ([BARCODE_ID],[INGREDIENT_ID]) values (?,?);";
                    add_active.Parameters.AddWithValue("@BARCODE_ID", edtbarcode.Text.ToUpper());
                    add_active.Parameters.AddWithValue("@INGREDIENT_ID", edtactiveid.Text.ToUpper());
                    add_active.Connection = conn;
                    conn.Open();
                    try
                    {
                        add_active.ExecuteNonQuery();
                        edtbarcode.Text = "";
                        edtactiveid.Text = "";
                        edtactivename.Text = "";
                    }
                    catch (Exception enq) { MessageBox.Show(enq.Message); }

                }
            }
        }
    }
}
