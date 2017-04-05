using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCalculator
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        #region Поля и свойства

        #region IsModified

        protected bool isModified = false;

        public bool IsModified
        {
            get { return isModified; }
            set
            {
                if (isModified != value)
                {
                    isModified = value;
                    OnPropertyChanged("IsModified");
                }
            }
        }

        #endregion

        #endregion

        #region Реализация INotifyPropertyChanged

        private PropertyChangedEventHandler onPropertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { onPropertyChanged += value; }
            remove { onPropertyChanged -= value; }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (onPropertyChanged != null)
                onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName != "IsModified")
                IsModified = true;
        }

        #endregion
    }
}
