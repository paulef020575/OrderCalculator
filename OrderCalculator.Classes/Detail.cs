using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCalculator.Classes
{
    [Serializable]
    public class Detail
    {
        #region Свойства

        public string Name { get; set; }

        public Material Material { get; set; }

        public decimal Price { get; set; }

        public Texture Texture { get; set; }

        public decimal Length { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public int Quantity { get; set; }

        #endregion

        #region Конструкторы

        public Detail()
        {
            Name = "Новая деталь";
            Material = new Material();
            Price = Material.Price;
            Texture = Texture.Empty;
            Length = 0;
            Width = 0;
            Height = 0;
            Quantity = 1;
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return Name + " (" + Quantity.ToString() + ")";
        }

        public Detail GetCopy()
        {
            Detail detail = new Classes.Detail();

            detail.Name = Name;
            detail.Material = Material;
            detail.Price = Price;
            detail.Texture = Texture;
            detail.Length = Length;
            detail.Width = Width;
            detail.Height = Height;
            detail.Quantity = Quantity;

            return detail;
        }

        #endregion
    }
}
