using NetworkDataDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Font = iTextSharp.text.Font;
using DALTest.Entities;
using Image = iTextSharp.text.Image;

namespace Client
{
    public class PdfGenerator
    {
        public static void GeneratePdf(TestResults testResults, string filePath, User user)
        {
            var document = new iTextSharp.text.Document();
            var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

            document.Open();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            BaseFont bf = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font fontNormal = new Font(bf, 12, Font.NORMAL);

            string suc = testResults.IsPassed ? "PASSED!" : "DID NOT PASS";
            var infoString = $"{suc}\n\n" +
             $"Title: {testResults.Title}\n" +
             $"Author: {testResults.Author}\n" +
             $"Description: {testResults.Description}\n" +
             $"Info: {testResults.Info}\n\n" +
             $"Loaded Date: {testResults.LoadedDate:yyyy-MM-dd HH:mm}\n" +
             $"Taken Date: {testResults.TakenDate:yyyy-MM-dd HH:mm}\n\n" +
             $"Requirement: {testResults.PassPercent}%\n" +
             $"You passed: {testResults.ScoredPercent}% ({testResults.PointsGrade} " +
             $"from {testResults.TotalPossiblePoints})\n\n";

            var paragraph = new Paragraph(infoString, fontNormal);
            document.Add(paragraph);


            int questionIndex = 0;
            foreach (var question in testResults.Questions)
            {
                questionIndex++;
                document.Add(new Paragraph($"\n{questionIndex}. {question.QuestionText}"));
                if (question.Img != null)
                {
                    Image img = Image.GetInstance(question.Img);
                    img.ScaleToFit(100f, 100f);
                    document.Add(img);

                }
                foreach (var answer in question.Answers)
                {

                    if (!answer.IsChecked)
                    {
                        document.Add(new Paragraph($"     - {answer.AnswerText}"));

                    }
                    else
                        document.Add(new Paragraph($"     - {answer.AnswerText} - your answer"));

                }
            }
            document.Close();
        }
    }
}
