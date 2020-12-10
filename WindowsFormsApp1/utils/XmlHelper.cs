using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
namespace WindowsFormsApp1.utils
{
    public static class XmlHelper<T>
    {

        public static string XmlSerialize(object o, Encoding encoding)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializeInternal(stream, o, encoding);

                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }


        public static T XmlDeserialize(string s, Encoding encoding)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("s");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(s)))
            {
                using (StreamReader sr = new StreamReader(ms, encoding))
                {
                    return (T)mySerializer.Deserialize(sr);
                }
            }
        }

        private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
        {
            if (o == null)
                throw new ArgumentNullException("o");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer serializer = new XmlSerializer(o.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            settings.Encoding = encoding;
            settings.IndentChars = " ";

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, o);
                writer.Close();
            }
        }


        public static void XmlSerializeToFile(object o, string path, Encoding encoding)
        {
            if(string.IsNullOrEmpty(path) )
                throw new ArgumentNullException("path");
                
                using(FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write) ) {
                XmlSerializeInternal(file, o, encoding);
            }
        }

        public static T XmlDeserializeFromFile(string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (encoding == null)
                throw new ArgumentNullException("encoding");
        
            string xml = File.ReadAllText(path, encoding);
            return XmlDeserialize(xml, encoding);
        }
    }
}