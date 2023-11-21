using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TestServices
{
    public class SerializationService : ISerializationService
    {
        public string SerializeObjectToXml<T>(T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringWriter writer = new StringWriter())
            {
                xmlSerializer.Serialize(writer, obj);
                return writer.ToString();
            }

        }
        public T DeserializeObjectFromXml<T>(string xml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xml))
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }
    }
}
