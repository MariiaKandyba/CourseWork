using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServices
{
    public interface ISerializationService
    {
        string SerializeObjectToXml<T>(T obj);
        T DeserializeObjectFromXml<T>(string xml);
    }
}
