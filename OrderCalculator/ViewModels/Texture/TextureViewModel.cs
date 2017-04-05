using OrderCalculator.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCalculator
{
    public class TextureViewModel : ViewModel
    {
        #region Свойства

        public Texture Texture { get; private set; }

        public string Name
        {
            get { return Texture.Name; }
            set
            {
                if (Texture.Name != value)
                {
                    Texture.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public decimal Rate
        {
            get { return Texture.Rate; }
            set
            {
                if (Texture.Rate != value)
                {
                    Texture.Rate = value;
                    OnPropertyChanged("Rate");
                }
            }
        }

        #endregion

        public TextureViewModel(Texture texture)
        {
            Texture = texture;
        }

        public TextureViewModel() 
            : this(new Texture())
        {
            isModified = true;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
