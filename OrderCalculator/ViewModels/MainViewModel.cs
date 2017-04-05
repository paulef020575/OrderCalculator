using Microsoft.Win32;
using OrderCalculator.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Xml.Serialization;

namespace OrderCalculator
{
    public class MainViewModel : ViewModel
    {
        public string Header { get { return Order.ToString(); } }

        public MaterialListViewModel MaterialList { get; private set; }

        public List<Material> MaterialSource { get { return MaterialList.GetMaterialSource(); } }

        public TextureListViewModel TextureList { get; private set; }

        public List<Texture> TextureSource { get { return TextureList.GetTextureSource(); } }

        private OrderViewModel order;

        public OrderViewModel Order
        {
            get { return order; }
            private set
            {
                if (order != null)
                {
                    TabCollection.Remove(Order);
                }

                order = value;
                TabCollection.Insert(0, order);
                SetActiveTabToOrder();
                OnPropertyChanged("TabCollection");
            }
        }
        
        public bool IsOrderOpened { get { return (Order != null); } }

        public PrintViewModel PrintTab { get; private set; }

        public ObservableCollection<ITabViewModel> TabCollection { get; private set; }

        public List<FileItem> LastFileItemList { get; private set; }

        public static string LastFileItemListFile
        {
            get
            {
                string folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                                    "OrderCalculator");
                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);

                return Path.Combine(folderName, "lastFile.lst");
            }
        }

        private MainViewModel(OrderViewModel order)
        {
            MaterialList = new OrderCalculator.MaterialListViewModel();
            TextureList = new OrderCalculator.TextureListViewModel();
            PrintTab = new OrderCalculator.PrintViewModel();

            TabCollection = new ObservableCollection<OrderCalculator.ITabViewModel>();
            TabCollection.Add(MaterialList);
            TabCollection.Add(TextureList);

            FillLastFileItemList();

            SetOrder(order);
        }

        public MainViewModel() : this(new OrderViewModel()) { }

        public MainViewModel(string fileName) : this(new OrderViewModel(fileName)) { }

        #region методы

        private void SetOrder(OrderViewModel orderToSet)
        {
            if (Order != null)
                Order = null;

            Order = orderToSet;

            OnPropertyChanged("Header");
            OnPropertyChanged("IsOrderOpened");
        }

        internal void SetNewOrder()
        {
            SetOrder(new OrderCalculator.OrderViewModel());
        }

        internal bool SetOrderFromFile(string fileName)
        {
            try
            {
                SetOrder(new OrderCalculator.OrderViewModel(fileName));
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка открытия файла");
                return false;
            }
        }

        private void SetActiveTab(ITabViewModel tab)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.TabCollection);
            if (collectionView != null)
                collectionView.MoveCurrentTo(tab);
        }

        public void SetActiveTabToOrder()
        {
            SetActiveTab(Order);
        }

        private void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "Файлы заказов (*.order)|*.order|Все файлы|*.*";
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Выберите файл заказа";

            if (openFileDialog.ShowDialog(System.Windows.Application.Current.MainWindow) == true)
            {
                OpenFile(openFileDialog.FileName);
            }
        }

        private void OpenFile(string fileName)
        {
                if (SetOrderFromFile(fileName))
                    AddLastFileItem(fileName);
        }

        private bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        private void AddLastFileItem(string fileName)
        {
            FileItem fileItem = FileItem.CreateFileItem(fileName);

            List<FileItem> newLastFileItemList = new List<FileItem>();
            newLastFileItemList.Add(fileItem);

            foreach (FileItem item in LastFileItemList)
            {
                if (!fileItem.Equals(item) && newLastFileItemList.Count < 25)
                {
                    newLastFileItemList.Add(item);
                }
            }

            LastFileItemList = newLastFileItemList;
            OnPropertyChanged("LastFileItemList");
            SaveLastFileItemList();
        }

        private void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "Файлы заказов (*.order)|*.order|Все файлы|*.*";
            saveFileDialog.DefaultExt = ".order";
            saveFileDialog.FileName = Order.Name;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Title = "Укажите файл для сохранения";

            if (saveFileDialog.ShowDialog(System.Windows.Application.Current.MainWindow) == true)
            {
                Order.Save(saveFileDialog.FileName);
                OnPropertyChanged("Header");

                AddLastFileItem(saveFileDialog.FileName);
            }
        }

        private void FillLastFileItemList()
        {
            if (File.Exists(LastFileItemListFile))
            {
                XmlSerializer formatter = FileItem.GetFormatter();

                using (FileStream fs = new FileStream(LastFileItemListFile, FileMode.OpenOrCreate))
                {
                    LastFileItemList = (List<FileItem>)formatter.Deserialize(fs);
                }
            }
            else
            {
                LastFileItemList = new List<FileItem>();
            }
        }

        private void SaveLastFileItemList()
        {
            XmlSerializer formatter = FileItem.GetFormatter();

            using (FileStream fs = new FileStream(LastFileItemListFile, FileMode.Create))
            {
                formatter.Serialize(fs, LastFileItemList);
            }
        }



        #endregion

        #region Команды

        #region NewCommand

        private EpvCommand newCommand;

        public EpvCommand NewCommand
        {
            get
            {
                if (newCommand == null)
                    newCommand = new EpvCommand(param => SetNewOrder());

                return newCommand;
            }
        }

        #endregion

        #region OpenCommand

        private EpvCommand openCommand;

        public EpvCommand OpenCommand
        {
            get
            {
                if (openCommand == null)
                    openCommand = new EpvCommand(param => Open());

                return openCommand;
            }
        }


        #endregion

        #region OpenFileCommand

        private EpvCommand openFileCommand;

        public EpvCommand OpenFileCommand
        {
            get
            {
                if (openFileCommand == null)
                    openFileCommand = new EpvCommand(param => OpenFile(param.ToString()), param => FileExists(param.ToString()));

                return openFileCommand;
            }
        }


        #endregion

        #region SaveCommand

        private void Save()
        {
            Order.Save();
        }

        private EpvCommand saveCommand;

        public EpvCommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                    saveCommand = new EpvCommand(param => Save(), param => CanSave);
                return saveCommand;
            }
        }

        public bool CanSave { get { return Order.IsModified; } }

        #endregion

        #region SaveAsCommand

        private EpvCommand saveAsCommand;

            public EpvCommand SaveAsCommand
        {
            get
            {
                if (saveAsCommand == null)
                    saveAsCommand = new EpvCommand(param => SaveAs());

                return saveAsCommand;
            }
        }

        #endregion

        #region ExitCommand

        private void Exit() { Application.Current.Shutdown(); }

        private EpvCommand exitCommand;

        public EpvCommand ExitCommand
        {
            get
            {
                if (exitCommand == null)
                    exitCommand = new EpvCommand(param => Exit());

                return exitCommand;
            }
        }

        #endregion

        #region PrintCommand

        private void Print()
        {
            if (PrintTab != null && TabCollection.Contains(PrintTab))
            {
                TabCollection.Remove(PrintTab);
                PrintTab.ClosingTab -= PrintTab_ClosingTab;
            }

            PrintTab = new OrderCalculator.PrintViewModel(Order);
            PrintTab.ClosingTab += PrintTab_ClosingTab;
            TabCollection.Add(PrintTab);


            OnPropertyChanged("TabCollection");
            SetActiveTab(PrintTab);
        }

        private void PrintTab_ClosingTab(object sender, EventArgs e)
        {
            PrintViewModel printTab = (PrintViewModel)sender;
            TabCollection.Remove(printTab);
            printTab.ClosingTab -= PrintTab_ClosingTab;

            SetActiveTabToOrder();
        }

        private EpvCommand printCommand;

        public EpvCommand PrintCommand
        {
            get
            {
                if (printCommand == null)
                {
                    printCommand = new EpvCommand(param => Print());
                }

                return printCommand;
            }
        }

        #endregion

        #region AddDetailCommand

        private void AddDetailToOrder()
        {
            Order.AddDetail();
        }

        private EpvCommand addDetailCommand;

        public EpvCommand AddDetailCommand
        {
            get
            {
                if (addDetailCommand == null)
                    addDetailCommand = new EpvCommand(param => AddDetailToOrder(), param => IsOrderOpened);

                return addDetailCommand;
            }
        }

        #endregion

        #region AddDetailCopyCommand

        private void AddDetailCopy()
        {
            Order.AddDetailCopy();
        }

        private EpvCommand addDetailCopyCommand;

        public EpvCommand AddDetailCopyCommand
        {
            get
            {
                if (addDetailCopyCommand == null)
                    addDetailCopyCommand = new EpvCommand(param => AddDetailCopy(), param => Order.HasSelectedItem);
                return addDetailCopyCommand;
            }
        }

        #endregion

        #region RemoveDetailCommand

        private void RemoveDetailFromOrder()
        {
            Order.RemoveDetail();
        }

        private EpvCommand removeDetailCommand;

        public EpvCommand RemoveDetailCommand
        {
            get
            {
                if (removeDetailCommand == null)
                    removeDetailCommand = new EpvCommand(param => RemoveDetailFromOrder(), param => Order.HasSelectedItem);

                return removeDetailCommand;
            }
        }

        #endregion

        #endregion

    }
}
