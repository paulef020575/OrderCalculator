using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OrderCalculator.Classes
{
    [Serializable]
    public class Texture : IComparable
    {
        #region Свойства

        public string Name { get; set; }

        public decimal Rate { get; set; }

        public static Texture Empty { get { return new Texture("Без полировки", 0m); } }

        #endregion

        #region Конструкторы

        public Texture(string name, decimal rate)
        {
            Name = name;
            Rate = rate;
        }

        public Texture() : this("Новая фактура", 0m) { }

        #endregion

        #region Методы

        public static List<Texture> GetFromFile(string fileName)
        {
            XmlSerializer formatter = GetSerializer();

            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                Texture[] textureList = (Texture[])formatter.Deserialize(fs);

                return new List<Texture>(textureList);
            }
        }

        private static XmlSerializer GetSerializer()
        {
            return new XmlSerializer(typeof(Texture[]));
        }

        public static void SaveToFile(List<Texture> textureList, string fileName)
        {
            XmlSerializer formatter = GetSerializer();

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                formatter.Serialize(fs, textureList.ToArray());
            }
        }

        public static List<Texture> EmptyList()
        {
            List<Texture> textureList = new List<Texture>();
            textureList.Add(new Texture());

            return textureList;
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(obj.ToString());
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            Texture texture = obj as Texture;
            return (texture != null && Name.Equals(texture.Name));
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion
    }
}
