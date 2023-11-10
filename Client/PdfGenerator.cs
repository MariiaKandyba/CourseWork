using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using NetworkDataDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public class PdfGenerator
    {
        public static void GeneratePdf(TestResults testResults, string filePath)
        {
            try
            {
                using (var writer = new PdfWriter(filePath))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new iText.Layout.Document(pdf);

                        document.Add(new Paragraph($"Title: {testResults.Title}"));
                        document.Add(new Paragraph($"Author: {testResults.Author}"));
                        document.Add(new Paragraph($"Description: {testResults.Description}"));
                        document.Add(new Paragraph($"Info: {testResults.Info}"));
                        document.Add(new Paragraph($"Pass Percent: {testResults.PassPercent}%"));
                        document.Add(new Paragraph($"Loaded Date: {testResults.LoadedDate:yyyy-MM-dd HH:mm}"));
                        document.Add(new Paragraph($"Is Passed: {testResults.IsPassed}"));
                        document.Add(new Paragraph($"Taken Date: {testResults.TakenDate:yyyy-MM-dd HH:mm}"));

                        // Додайте дані з питань і відповідей
                        foreach (var question in testResults.Questions)
                        {
                            document.Add(new Paragraph($"Question Text: {question.QuestionText}"));
                            document.Add(new Paragraph($"Img: {question.Img}"));

                            foreach (var answer in question.Answers)
                            {
                                document.Add(new Paragraph($"  Answer Text: {answer.AnswerText}"));
                                document.Add(new Paragraph($"  Is Checked: {answer.IsChecked}"));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Access denied!");
            }
           
        }
    }
    
}
