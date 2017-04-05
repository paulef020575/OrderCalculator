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
    public class TextureListViewModel : ObservableCollection<TextureViewModel>, ITabViewModel
    {
        #region Свойства

        public string Header { get { return "Фактуры"; } }

        private bool isCollectionModified = false;

        public bool IsModified
        {
            get
            {
                bool isItemsModified = false;
                foreach (TextureViewModel item in Items)
                    isItemsModified = (isItemsModified || item.IsModified);

                return (isCollectionModified || isItemsModified);
            }
        }

        private TextureViewModel selectedItem;

        public TextureViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedItem"));
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("HasSelectedItem"));
            }
        }

        public bool HasSelectedItem { get { return (SelectedItem != null); } }

        #endregion

        #region Конструктор

        public TextureListViewModel()
        {
            string fileName = CreateFileName();

            List<Texture> textureList = ReadFromFile(fileName);
            CollectionChanged += TextureListViewModel_CollectionChanged;

            foreach (Texture texture in textureList)
                Add(new TextureViewModel(texture));

            ClearModifiedState();
        }

        private void TextureListViewModel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (TextureViewModel item in e.NewItems)
                        item.PropertyChanged += Item_PropertyChanged;
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (TextureViewModel item in e.OldItems)
                        item.PropertyChanged -= Item_PropertyChanged;
                    break;
            }

            isCollectionModified = true;
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsModified"));
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsModified"));
        }

        private List<Texture> ReadFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                return Texture.GetFromFile(fileName);
            }
            else
            {
                return Texture.EmptyList();
            }
        }

        private string CreateFileName()
        {
            string folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                                    "OrderCalculator");//Path.GetDirectoryName(GetType().Assembly.Location);
            return Path.Combine(folderName, Settings.Default.TextureListFile);
        }

        #endregion

        #region Методы

        private void ClearModifiedState()
        {
            isCollectionModified = false;
            foreach (TextureViewModel item in Items)
                item.IsModified = false;

            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsModified"));
        }

        #endregion

        #region Команды

        #region AddCommand

        private EpvCommand addCommand;

        private void AddNewItem()
        {
            TextureViewModel item = new TextureViewModel();
            Add(item);
            SelectedItem = item;
        }

        public EpvCommand AddCommand
        {
            get
            {
                if (addCommand == null)
                    addCommand = new EpvCommand(param => AddNewItem());

                return addCommand;
            }
        }

        #endregion

        #region RemoveCommand

        private EpvCommand removeCommand;

        private void RemoveSelectedItem()
        {
            Remove(SelectedItem);
            SelectedItem = (Count > 0 ? Items[0] : null);
        }

        public EpvCommand RemoveCommand
        {
            get
            {
                if (removeCommand == null)
                    removeCommand = new EpvCommand(param => RemoveSelectedItem(), param => HasSelectedItem);
                return removeCommand;
            }
        }

        #endregion

        #region SaveCommand

        private EpvCommand saveCommand;

        private void Save()
        {
            string fileName = CreateFileName();

            List<Texture> textureList = GetTextureSource();

            Texture.SaveToFile(textureList, fileName);
            ClearModifiedState();
        }

        public List<Texture> GetTextureSource()
        {
            List<Texture> textureList = new List<Texture>();
            foreach (TextureViewModel item in Items)
                textureList.Add(item.Texture);

            return textureList;
        }

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
