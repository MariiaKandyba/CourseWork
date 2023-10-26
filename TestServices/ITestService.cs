using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServices
{
    public interface ITestService
    {
        Task<Test> LoadTestFromFileAsync(string filePath);
        Task SaveTestToFileAsync(Test test, string filePath);
        Task<Test> AssembleTestAsync(Test test, List<Question> questions);
    }
}
