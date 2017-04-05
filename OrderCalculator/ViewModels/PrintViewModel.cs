using OrderCalculator.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace OrderCalculator
{
    public class PrintViewModel : ViewModel, ITabViewModel
    {
        #region Свойства

        public string Header { get { return "Печать"; } }


        public PrintQueueCollection PrinterList { get; private set; }

        private PrintQueue selectedPrinter;

        public PrintQueue SelectedPrinter
        {
            get { return selectedPrinter; }
            set
            {
                if (selectedPrinter != value)
                {
                    selectedPrinter = value;
                    OnPropertyChanged("SelectedPrinter");
                }
            }
        }

        public OrderViewModel Order { get; set; }

        private int copyQuantity;

        public int CopyQuantity
        {
            get { return copyQuantity; }
            set
            {
                if (copyQuantity != value)
                {
                    copyQuantity = value;
                    OnPropertyChanged("CopyQuantity");
                }
            }
        }

        public ViewModel PrintingObject { get; set; }

        #endregion

        public PrintViewModel() : this(new OrderViewModel()) { }

        public PrintViewModel(OrderViewModel order)
        {
            FillPrinterList();
            SetDefaultPrinter();
            CopyQuantity = Settings.Default.CopyQuantity;

            Order = order;
            PrintingObject = Order;
        }

        private void SetDefaultPrinter()
        {
            string defaultPrinterName = Settings.Default.DefaultPrinter;
            if (defaultPrinterName.Length > 0)
            {
                foreach (PrintQueue printer in PrinterList)
                    if (printer.Name.Equals(defaultPrinterName))
                        SelectedPrinter = printer;
            }

            if (SelectedPrinter == null)
            {
                PrintQueue defaultPrinter = LocalPrintServer.GetDefaultPrintQueue();

                foreach (PrintQueue printer in PrinterList)
                    if (printer.Name.Equals(defaultPrinter.Name))
                        SelectedPrinter = printer;
            }
        }

        private void FillPrinterList()
        {
            LocalPrintServer printServer = new LocalPrintServer();
            PrinterList = printServer.GetPrintQueues();
        }

        private EventHandler onClosingTab;

        public event EventHandler ClosingTab
        {
            add { onClosingTab += value; }
            remove { onClosingTab -= value; }
        }

        public void OnClosingTab()
        {
            if (onClosingTab != null)
                onClosingTab(this, EventArgs.Empty);
        }
    }
}
