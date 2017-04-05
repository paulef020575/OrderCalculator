using OrderCalculator.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCalculator
{
    public class MaterialViewModel : ViewModel
    {
        #region Поля и свойства

        public Material Material { get; private set; }

        public string Name
        {
            get { return Material.Name; }
            set
            {
                if (Material.Name != value)
                {
                    Material.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public decimal Price
        {
            get { return Material.Price; }
            set
            {
                if (Material.Price != value)
                {
                    Material.Price = value;
                    OnPropertyChanged("Price");
                }
            }
        }

        public decimal Density
        {
            get { return Material.Density; }
            set
            {
                if (Material.Density != value)
                {
                    Material.Density = value;
                    OnPropertyChanged("Density");
                }
            }
        }

        #endregion

        public MaterialViewModel(Material material)
        {
            Material = material;
        }

        public MaterialViewModel()
            : this(new Material())
        {
            isModified = true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
