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

namespace PharmY
{
    /// <summary>
    /// Interaction logic for CreateReview.xaml
    /// </summary>
    public partial class CreateReview : Window
    {
        public CreateReview()
        {
            InitializeComponent();
        }

        private void btnbacktoadminreview_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Admin(); //create your new form main window.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }
    }
}
