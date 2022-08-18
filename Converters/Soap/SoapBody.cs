using System.Xml.Serialization;

namespace VSSystem.Converter.Soap
{
    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class SoapBody<T>
    {
        //[XmlElement(ElementName = "dataSaving", Namespace = "http://message.test.testing.fi/")]
        public T obj { get; set; }
    }
}

