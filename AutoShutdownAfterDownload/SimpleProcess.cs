using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoShutdownAfterDownload
{
    public class SimpleProcess
    {
        public string Name { get; private set; }
        public string MainWindowTitle { get; private set; }
        public int ID { get; private set; }

        public SimpleProcess(string name, string mainWindowTitle, int ID)
        {
            this.Name = name;
            this.MainWindowTitle = mainWindowTitle;
            this.ID = ID;
        }
    }
}