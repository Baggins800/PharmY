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
    /// Interaction logic for CreateReview.xaml
    /// </summary>
    /// 
    public class LayoutHelper
    {
        private readonly PdfDocument _document;
        private readonly XUnit _topPosition;
        private readonly XUnit _bottomMargin;
        private XUnit _currentPosition;

        public LayoutHelper(PdfDocument document, XUnit topPosition, XUnit bottomMargin)
        {
            _document = document;
            _topPosition = topPosition;
            _bottomMargin = bottomMargin;
            // Set a value outside the page - a new page will be created on the first request.
            _currentPosition = bottomMargin + 10000;
        }

        public XUnit GetLinePosition(XUnit requestedHeight)
        {
            return GetLinePosition(requestedHeight, -1f);
        }

        public XUnit GetLinePosition(XUnit requestedHeight, XUnit requiredHeight)
        {
            XUnit required = requiredHeight == -1f ? requestedHeight : requiredHeight;
            if (_currentPosition + required > _bottomMargin)
                CreatePage();
            XUnit result = _currentPosition;
            _currentPosition += requestedHeight;
            return result;
        }

        public XGraphics Gfx { get; private set; }
        public PdfPage Page { get; private set; }

        void CreatePage()
        {
            Page = _document.AddPage();
            Page.Size = PageSize.A4;
            Gfx = XGraphics.FromPdfPage(Page);
            _currentPosition = _topPosition;
        }
    }

    public partial class CreateReview
    {
        public CreateReview()
        {
            InitializeComponent();
            this.fromDate.SelectedDate = DateTime.Today.AddDays(-30);
            this.toDate.SelectedDate = DateTime.Today;
        }

        private void btnbacktoadminreview_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new Admin(); //create your new form main window.
            newForm.Show(); //show the new form.
            this.Close(); //only if you want to close the current form
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<string> review_lines = new List<string>();
            List<string> review_date = new List<string>();
            List<string> review_nsn = new List<string>();
            List<string> review_name = new List<string>();
            List<string> review_patient_name = new List<string>();
            List<string> review_quantity = new List<string>();

            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
            {
                OleDbCommand review = new OleDbCommand();
                review.CommandType = CommandType.Text;
                review.CommandText = "SELECT OUT_SCRIPTS.DATE as [DATE], ITEM_INGREDIENTS.INGREDIENT_ID as [NSN], ITEMS.NAME as [NAME], " +
                    "OUT_SCRIPTS.PATIENT_NAME as [PATIENT_NAME], OUT_SCRIPTS.QUANTITY as [QUANTITY] from OUT_SCRIPTS, ITEM_INGREDIENTS, ITEMS" +
                    " where OUT_SCRIPTS.DATE between #" + fromDate.SelectedDate.Value.ToString() + "# AND #" +
                    toDate.SelectedDate.Value.ToString() + "# AND OUT_SCRIPTS.BARCODE_ID = ITEM_INGREDIENTS.BARCODE_ID" +
                    " AND ITEMS.BARCODE_ID = ITEM_INGREDIENTS.BARCODE_ID" +
                    " order by ITEMS.NAME;";
                review.Connection = conn;
                conn.Open();
                using (OleDbDataReader reader = review.ExecuteReader())
                    while (reader.Read())
                    {
                        review_date.Add(reader.GetDateTime(0).ToString("dd/MM/yyyy"));
                        review_nsn.Add(reader.GetString(1));
                        review_name.Add(reader.GetString(2));
                        review_patient_name.Add(reader.GetString(3));
                        review_quantity.Add(reader.GetInt32(4).ToString());
                    }
            }
            int max_date = 0, max_nsn = 0, max_name = 0, max_patient = 0;
            for (int i = 0; i < review_date.Count; ++i)
            {
                if (max_date < review_date[i].Length) max_date = review_date[i].Length;
                if (max_nsn < review_nsn[i].Length) max_nsn = review_nsn[i].Length;
                if (max_name < review_name[i].Length) max_name = review_name[i].Length;
                if (max_patient < review_patient_name[i].Length) max_patient = review_patient_name[i].Length;
            }
            int spacing = 2;
            for (int i = 0; i < review_date.Count; ++i)
                review_lines.Add(review_date[i].PadRight(max_date + spacing, ' ') + review_nsn[i].PadRight(max_nsn + spacing, ' ')
                    + review_name[i].PadRight(max_name + spacing, ' ') + review_patient_name[i].PadRight(max_patient + spacing, ' ')
                    + review_quantity[i]);

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
            helper.Gfx.DrawString("Out Review", fontHeader, XBrushes.Black, left, top1, XStringFormats.TopLeft);
            for (int line = 0; line < totalLines; ++line)
            {
                XUnit top = helper.GetLinePosition(normalFontSize + 2, normalFontSize);
                helper.Gfx.DrawString(review_lines[line], fontNormal, XBrushes.Black, left, top, XStringFormats.TopLeft);
            }

            // Save the document... 
            const string filename = "reviewout.pdf";
            document.Save(filename);
            // ...and start a viewer.
            Process.Start(filename);
        }

        private void review_in_Click(object sender, RoutedEventArgs e)
        {
            List<string> review_lines = new List<string>();
            List<string> review_date = new List<string>();
            List<string> review_nsn = new List<string>();
            List<string> review_name = new List<string>();

            List<string> review_quantity = new List<string>();

            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
            {
                OleDbCommand review = new OleDbCommand();
                review.CommandType = CommandType.Text;
                review.CommandText = "SELECT DATES_ADDED.DATE as [DATE], ITEM_INGREDIENTS.INGREDIENT_ID as [NSN], ITEMS.NAME as [NAME], cStr(SUM(DATES_ADDED.QUANTITY)) as [QUANTITY] " +
                    "from DATES_ADDED, ITEM_INGREDIENTS, ITEMS " +
                    "where DATES_ADDED.DATE between #" + fromDate.SelectedDate.Value.ToString() + "# AND #" + toDate.SelectedDate.Value.ToString() + "#" + " " +
                    "AND DATES_ADDED.BARCODE_ID = ITEM_INGREDIENTS.BARCODE_ID AND ITEMS.BARCODE_ID = ITEM_INGREDIENTS.BARCODE_ID " +
                    "group by DATES_ADDED.BARCODE_ID,DATES_ADDED.DATE, ITEM_INGREDIENTS.INGREDIENT_ID,ITEMS.NAME " +
                    "order by  ITEMS.NAME; ";
                review.Connection = conn;
                conn.Open();
                using (OleDbDataReader reader = review.ExecuteReader())
                    while (reader.Read())
                    {
                        review_date.Add(reader.GetDateTime(0).ToString("dd/MM/yyyy"));
                        review_nsn.Add(reader.GetString(1));
                        review_name.Add(reader.GetString(2));
                        review_quantity.Add(reader.GetString(3));
                    }
            }
            int max_date = 0, max_nsn = 0, max_name = 0;
            for (int i = 0; i < review_date.Count; ++i)
            {
                if (max_date < review_date[i].Length) max_date = review_date[i].Length;
                if (max_nsn < review_nsn[i].Length) max_nsn = review_nsn[i].Length;
                if (max_name < review_name[i].Length) max_name = review_name[i].Length;
            }
            int spacing = 2;
            for (int i = 0; i < review_date.Count; ++i)
                review_lines.Add(review_date[i].PadRight(max_date + spacing, ' ') + review_nsn[i].PadRight(max_nsn + spacing, ' ')
                    + review_name[i].PadRight(max_name + spacing, ' ') + review_quantity[i]);

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
            helper.Gfx.DrawString("In Review", fontHeader, XBrushes.Black, left, top1, XStringFormats.TopLeft);
            for (int line = 0; line < totalLines; ++line)
            {
                XUnit top = helper.GetLinePosition(normalFontSize + 2, normalFontSize);
                helper.Gfx.DrawString(review_lines[line], fontNormal, XBrushes.Black, left, top, XStringFormats.TopLeft);
            }

            // Save the document... 
            const string filename = "inreview.pdf";
            document.Save(filename);
            // ...and start a viewer.
            Process.Start(filename);
        }
        private void review_out_departments(string department_id)
        {
            List<string> review_lines = new List<string>();
            List<string> review_date = new List<string>();
            List<string> review_nsn = new List<string>();
            List<string> review_name = new List<string>();
            List<string> review_patient_name = new List<string>();
            List<string> review_quantity = new List<string>();

            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["PharmY"].ConnectionString))
            {
                OleDbCommand review = new OleDbCommand();
                review.CommandType = CommandType.Text;
                review.CommandText = "SELECT OUT_SCRIPTS.DATE as [DATE], ITEM_INGREDIENTS.INGREDIENT_ID as [NSN], ITEMS.NAME, " +
                    "OUT_SCRIPTS.PATIENT_NAME as [PATIENT_NAME], OUT_SCRIPTS.QUANTITY as [QUANTITY] from OUT_SCRIPTS, ITEM_INGREDIENTS, ITEMS" +
                    " where OUT_SCRIPTS.DATE between #" + fromDate.SelectedDate.Value.ToString() + "# AND #" +
                    toDate.SelectedDate.Value.ToString() + "# AND OUT_SCRIPTS.BARCODE_ID = ITEM_INGREDIENTS.BARCODE_ID" + " AND OUT_SCRIPTS.DEPARTMENT_ID =  " + department_id +
                    " AND ITEMS.BARCODE_ID = ITEM_INGREDIENTS.BARCODE_ID" +
                    " order by OUT_SCRIPTS.DATE,ITEMS.NAME;";
                review.Connection = conn;
                conn.Open();
                using (OleDbDataReader reader = review.ExecuteReader())
                    while (reader.Read())
                    {
                        review_date.Add(reader.GetDateTime(0).ToString("dd/MM/yyyy"));
                        review_nsn.Add(reader.GetString(1));
                        review_name.Add(reader.GetString(2));
                        review_patient_name.Add(reader.GetString(3));
                        review_quantity.Add(reader.GetInt32(4).ToString());
                    }
            }
            int max_date = 0, max_nsn = 0, max_name = 0, max_patient = 0;
            for (int i = 0; i < review_date.Count; ++i)
            {
                if (max_date < review_date[i].Length) max_date = review_date[i].Length;
                if (max_nsn < review_nsn[i].Length) max_nsn = review_nsn[i].Length;
                if (max_name < review_name[i].Length) max_name = review_name[i].Length;
                if (max_patient < review_patient_name[i].Length) max_patient = review_patient_name[i].Length;
            }
            int spacing = 2;
            for (int i = 0; i < review_date.Count; ++i)
                review_lines.Add(review_date[i].PadRight(max_date + spacing, ' ') + review_nsn[i].PadRight(max_nsn + spacing, ' ')
                    + review_name[i].PadRight(max_name + spacing, ' ') + review_quantity[i]);

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
            if (department_id == "5")
                helper.Gfx.DrawString(" Out Ward A", fontHeader, XBrushes.Black, left, top1, XStringFormats.TopLeft);
            if (department_id == "6")
                helper.Gfx.DrawString(" Out Ward B", fontHeader, XBrushes.Black, left, top1, XStringFormats.TopLeft);
            for (int line = 0; line < totalLines; ++line)
            {
                XUnit top = helper.GetLinePosition(normalFontSize + 2, normalFontSize);
                helper.Gfx.DrawString(review_lines[line], fontNormal, XBrushes.Black, left, top, XStringFormats.TopLeft);
            }

            // Save the document... 
            string filename = "wardA.pdf";
            if (department_id == "6")
                filename = "wardB.pdf";
            document.Save(filename);
            // ...and start a viewer.
            Process.Start(filename);
        }
        private void btn_ward_a_Click(object sender, RoutedEventArgs e)
        {
            review_out_departments("5");
        }

        private void btn_ward_b_Click(object sender, RoutedEventArgs e)
        {
            review_out_departments("6");
        }

        private void btn_total_out_Click(object sender, RoutedEventArgs e)
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

                review.CommandText = "SELECT ITEM_INGREDIENTS.INGREDIENT_ID as [NSN], ACTIVE_INGREDIENTS.INGREDIENT_NAME as [ACTIVE], left(ITEMS.NAME,255) as [NAME], " +
                    "cStr(sum(OUT_SCRIPTS.QUANTITY)) as [QUANTITY], OUT_SCRIPTS.BARCODE_ID as BARCODE_ID from OUT_SCRIPTS, ITEM_INGREDIENTS, ITEMS, " +
                    "ACTIVE_INGREDIENTS where OUT_SCRIPTS.DATE between #" + fromDate.SelectedDate.Value.ToString() +"# AND #" + toDate.SelectedDate.Value.ToString() +
                    "# AND OUT_SCRIPTS.BARCODE_ID = ITEM_INGREDIENTS.BARCODE_ID " +
                    "AND ITEMS.BARCODE_ID = ITEM_INGREDIENTS.BARCODE_ID AND ACTIVE_INGREDIENTS.INGREDIENT_ID= ITEM_INGREDIENTS.INGREDIENT_ID " +
                    "group by ITEM_INGREDIENTS.INGREDIENT_ID, ACTIVE_INGREDIENTS.INGREDIENT_NAME, ITEMS.NAME, OUT_SCRIPTS.BARCODE_ID " +
                    "order by ACTIVE_INGREDIENTS.INGREDIENT_NAME; ";
                review.Connection = conn;
                conn.Open();
                using (OleDbDataReader reader = review.ExecuteReader())
                    while (reader.Read())
                    {
                        review_active.Add(reader.GetString(1));
                        review_nsn.Add(reader.GetString(0));
                        review_name.Add(reader.GetString(2));
                        review_quantity.Add(reader.GetString(3));
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
                if (a.Length > max_len) a = a.Truncate(max_len);
                if (b.Length > max_len) b = b.Truncate(max_len);
                if (c.Length > max_len) c = c.Truncate(max_len);
                if (d.Length > max_len) d = d.Truncate(max_len);

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
            helper.Gfx.DrawString("Total Out", fontHeader, XBrushes.Black, left, top1, XStringFormats.TopLeft);
            for (int line = 0; line < totalLines; ++line)
            {
                XUnit top = helper.GetLinePosition(normalFontSize + 2, normalFontSize);
                helper.Gfx.DrawString(review_lines[line], fontNormal, XBrushes.Black, left, top, XStringFormats.TopLeft);
            }

            // Save the document... 
            const string filename = "totalout.pdf";
            document.Save(filename);
            // ...and start a viewer.
            Process.Start(filename);
        }
    }
}
