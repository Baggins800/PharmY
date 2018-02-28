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
using MahApps.Metro.Controls;
using System.Data.OleDb;
using System.Data;
using System.Configuration;

namespace PharmY
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>

    public partial class Hospital
    {
        private class Item
        {
            public string Name;
            public int Value, Index;
            public Item(string name, int value, int index)
            {
                Name = name; Value = value; Index = index;
            }
            public override string ToString()
            {
                return Name;
            }
        }
        private class CheckoutItem
        {
            public string Name, PatientName, CheckoutTime, Department;
            public int Quantity, DepartmentID;
            public string BarcodeID;
            public CheckoutItem(string name, string barcodeid, int quantity, string patientname, string checkouttime, string department, int departmentid)
            {
                Name = name; BarcodeID = barcodeid; Quantity = quantity; PatientName = patientname; CheckoutTime = checkouttime;
                DepartmentID = departmentid; Department = department;
            }
            public override string ToString()
            {
                return Department + "\t" + PatientName + "\t" + Name + "\t" + Quantity.ToString();
            }
        }
        public Hospital()
        {
            InitializeComponent();
            try
            {
                using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
                {
                    OleDbCommand select_departments = new OleDbCommand();
                    select_departments.CommandType = CommandType.Text;
                    select_departments.CommandText = "select DEPARTMENT_ID, DEPARTMENT from DEPARTMENTS;";
                    select_departments.Connection = conn;
                    conn.Open();
                    int index = 0;
                    using (OleDbDataReader reader = select_departments.ExecuteReader())
                        while (reader.Read()) {
                            cbdepartment.Items.Add(new Item(reader.GetString(1), reader.GetInt32(0), index++));
                        }

                }
                cbdepartment.SelectedIndex = 0;
            }
            catch (Exception enq) { MessageBox.Show(enq.Message); }
            DateHospital.SelectedDate = DateTime.Today;
            edtbarcode.Focus();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new MainWindow(); //create your new form mainwindow.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

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
                        select_barcode.Parameters.AddWithValue("@BARCODE_ID", edtbarcode.Text);
                        select_barcode.Connection = conn;
                        conn.Open();
                        using (OleDbDataReader reader = select_barcode.ExecuteReader())
                            if (reader.Read())
                                lblname.Content = reader.GetString(0);

                    }
                }
                catch (Exception enq) { MessageBox.Show(enq.Message); }
                this.edtquantity.Focus();
            }
        }

        private void btnaddhospital_Click(object sender, RoutedEventArgs e)
        {
            int quantity = 0;
            if (!Int32.TryParse(edtquantity.Text, out quantity))
            {
                MessageBox.Show("Please make sure the barcode/quantity is a number.");
            }
            else
            {
                Item a = new Item("", 0, 0);
                foreach (Item l in cbdepartment.Items)
                    if (l.Index == cbdepartment.SelectedIndex) a = l;
                lbcheckout.Items.Add(new CheckoutItem((dynamic)lblname.Content, edtbarcode.Text, quantity, edtpatientname.Text, DateHospital.Text, cbdepartment.Text, a.Value));
                edtbarcode.Focus();
                edtbarcode.Clear();
                edtpatientname.Clear();
                edtquantity.Clear();
                lblname.Content = "";
            }
        }

        private void btncheckout_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckoutItem item in lbcheckout.Items)
            {
                using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
                {
                    OleDbCommand add_script = new OleDbCommand();
                    OleDbCommand add_quantity = new OleDbCommand();
                    add_script.CommandType = CommandType.Text;
                    add_script.CommandText = "insert into OUT_SCRIPTS ([DEPARTMENT_ID],[BARCODE_ID],[QUANTITY],[DATE], [PATIENT_NAME]) values (?,?,?,?,?);";
                    add_script.Parameters.AddWithValue("@DEPARTMENT_ID", item.DepartmentID);
                    add_script.Parameters.AddWithValue("@BARCODE_ID", item.BarcodeID);
                    add_script.Parameters.AddWithValue("@QUANTITY", item.Quantity);
                    add_script.Parameters.AddWithValue("@DATE", item.CheckoutTime);
                    add_script.Parameters.AddWithValue("@PATIENT_NAME", item.PatientName);
                    add_script.Connection = conn;
                    conn.Open();
                    try { add_script.ExecuteNonQuery(); }
                    catch (Exception enq) { MessageBox.Show(enq.Message); }
                    edtquantity.Clear();
                }
            }
            lbcheckout.Items.Clear();
            edtbarcode.Focus();
            edtbarcode.Clear();
            edtpatientname.Clear();
            edtquantity.Clear();
            lblname.Content = "";
        }

        private void edtquantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Tab)
            {
                edtpatientname.Focus();
            }
        }

        private void btnaddhospital_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Tab)
            {
                btncheckout.Focus();
            }
        }

        private void edtpatientname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Tab)
            {
                btnaddhospital.Focus();
            }
        }
    }
}
