using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PVHelper
{
    class CommandWithHelp
    {
        String cmd;
        String CmdHelp;

        public String Command { get { return this.cmd; } set { this.cmd = value; } }
        public String CommandHepString { get { return this.CmdHelp; } set { this.CmdHelp = value; } }     
    }
}
