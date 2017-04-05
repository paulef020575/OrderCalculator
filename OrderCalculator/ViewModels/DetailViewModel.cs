using OrderCalculator.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCalculator
{
    public class DetailViewModel : ViewModel
    {
        #region Свойства

        public Detail Detail { get; private set; }

        public string Name
        {
            get { return Detail.Name; }
            set
            {
                if (Detail.Name != value)
                {
                    Detail.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public Material Material
        {
            get { return Detail.Material; }
            set
            {
                if (Detail.Material == null || !Detail.Material.Equals(value))
                {
                    Detail.Material = value;
                    Price = value.Price;
                    OnPropertyChanged("Material");
                    OnPropertyChanged("Weight");
                    OnPropertyChanged("WeightFull");
                }
            }
        }

        public decimal Price
        {
            get { return Detail.Price; }
            set
            {
                if (Detail.Price != value)
                {
                    Detail.Price = value;
                    OnPropertyChanged("Price");
                    OnPropertyChanged("Cost");
                    OnPropertyChanged("Amount");
                }
            }
        }

        public Texture Texture
        {
            get { return Detail.Texture; }
            set
            {
                if (Detail.Texture == null || !Detail.Texture.Equals(value))
                {
                    Detail.Texture = value;
                    OnPropertyChanged("Texture");
                    OnPropertyChanged("Cost");
                    OnPropertyChanged("Amount");
                }
            }

        }

        public decimal Length
        {
            get { return Detail.Length; }
            set
            {
                if (Detail.Length != value)
                {
                    Detail.Length = value;
                    OnPropertyChanged("Length");
                    OnPropertyChanged("Cost");
                    OnPropertyChanged("Amount");
                    OnPropertyChanged("Weight");
                    OnPropertyChanged("WeightFull");
                }
            }
        }

        public decimal Width
        {
            get { return Detail.Width; }
            set
            {
                if (Detail.Width != value)
                {
                    Detail.Width = value;
                    OnPropertyChanged("Width");
                    OnPropertyChanged("Cost");
                    OnPropertyChanged("Amount");
                    OnPropertyChanged("Weight");
                    OnPropertyChanged("WeightFull");
                }
            }
        }

        public decimal Height
        {
            get { return Detail.Height; }
            set
            {
                if (Detail.Height != value)
                {
                    Detail.Height = value;
                    OnPropertyChanged("Height");
                    OnPropertyChanged("Cost");
                    OnPropertyChanged("Amount");
                    OnPropertyChanged("Weight");
                    OnPropertyChanged("WeightFull");
                }
            }
        }

        public int Quantity
        {
            get { return Detail.Quantity; }
            set
            {
                if (Detail.Quantity != value)
                {
                    Detail.Quantity = value;
                    OnPropertyChanged("Quantity");
                    OnPropertyChanged("Amount");
                    OnPropertyChanged("WeightFull");
                }
            }
        }

        public decimal Weight
        {
            get { return Material.Density * Length * Width * Height / (decimal)Math.Pow(1000, 3); }
        }

        public decimal WeightFull
        {
            get { return Weight * (decimal)Quantity; }
        }

        public decimal Cost
        {
            get
            {
                return Price * (100m + Texture.Rate) * Length * Width * Height / 100m / (decimal)Math.Pow(1000, 3);
            }
        }

        public decimal Amount { get { return Cost * (decimal)Quantity; } }


        #endregion

        #region Конструктор

        public DetailViewModel(Detail detail)
        {
            Detail = detail;
        }

        public DetailViewModel()
            : this(new Detail())
        {
            isModified = true;
        }

        #endregion

        public override string ToString()
        {
            return Detail.ToString();
        }
    }
}
