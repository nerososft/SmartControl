using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class DataEntity
    {

        private String power;
        private string r;
        private string g;
        private string b;
        private string t;
        private string l;

        public string L{
            get {return l;}
            set { l = value; }
        }
        public String Power
        {
            get { return power; }
            set { power = value; }
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
         public string B{
            get { return b; }
            set { b = value; }

        }
        public string T
        {
            get { return t; }
            set { t = value; }

        }

    }
}
