using System.Xml.Serialization;
namespace VSSystem.Converter.Soap
{
    [XmlRoot(ElementName = "dataSaving", Namespace = "")]
    public class SoapDataSaving
    {
        [XmlElement(ElementName = "prdoductType")]
        public string PrdoductType { get; set; }
        [XmlElement(ElementName = "action")]
        public string Action { get; set; }
        [XmlElement(ElementName = "priority")]
        public string Priority { get; set; }
        [XmlElement(ElementName = "timestamp")]
        public string Timestamp { get; set; }
        [XmlElement(ElementName = "message")]
        public SoapMessage Message { get; set; }
        [XmlAttribute(AttributeName = "mes", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Mes { get; set; }
        [XmlAttribute(AttributeName = "encodingStyle", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public string EncodingStyle { get; set; }
    }
}
