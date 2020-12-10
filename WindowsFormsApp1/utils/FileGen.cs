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
    class FileGen<T>
    {
        public static string dataFileGen(FileType v,T dt)
        {
            StringBuilder tmpFile = new StringBuilder();
            switch (v)
            {
                case FileType.control:
                    SaveControlDto controldto = dt as SaveControlDto;

                    tmpFile.Append("type:control\r\n");
                    tmpFile.Append("version:" + controldto.Version+"\r\n");
                    tmpFile.Append("light_status:" + controldto.Light_status + "\r\n");
                    tmpFile.Append("r:" + controldto.R + "\r\n");
                    tmpFile.Append("g:" + controldto.G + "\r\n");
                    tmpFile.Append("b:" + controldto.B + "\r\n");
                    return tmpFile.ToString();
                case FileType.smt:

                    SaveDataDto dto = dt as SaveDataDto;
                    tmpFile.Append("type:data\r\n");
                    tmpFile.Append("dataType:"+dto.DataType+"\r\n");
                    tmpFile.Append("version:" + dto.Version + "\r\n");
                    tmpFile.Append("buffer_size:" + dto.Buffer_size + "\r\n");
                    tmpFile.Append("grid_size:" + dto.Grid_size + "\r\n");
                    tmpFile.Append("data:");

                    for (int j = 0; j < dto.Data_l.Count - 1; j++)
                    {
                        tmpFile.Append(dto.Data_t[j] + "," + dto.Data_l[j]+",");
                    }
                    return tmpFile.ToString();

                case FileType.json:
                     
                     String resultJson = JsonConvert.SerializeObject(dt);
                     return resultJson;
                   
                case FileType.xml:

                    string resultXml = XmlHelper<SaveDataDto>.XmlSerialize(dt, Encoding.UTF8);
                    return resultXml;

                default:
                    throw new FileTypeException(CONSTANT.UNKNOWN_FILE_TYPE);
            }
            throw new NotImplementedException();
        }

        public static void FileWrite(String p, string fileName)
        {

            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sm = new StreamWriter(fs);

            sm.Write(p);
            sm.Flush();
            sm.Close();
            fs.Close();
        }

    }
}
