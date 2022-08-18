using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using VSSystem.Converter.Soap;

namespace VSSystem
{
    public class SoapConverter
    {
        public static string SerializeObject(object src, Encoding encoding = default)
        {
            try
            {

                if (src == null)
                    return string.Empty;
                if (encoding == null)
                    encoding = Encoding.UTF8;



                var envelope = new SoapEnvelope<object>();
                envelope.Body = new SoapBody<object>();
                envelope.Body.obj = src;

                Type srcType = src.GetType();

                Type envType = envelope.GetType();

                XmlTypeMapping typeMapping = new SoapReflectionImporter()
                .ImportTypeMapping(envType);

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                XmlWriterSettings writerSettings = new XmlWriterSettings();

                using (MemoryStream ms = new MemoryStream())
                {
                    StreamWriter xmlStream;
                    xmlStream = new StreamWriter(ms, encoding);
                    var xmlWr = XmlWriter.Create(xmlStream, writerSettings);
                    XmlSerializer serializer = new XmlSerializer(typeMapping);
                    serializer.Serialize(xmlWr, src, ns);
                    return encoding.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static TResult DeserializeObject<TResult>(string xmlString, Encoding encoding = default, string defaultNameSpace = "")
        {
            try
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                TResult result = default;

                using (MemoryStream xmlStream = new MemoryStream(encoding.GetBytes(xmlString)))
                {
                    XmlTypeMapping typeMapping = new SoapReflectionImporter(defaultNameSpace).ImportTypeMapping(typeof(TResult));
                    //typeMapping.XsdTypeNamespace = defaultNameSpace;
                    XmlSerializer serializer = new XmlSerializer(typeMapping);
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
    }
}