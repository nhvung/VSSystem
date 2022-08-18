using System.Xml.Serialization;

[XmlRoot(ElementName = "message")]
public class SoapMessage
{
    [XmlAttribute(AttributeName = "priority")]
    public string Priority { get; set; }
    [XmlAttribute(AttributeName = "messageTimestamp")]
    public string MessageTimestamp { get; set; }
    [XmlText]
    public string Text { get; set; }
}