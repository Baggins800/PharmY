﻿using System;
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
    /// Interaction logic for Window7.xaml
    /// </summary>
    public partial class SearchActiveIngridient 
    {
        public SearchActiveIngridient()
        {
            InitializeComponent();
        }

        private void btnbacktomain_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new MainWindow(); //create your new form hospital.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnsearch_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
