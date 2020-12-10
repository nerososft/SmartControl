using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.constant;
using WindowsFormsApp1.dto;
using WindowsFormsApp1.exceptions;

namespace WindowsFormsApp1.utils
{
    class EntityGen<T>
    {

        public static T dataFileGen(FileType fileType,StreamReader str)
        {
            StringBuilder tmpFile = new StringBuilder();
            switch (fileType)
            {
                case FileType.control:
                    T controldto = System.Activator.CreateInstance<T>();
                    SaveControlDto cc = controldto as SaveControlDto;
                    String line;
                    Dictionary<String, String> control = new Dictionary<string, string>();
                    while ((line = str.ReadLine()) != null)
                    {
                        string[] a = line.Split(':');
                        control.Add(a[0], a[1]);
                    }
                    cc.Light_status = control["light_status"];
                    cc.Version = control["version"];
                    cc.R = control["r"];
                    cc.G = control["g"];
                    cc.B = control["b"];
                
                    return (T)(Object)cc;

                case FileType.smt:

                    T datadto = System.Activator.CreateInstance<T>();
                    SaveDataDto datadd = datadto as SaveDataDto;

                    Dictionary<String, String> data = new Dictionary<string, string>();
                    while ((line = str.ReadLine()) != null)
                    {
                        string[] a = line.Split(':');
                    }

                    datadd.DataType = data["dataType"];
                    datadd.Version = data["version"];
                    datadd.Buffer_size = data["buffer_size"];
                    datadd.Grid_size = data["grid_size"];
                    List<String> ll = new List<String>();
                    List<String> tt = new List<String>();
                    String[] tmp = data["data"].Split(',');
                    for (int i = 0;i<tmp.Length-1;i++) {
                        if (i % 2 == 0)
                        {
                            tt.Add(tmp[i]);
                        }
                        else {
                            ll.Add(tmp[i]);
                        }
                    }


                    return (T)(Object)datadd;

                case FileType.json:
                   
                    String json_string = str.ReadToEnd();
                    return  JsonConvert.DeserializeObject<T>(json_string);

                case FileType.xml:
                  
                    String xml_string = str.ReadToEnd();
                    return XmlHelper<T>.XmlDeserialize(xml_string, Encoding.UTF8);
                  
                default:
                    throw new FileTypeException(CONSTANT.UNKNOWN_FILE_TYPE);
            }
            throw new NotImplementedException();
        }

        public static T ReadFile(String path){
            StreamReader str = new StreamReader(path, Encoding.Default);
            String tmp = path.Split('.')[1];

            switch (tmp) {
                case "smtc":
                    return dataFileGen(FileType.control, str);
                   
                case "smt":
                    return dataFileGen(FileType.smt, str);
                   
                case "json":
                    return dataFileGen(FileType.json, str);
                   
                case "xml":
                    return dataFileGen(FileType.xml, str);
                   
                default:
                    return dataFileGen(FileType.smt, str);
            }
        }

    }
}
