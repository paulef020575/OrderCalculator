using Microsoft.Win32;
using OrderCalculator.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCalculator
{
    public class OrderViewModel : ViewModel, ITabViewModel
    {
        #region Свойства

        private string fileName = "";

        public Order Order { get; private set; }

        public string Name
        {
            get { return Order.Name; }
            set
            {
                if (Order.Name != value)
                {
                    Order.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Header
        {
            get
            {
                string fileToHeader = "";
                if (fileName.Length > 0)
                    fileToHeader = " (" + Path.GetFileName(fileName) + ")";

                return Name;
            }
        }

        public string Customer
        {
            get { return Order.Customer; }
            set
            {
                if (Order.Customer != value)
                {
                    Order.Customer = value;
                    OnPropertyChanged("Customer");
                }
            }
        }

        public DateTime CreateDate { get { return Order.CreateDate; } }

        public decimal Discount
        {
            get { return Order.Discount; }
            set
            {
                if (Order.Discount != value)
                {
                    Order.Discount = value;
                    OnPropertyChanged("Discount");
                    OnPropertyChanged("Amount");
                }
            }
        }

        public ObservableCollection<DetailViewModel> DetailList { get; private set; }

        private DetailViewModel selectedItem;

        public DetailViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public bool HasSelectedItem { get { return (SelectedItem != null); } }

        public decimal Cost
        {
            get
            {
                decimal cost = 0m;

                foreach (DetailViewModel detail in DetailList)
                {
                    cost += detail.Amount;
                }

                return cost;
            }
        }

        public decimal Amount { get { return Cost * (100m - Discount) / 100m; } }

        public decimal Weight
        {
            get
            {
                decimal weight = 0m;

                foreach (DetailViewModel detail in DetailList)
                {
                    weight += detail.WeightFull;
                }

                return weight;
            }
        }
        #endregion

        #region Конструкторы

        public OrderViewModel(Order order)
        {
            Order = order;

            DetailList = new ObservableCollection<OrderCalculator.DetailViewModel>();
            DetailList.CollectionChanged += DetailList_CollectionChanged;
            GetDetailList();

            ClearModifiedState();
        }

        public OrderViewModel()
            : this(new Order())
        {
            IsModified = true;
        }

        public OrderViewModel(string fileName)
            : this(Order.GetFromFile(fileName))
        {
            this.fileName = fileName;

            AddFileNameToLast(fileName);
        }

        #endregion

        #region Методы

        private void GetDetailList()
        {
            foreach (Detail orderDetail in Order.DetailList)
            {
                DetailList.Add(new OrderCalculator.DetailViewModel(orderDetail));
            }
        }

        private void DetailList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (DetailViewModel detail in e.NewItems)
                        detail.PropertyChanged += Detail_PropertyChanged;
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (DetailViewModel detail in e.OldItems)
                        detail.PropertyChanged -= Detail_PropertyChanged;
                    break;
            }

            OnPropertyChanged("Cost");
            OnPropertyChanged("Amount");
            OnPropertyChanged("Weight");
        }

        internal void Save(string fileName)
        {
            this.fileName = fileName;
            Save();


        }

        private void Detail_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsModified = true;
            OnPropertyChanged("Cost");
            OnPropertyChanged("Amount");
            OnPropertyChanged("Weight");
        }


        private void ClearModifiedState()
        {
            foreach (DetailViewModel detail in DetailList)
                detail.IsModified = false;

            IsModified = false;
        }

        private void AddFileNameToLast(string fileName)
        {
            //List<FileItem> newFileList = new List<FileItem>();

            //newFileList.Add(new FileItem(fileName));

            //    List<FileItem> fileList = FileItem.ReadFileList();
            //    int count = 1;

            //    foreach (FileItem file in fileList)
            //    {
            //        if (count < 25 && !string.Equals(file.FullPath, fileName, StringComparison.CurrentCultureIgnoreCase))
            //        {
            //            newFileList.Add(file);
            //            count++;
            //        }
            //    }

            //FileItem.SaveToFile(newFileList);
        }

        #endregion

        #region Команды

        #region AddDetailCommand

        public void AddDetail(DetailViewModel item)
        {
            DetailList.Add(item);
            SelectedItem = item;

        }

        public void AddDetail()
        {
            DetailViewModel item = new OrderCalculator.DetailViewModel();
            if (SelectedItem != null)
                item.Material = SelectedItem.Material;

            AddDetail(item);
        }
        
        internal void AddDetailCopy()
        {
            DetailViewModel item = new OrderCalculator.DetailViewModel(SelectedItem.Detail.GetCopy());

            AddDetail(item);
        }



        private EpvCommand addDetailCommand;

        public EpvCommand AddDetailCommand
        {
            get
            {
                if (addDetailCommand == null)
                    addDetailCommand = new EpvCommand(param => AddDetail());

                return addDetailCommand;
            }
        }

        #endregion

        #region RemoveDetailCommand

        public void RemoveDetail()
        {
            DetailList.Remove(SelectedItem);

            if (DetailList.Count > 0)
                SelectedItem = DetailList[0];
            else
                SelectedItem = null;
        }

        private EpvCommand removeDetailCommand;

        public EpvCommand RemoveDetailCommand
        {
            get
            {
                if (removeDetailCommand == null)
                    removeDetailCommand = new EpvCommand(param => RemoveDetail(), param => HasSelectedItem);

                return removeDetailCommand;
            }
        }

        #endregion

        #region SaveCommand

        public void Save()
        {
            if (fileName == "")
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = "order";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.Filter = "Файлы заказов (*.order)|*.order|Все файлы|*.*";
                dialog.OverwritePrompt = true;
                dialog.Title = "Укажите файл для сохранения";

                if (dialog.ShowDialog() == true)
                {
                    fileName = dialog.FileName;
                }
            }

            Order.DetailList = SaveDetailListToOrder();
            Order.Save(fileName);

            ClearModifiedState();
        }

        private List<Detail> SaveDetailListToOrder()
        {
            List<Detail> detailList = new List<Detail>();
            foreach (DetailViewModel detail in DetailList)
                detailList.Add(detail.Detail);

            return detailList;
        }

        private EpvCommand saveCommand;

        public EpvCommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                    saveCommand = new EpvCommand(param => Save(), param => IsModified);

                return saveCommand;
            }
        }

        public override string ToString()
        {
            return Header + (fileName.Length > 0 ? " - " + Path.GetFileName(fileName) : "");
        }

        #endregion


        #endregion
    }
}
