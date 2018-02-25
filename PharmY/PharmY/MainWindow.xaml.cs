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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace PharmY
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Admin(); //create your new form admin.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

        private void btnhospital_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Hospital(); //create your new form hospital.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new SearchActiveIngridient(); //create your new form search active ingridient.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }
    }
}
