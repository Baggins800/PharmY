using System;
using System.Diagnostics;
using System.IO;
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
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Data.OleDb;
using System.Data;
using System.Configuration;
using PdfSharp.Pdf.IO;

namespace PharmY
{
    /// <summary>
    /// Interaction logic for Invetory.xaml
    /// </summary>
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
    public partial class Invetory
    {
        public Invetory()
        {
            InitializeComponent();

            this.toDate.SelectedDate = DateTime.Today;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> review_lines = new List<string>();
            List<string> review_active = new List<string>();
            List<string> review_nsn = new List<string>();
            List<string> review_name = new List<string>();

            List<string> review_quantity = new List<string>();
           
            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
            {
                OleDbCommand review = new OleDbCommand();
                review.CommandType = CommandType.Text;

                review.CommandText = "SELECT  T2.NSN,  T2.BARCODE_ID, T2.ACTIVE, T2.NAME, cStr(IIF(IsNull(T2.QUANTITY), 0, T2.QUANTITY) -  IIF(IsNull(T1.QUANTITY), 0, T1.QUANTITY)) as QUANTITY " +
                    "FROM (SELECT ITEM_INGREDIENTS.INGREDIENT_ID as [NSN], ACTIVE_INGREDIENTS.INGREDIENT_NAME as [ACTIVE], ITEMS.NAME as [NAME], " +
                    "sum(OUT_SCRIPTS.QUANTITY) as [QUANTITY], OUT_SCRIPTS.BARCODE_ID as BARCODE_ID from OUT_SCRIPTS, ITEM_INGREDIENTS, ITEMS, " +
                    "ACTIVE_INGREDIENTS where OUT_SCRIPTS.DATE between #01/01/1970# AND #" + toDate.SelectedDate.Value.ToString() + 
                    "# AND OUT_SCRIPTS.BARCODE_ID = ITEM_INGREDIENTS.BARCODE_ID " +
                    "AND ITEMS.BARCODE_ID = ITEM_INGREDIENTS.BARCODE_ID AND ACTIVE_INGREDIENTS.INGREDIENT_ID= ITEM_INGREDIENTS.INGREDIENT_ID " +
                    "group by ITEM_INGREDIENTS.INGREDIENT_ID, ACTIVE_INGREDIENTS.INGREDIENT_NAME, ITEMS.NAME, OUT_SCRIPTS.BARCODE_ID)  AS T1 " +
                    "right join (SELECT ITEM_INGREDIENTS.INGREDIENT_ID as [NSN], ACTIVE_INGREDIENTS.INGREDIENT_NAME as [ACTIVE], ITEMS.NAME as [NAME], " +
                    "sum(DATES_ADDED.QUANTITY) as [QUANTITY], DATES_ADDED.BARCODE_ID as BARCODE_ID from DATES_ADDED, ITEM_INGREDIENTS, ITEMS, ACTIVE_INGREDIENTS " +
                    "where DATES_ADDED.DATE between #01/01/1970# AND #" +
                    toDate.SelectedDate.Value.ToString() + "# AND DATES_ADDED.BARCODE_ID = ITEM_INGREDIENTS.BARCODE_ID " +
                    "AND ITEMS.BARCODE_ID = ITEM_INGREDIENTS.BARCODE_ID AND ACTIVE_INGREDIENTS.INGREDIENT_ID= ITEM_INGREDIENTS.INGREDIENT_ID " +
                    "group by ITEM_INGREDIENTS.INGREDIENT_ID, ACTIVE_INGREDIENTS.INGREDIENT_NAME, ITEMS.NAME, DATES_ADDED.BARCODE_ID)  AS T2 " +
                    "on T1.BARCODE_ID=[T2].[BARCODE_ID] ORDER BY T2.ACTIVE; ";
                review.Connection = conn; 
                conn.Open();
                using (OleDbDataReader reader = review.ExecuteReader())
                    while (reader.Read())
                    {
                        review_active.Add(reader.GetString(2));
                        review_nsn.Add(reader.GetString(0));
                        review_name.Add(reader.GetString(3));
                        review_quantity.Add(reader.GetString(4));
                    }
            }
            int max_active = 0, max_nsn = 0, max_name = 0;
            for (int i = 0; i < review_active.Count; ++i)
            {
                if (max_active < review_active[i].Length) max_active = review_active[i].Length;
                if (max_nsn < review_nsn[i].Length) max_nsn = review_nsn[i].Length;
                if (max_name < review_name[i].Length) max_name = review_name[i].Length;
            }
            int spacing = 2;
            int max_len = 30;
            for (int i = 0; i < review_active.Count; ++i)
            {
                string a = review_active[i].PadRight(max_active + spacing, ' ');
                string b = review_nsn[i].PadRight(max_nsn + spacing, ' ');
                string c = review_name[i].PadRight(max_name + spacing, ' ');
                string d = review_quantity[i];
                if (a.Length > max_len ) a = a.Truncate(max_len );
                if (b.Length > max_len) b = b.Truncate(max_len );
                if (c.Length > max_len) c = c.Truncate(max_len );
                if (d.Length > max_len) d = d.Truncate(max_len );

                if (a.Length == max_len) a = a.PadRight(max_len + spacing, ' ');
                if (b.Length == max_len) b = b.PadRight(max_len + spacing, ' ');
                if (c.Length == max_len) c = c.PadRight(max_len + spacing, ' ');
                if (d.Length == max_len) d = d.PadRight(max_len + spacing, ' ');
                review_lines.Add(b + a + c + d);
            }

            PdfDocument document = new PdfDocument();

            // Sample uses DIN A4, page height is 29.7 cm. We use margins of 2.5 cm.
            LayoutHelper helper = new LayoutHelper(document, XUnit.FromCentimeter(2.5), XUnit.FromCentimeter(29.7 - 2.5));
            XUnit left = XUnit.FromCentimeter(2.5);

            // Random generator with seed value, so created document will always be the same.
            Random rand = new Random(42);

            const int headerFontSize = 20;
            const int normalFontSize = 10;

            XFont fontHeader = new XFont("Consolas", headerFontSize, XFontStyle.Bold);
            XFont fontNormal = new XFont("Consolas", normalFontSize, XFontStyle.Regular);

            int totalLines = review_lines.Count;
            XUnit top1 = helper.GetLinePosition(headerFontSize + 5, headerFontSize);
            helper.Gfx.DrawString("In store Review", fontHeader, XBrushes.Black, left, top1, XStringFormats.TopLeft);
            for (int line = 0; line < totalLines; ++line)
            {
                XUnit top = helper.GetLinePosition(normalFontSize + 2, normalFontSize);
                helper.Gfx.DrawString(review_lines[line], fontNormal, XBrushes.Black, left, top, XStringFormats.TopLeft);
            }

            // Save the document... 
            const string filename = "instore.pdf";
            document.Save(filename);
            // ...and start a viewer.
            Process.Start(filename);
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Admin(); //create your new form admin.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }
    }
}
