using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OrderCalculator
{
    public class CommandViewModel 
    {
        #region Свойства

        public string Header { get; private set; }

        public ICommand Command { get; private set; }

        #endregion

        public CommandViewModel(string header, ICommand command)
        {
            Header = header;
            Command = command;
        }

        public override bool Equals(object obj)
        {
            CommandViewModel command = obj as CommandViewModel;

            if (command != null)
                return Header.Equals(command.Header);

            return false;
        }

        public override int GetHashCode()
        {
            return Header.GetHashCode();
        }

        public override string ToString()
        {
            return Header;
        }
    }
}
