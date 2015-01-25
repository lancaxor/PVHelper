using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PVHelper
{
    class CmdWithHlpComparer : IComparer<CommandWithHelp>
    {
        public int Compare(CommandWithHelp x, CommandWithHelp y)
        {
            return x.Command.CompareTo(y.Command);
        }
    }
}
