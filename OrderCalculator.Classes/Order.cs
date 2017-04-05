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
    public class Order
    {
        #region Свойства

        public string Name { get; set; }

        public string Customer { get; set; }

        public DateTime CreateDate { get; set; }

        public List<Detail> DetailList { get; set; }

        public decimal Discount { get; set; }

        #endregion

        #region Конструктор

        public Order()
        {
            Name = "Новый заказ";
            Customer = "Имя заказчика";
            CreateDate = DateTime.Now;

            DetailList = new List<Classes.Detail>();

            Discount = 0;
        }

        #endregion

        #region Методы

        public static Order GetFromFile(string fileName)
        {
            XmlSerializer formatter = GetFormatter();

            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                return (Order)formatter.Deserialize(fs);
            }
        }

        private static XmlSerializer GetFormatter()
        {
            return new XmlSerializer(typeof(Order));
        }


        public void Save(string fileName)
        {
            XmlSerializer formatter = GetFormatter();

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                formatter.Serialize(fs, this);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
