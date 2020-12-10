using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.dto
{
    class SaveControlDto
    {
        
            private String type ="control";
            private String version;
            private String light_status;
            private String r;
            private String g;
            private String b;

        public string Version { get => version; set => version = value; }
        public string Light_status { get => light_status; set => light_status = value; }
        public string R { get => r; set => r = value; }
        public string G { get => g; set => g = value; }
        public string B { get => b; set => b = value; }

        public SaveControlDto()
        {

        }

        public SaveControlDto(String version,String light_status,String r,String g,String b)
        { 
            this.version = version;
            this.light_status = light_status;
            this.r = r;
            this.g = g;
            this.b = b;
        }


    }
}
