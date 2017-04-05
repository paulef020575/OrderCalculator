using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Printing;
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

namespace OrderCalculator
{
    /// <summary>
    /// Логика взаимодействия для PrintView.xaml
    /// </summary>
    public partial class PrintView : UserControl
    {
        public PrintViewModel ViewModel { get { return (PrintViewModel)this.DataContext; } }

        public PrintView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ObservableCollection<DetailViewModel> detailList = ViewModel.Order.DetailList;

            foreach (DetailViewModel detail in detailList)
            {
                DetailTable.Rows.Add(CreateRow(detail));
            }
        }

        private TableRow CreateRow(DetailViewModel detail)
        {
            TableRow row = new TableRow();
            row.Cells.Add(new TableCell(new Paragraph(new Run(detail.Name))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(detail.Material.ToString()))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(detail.Price.ToString("N2", CultureInfo.CurrentCulture)))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(GetSizes(detail)))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(detail.Texture.ToString()))));

            TableCell cell = new TableCell(new Paragraph(new Run(detail.Cost.ToString("N2", CultureInfo.CurrentCulture))));
            cell.TextAlignment = TextAlignment.Right;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run(detail.Quantity.ToString())));
            cell.TextAlignment = TextAlignment.Right;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run(detail.Amount.ToString("N2", CultureInfo.CurrentCulture))));
            cell.TextAlignment = TextAlignment.Right;
            cell.FontWeight = FontWeights.SemiBold;
            row.Cells.Add(cell);

            cell = new TableCell(new Paragraph(new Run(detail.Weight.ToString("N2", CultureInfo.CurrentCulture))));
            cell.TextAlignment = TextAlignment.Right;
            row.Cells.Add(cell);
            cell = new TableCell(new Paragraph(new Run(detail.WeightFull.ToString("N2", CultureInfo.CurrentCulture))));
            cell.TextAlignment = TextAlignment.Right;
            row.Cells.Add(cell);

            return row;
        }

        private string GetSizes(DetailViewModel detail)
        {
            StringBuilder sizes = new StringBuilder(detail.Length.ToString());
            sizes.Append(" x " + detail.Width.ToString());
            sizes.Append(" x " + detail.Height.ToString());

            return sizes.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                FlowDocument document = (FlowDocument)DocViewer.Document;
                document.PageHeight = dialog.PrintableAreaHeight;
                document.PageWidth = dialog.PrintableAreaWidth;
                document.PagePadding = new Thickness(40);

                dialog.PrintDocument(((IDocumentPaginatorSource)DocViewer.Document).DocumentPaginator, "Печать заказа");
            }

            ViewModel.OnClosingTab();
        }

        private PrintQueue GetPrinter(PrintQueue selectedPrinter)
        {
                LocalPrintServer printServer = new LocalPrintServer();
                PrintQueueCollection printerList = printServer.GetPrintQueues();

                PrintQueue defaultPrinter = LocalPrintServer.GetDefaultPrintQueue();

                foreach (PrintQueue printer in printerList)
                    if (printer.Name.Equals(defaultPrinter.Name))
                        return printer;

                return printServer.DefaultPrintQueue;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ViewModel.OnClosingTab();
        }
    }
}
