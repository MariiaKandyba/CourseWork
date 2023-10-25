using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDesigner.Services
{
    using System.IO;
    using System.Threading.Tasks;

    namespace TestDesigner.Services
    {
        public class FileService : IFileService
        {
            public async Task<string> LoadFileAsync(string filePath)
            {
                return await File.ReadAllTextAsync(filePath);
            }

            public async Task SaveFileAsync(string filePath, string content)
            {
                await File.WriteAllTextAsync(filePath, content);
            }
        }
    }


}
