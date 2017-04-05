using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OrderCalculator.Classes
{
    /// <summary>
    ///     Материал
    /// </summary>
    [Serializable]
    public class Material : IComparable
    {
        #region Свойства

        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal Density { get; set; }

        #endregion

        public Material(string name, decimal price, decimal density)
        {
            Name = name;
            Price = price;
            Density = density;
        }

        public Material()
            : this("Новый материал", 0m, 3000m)
        { }

        public static List<Material> GetFromFile(string fileName)
        {
            XmlSerializer formatter = CreateFormatter();
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                Material[] materialList = (Material[])formatter.Deserialize(fs);

                return new List<Material>(materialList);
            }
        }

        public static void SaveToFile(List<Material> materialList, string fileName)
        {
            XmlSerializer formatter = CreateFormatter();
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                formatter.Serialize(fs, materialList.ToArray());
            }
        }

        private static XmlSerializer CreateFormatter()
        {
            return new XmlSerializer(typeof(Material[]));
        }

        public static List<Material> EmptyList()
        {
            List<Material> materialList = new List<Material>();
            materialList.Add(new Material());

            return materialList;           
        }

        public int CompareTo(object obj)
        {
            if (obj is Material)
                return Name.CompareTo(((Material)obj).Name);

            return 0;
        }

        public override bool Equals(object obj)
        {
            Material material = obj as Material;

            return (material != null && Name.Equals(material.Name));
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
