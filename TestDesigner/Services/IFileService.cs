using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDesigner.Services
{
    public interface IFileService
    {
        Task<string> LoadFileAsync(string filePath);
        Task SaveFileAsync(string filePath, string content);
    }


}
