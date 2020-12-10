using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class ControlEntity
    {
        private string version;
        private string light_status;
        private string r;
        private string g;
        private string b;

        public string Version
        {
            get { return version; }
            set { version = value; }
        }
        public string Light {
            get { return light_status; }
            set { light_status = value; }
        }
        public string R
        {
            get { return r; }
            set { r = value; }
        }
        public string G
        {
            get { return g; }
            set { g = value; }
        }
        public string B
        {
            get { return b; }
            set { b = value; }

        }
    }
}










