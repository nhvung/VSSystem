using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace VSSystem.IO
{
    public class XmlFile
    {
        public static string SerializeObject(object src, Encoding encoding = default)
        {
            try
            {

                if (src == null)
                    return string.Empty;
                if (encoding == null)
                    encoding = Encoding.UTF8;

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                XmlWriterSettings writerSettings = new XmlWriterSettings();
                //writerSettings.OmitXmlDeclaration = true;
                using (MemoryStream ms = new MemoryStream())
                {

                    StreamWriter xmlStream;
                    xmlStream = new StreamWriter(ms, encoding);
                    var xmlWr = XmlWriter.Create(xmlStream, writerSettings);
                    XmlSerializer serializer = new XmlSerializer(src.GetType());
                    serializer.Serialize(xmlWr, src, ns);
                    return encoding.GetString(ms.ToArray());
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static TResult DeserializeObject<TResult>(string xmlString, Encoding encoding = default)
        {
            try
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                TResult result = default;
                using (MemoryStream xmlStream = new MemoryStream(encoding.GetBytes(xmlString)))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TResult));
                    result = (TResult)serializer.Deserialize(xmlStream);
                    xmlStream.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static TResult DeserializeObjectFromFile<TResult>(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                    return default;
                TResult result = default;
                using (FileStream xmlStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TResult));
                    result = (TResult)serializer.Deserialize(xmlStream);
                    xmlStream.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static TResult DeserializeObjectFromBytes<TResult>(byte[] binData, Encoding encoding = default)
        {
            try
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                TResult result = default;
                using (MemoryStream ms = new MemoryStream(binData))
                {
                    StreamReader xmlStream = new StreamReader(new MemoryStream(binData), encoding);
                    XmlSerializer serializer = new XmlSerializer(typeof(TResult));
                    result = (TResult)serializer.Deserialize(xmlStream);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SaveObjectToFile(string fileName, object src, Encoding encoding = default)
        {
            try
            {
                if (src == null)
                    return;
                if (encoding == null)
                    encoding = Encoding.UTF8;

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                if (File.Exists(fileName))
                {
                    File.WriteAllBytes(fileName, new byte[0]);
                }
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(src.GetType());
                    serializer.Serialize(fs, src, ns);
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
