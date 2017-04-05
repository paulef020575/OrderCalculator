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
    public class FileItem
    {
        [NonSerialized]
        private FileSystemInfo systemInfo;

        public FileSystemInfo SystemInfo
        {
            get
            {
                if (systemInfo == null)
                    systemInfo = new FileInfo(Initializing);
                return systemInfo;
            }
        }

        public string Name { get { return SystemInfo.Name; } }

        public string FullName
        {
            get { return SystemInfo.FullName; } 
        }
        
        public bool IsFolder { get { return (SystemInfo is DirectoryInfo); } }

        public string Initializing { get; set; }

        public DateTime OpenDate { get; set; }

        public string ShortName
        {
            get
            {
                int length = 50;

                if (FullName.Length <= length) return FullName;

                List<string> pathParts = new List<string>();

                pathParts.Add(Name);

                DirectoryInfo parentInfo;
                if (IsFolder)
                {
                    parentInfo = ((DirectoryInfo)SystemInfo).Parent;
                }
                else
                {
                    parentInfo = ((FileInfo)SystemInfo).Directory;
                }

                while (parentInfo != null)
                {
                    pathParts.Add(parentInfo.Name);
                    parentInfo = parentInfo.Parent;
                }

                string result = "";

                for (int i = pathParts.Count - 1; i > 0; i--)
                {
                    result = Path.Combine(pathParts[pathParts.Count - 1], "...");

                    for (int j = i - 1; j >= 0; j--)
                    {
                        result = Path.Combine(result, pathParts[j]);
                    }

                    if (result.Length <= length)
                        return result;
                }

                return result;
            }
        }

        public FileItem()
        {
            Initializing = string.Empty;
            OpenDate = DateTime.Now;
        }        

        public FileItem(FileSystemInfo systemInfo) 
            : this()
        {
            this.systemInfo = systemInfo;
            Initializing = this.systemInfo.FullName;
        }

        public static FileItem CreateFileItem(string fullName)
        {
            return new FileItem(new FileInfo(fullName));
        }

        public static FileItem CreateFolderItem(string fullName)
        {
            return new FileItem(new DirectoryInfo(fullName));
        }

        public override string ToString()
        {
            return ShortName;
        }

        public override bool Equals(object obj)
        {
            FileItem fileItem = obj as FileItem;

            if (fileItem != null)
                return string.Equals(FullName, fileItem.FullName, StringComparison.CurrentCultureIgnoreCase);

            return false;
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        public static XmlSerializer GetFormatter()
        {
            return new XmlSerializer(typeof(List<FileItem>));
        }
    }
}
