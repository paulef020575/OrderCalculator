using OrderCalculator.Classes;
using OrderCalculator.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCalculator
{
    public class MaterialListViewModel : ObservableCollection<MaterialViewModel>, ITabViewModel
    {
        #region Свойства

        public string Header { get { return "Материалы"; } }

        private bool isCollectionModified = false;

        public bool IsModified
        {
            get
            {
                bool isItemsModified = false;

                foreach (MaterialViewModel item in Items)
                    isItemsModified = isItemsModified || item.IsModified;

                return isItemsModified || isCollectionModified;
            }
        }

        private MaterialViewModel selectedItem;

        public MaterialViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedItem"));
                    OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("HasSelectedItem"));
                }
            }
        }

        public bool HasSelectedItem { get { return (SelectedItem != null); } }

        #endregion

        #region Конструктор

        public MaterialListViewModel()
        {
            string fileName = CreateFileName();

            List<Material> materialList = ReadFromFile(fileName);
            CollectionChanged += MaterialListViewModel_CollectionChanged;

            foreach (Material material in materialList)
            {
                Add(new OrderCalculator.MaterialViewModel(material));
            }

            ClearModifiedState();
        }

        #endregion

        #region Обработчики событий

        /// <summary>
        ///     обрабатывает изменение списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaterialListViewModel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (MaterialViewModel vModel in e.NewItems)
                        vModel.PropertyChanged += Item_PropertyChanged;
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (MaterialViewModel vModel in e.OldItems)
                        vModel.PropertyChanged -= Item_PropertyChanged;
                    break;
            }

            isCollectionModified = true;
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsModified"));
        }

        /// <summary>
        ///     обрабатывает изменение элемента списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsModified"));
        }

        #endregion

        #region Методы

        /// <summary>
        ///     Инициализирует начальный список
        /// </summary>
        /// <param name="fileName">имя файла со списком</param>
        /// <returns>список материалов</returns>
        private List<Material> ReadFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                return Material.GetFromFile(fileName);
            }
            else
            {
                return Material.EmptyList();
            }

        }

        /// <summary>
        ///  возвращает имя файла со спи
        /// </summary>
        /// <returns></returns>
        private string CreateFileName()
        {
            string folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                                    "OrderCalculator");//Path.GetDirectoryName(GetType().Assembly.Location);
            return Path.Combine(folderName, Settings.Default.MaterialListFile);
        }

        private void ClearModifiedState()
        {
            isCollectionModified = false;

            foreach (MaterialViewModel vModel in Items)
                vModel.IsModified = false;

            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsModified"));
        }

        #endregion

        #region Команды

        #region AddItem

        private EpvCommand addItem;

        public EpvCommand AddItem
        {
            get
            {
                if (addItem == null)
                    addItem = new EpvCommand(param => AddNewItem());

                return addItem;
            }
        }

        private void AddNewItem()
        {
            MaterialViewModel vModel = new OrderCalculator.MaterialViewModel();
            Add(vModel);
            SelectedItem = vModel;
        }

        #endregion

        #region RemoveItem

        private EpvCommand removeItem;

        public EpvCommand RemoveItemCommand
        {
            get
            {
                if (removeItem == null)
                    removeItem = new EpvCommand(param => RemoveSelectedItem(), param => HasSelectedItem);
                return removeItem;
            }
        }

        private void RemoveSelectedItem()
        {
            Remove(SelectedItem);
            SelectedItem = (Count > 0 ? Items[0] : null);
        }


        #endregion

        #region SaveCommand

        private void Save()
        {
            string fileName = CreateFileName();

            //List<Material> materialList = new List<Material>();

            //foreach (MaterialViewModel vModel in Items)
            //    materialList.Add(vModel.Material);

            Material.SaveToFile(GetMaterialSource(), fileName);

            ClearModifiedState();
        }

        internal List<Material> GetMaterialSource()
        {
            List<Material> materialList = new List<Material>();

            foreach (MaterialViewModel vModel in Items)
                materialList.Add(vModel.Material);

            return materialList;
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

        #endregion

        #endregion
    }
}
