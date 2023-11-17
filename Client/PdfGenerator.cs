using NetworkDataDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Drawing;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Font = iTextSharp.text.Font;
using DALTest.Entities;

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

            //string arialunitff = Path.Combine(Environment.
            //    GetFolderPath(Environment.SpecialFolder.Fonts), "arialuni.ttf");
            //iTextSharp.text.FontFactory.Register(arialunitff);





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
                    //if (question.Img != string.Empty)
                    //{
                    //    document.Add(new Paragraph($"{question.Img}"));

                    //}
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


                //using (var fs = new FileStream(filePath, FileMode.Create))
                //{
                //    using (var document = new iTextSharp.text.Document())
                //    {
                //        using (var writer = PdfWriter.GetInstance(document, fs))
                //        {
                //            document.Open();
                //            document.Add(new Paragraph($"Is Passed: {testResults.IsPassed}"));
                //            document.Add(new Paragraph($"Title: {testResults.Title}"));
                //            document.Add(new Paragraph($"Author: {testResults.Author}"));
                //            document.Add(new Paragraph($"Description: {testResults.Description}"));
                //            document.Add(new Paragraph($"Info: {testResults.Info}"));
                //            document.Add(new Paragraph($"Loaded Date: {testResults.LoadedDate:yyyy-MM-dd HH:mm}"));
                //            document.Add(new Paragraph($"Pass Percent: {testResults.PassPercent}%"));
                //            document.Add(new Paragraph($"Taken Date: {testResults.TakenDate:yyyy-MM-dd HH:mm}"));

                //            int questionIndex = 0;
                //            // Додайте дані з питань і відповідей
                //            foreach (var question in testResults.Questions)
                //            {
                //                questionIndex++;
                //                document.Add(new Paragraph($"{questionIndex}: {question.QuestionText}"));
                //               if(question.Img != string.Empty)
                //                {
                //                    document.Add(new Paragraph($"{question.Img}"));

                //                }
                //                foreach (var answer in question.Answers)
                //                {

                //                    if(answer.IsChecked)
                //                    {
                //                        document.Add(new Paragraph($"     - {answer.AnswerText}"));

                //                    }
                //                    else
                //                        document.Add(new Paragraph($"     * {answer.AnswerText}"));

                //                }
                //            }
                //            document.Close();

                //        }
                //    }
                //}

                        
                   




            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
           
        }
    }
    
}
