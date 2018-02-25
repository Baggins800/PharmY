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

namespace PharmY
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Admin
    {
        public Admin()
        {
            InitializeComponent();
        }

        private void btnadd_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new AddBarcode(); //create your new form add barcode.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

        private void btnremove_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new RemoveBarcode(); //create your new form remove barcode.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

        private void btnaddactive_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new AddActiveIngridient(); //create your new form add active ingredient.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

        private void btnremoveactive_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new RemoveActiveIngridient(); //create your new form remove active ingridient.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new MainWindow(); //create your new form main window.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

        private void btnreview_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new CreateReview(); //create your new form main window.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }
    }
}
