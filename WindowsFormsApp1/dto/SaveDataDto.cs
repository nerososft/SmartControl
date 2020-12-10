using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class SaveDataDto
    {
        private String type  = "data";
        private String dataType;
        private String version;
        private String buffer_size;
        private String grid_size;
        private List<String> data_t;
        private List<String> data_l;

        public SaveDataDto()
        {

        }

        public SaveDataDto(String dataType,String version,String buffer_size,String grid_size,List<String> data_t,List<String> data_l)
        {
            this.dataType  = dataType;
            this.version = version;
            this.buffer_size = buffer_size;
            this.grid_size = grid_size;
            this.data_t = data_t;
            this.data_l  = data_l;
        }


        public string Type { get => type; set => type = value; }
        public string DataType { get => dataType; set => dataType = value; }
        public string Version { get => version; set => version = value; }
        public string Buffer_size { get => buffer_size; set => buffer_size = value; }
        public string Grid_size { get => grid_size; set => grid_size = value; }
        public List<string> Data_t { get => data_t; set => data_t = value; }
        public List<string> Data_l { get => data_l; set => data_l = value; }
    }
}
