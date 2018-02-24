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
    /// Interaction logic for Window6.xaml
    /// </summary>
    public partial class RemoveActiveIngridient : Window
    {
        public RemoveActiveIngridient()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Admin(); //create your new form admin.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

        private void btnremoveactive_Click(object sender, RoutedEventArgs e)
        {
            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
            {
                OleDbCommand remove_ingredient = new OleDbCommand();
                remove_ingredient.CommandType = CommandType.Text;
                remove_ingredient.CommandText = "delete * from ACTIVE_INGREDIENTS where [INGREDIENT_ID]=?;";
                remove_ingredient.Parameters.AddWithValue("@INGREDIENT_ID", edtactivename.Text.ToUpper());
                remove_ingredient.Connection = conn;
                conn.Open();
                try { remove_ingredient.ExecuteNonQuery(); }
                catch (Exception enq) { MessageBox.Show(enq.Message); }
            }
        }
    }
}
