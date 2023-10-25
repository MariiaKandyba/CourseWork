using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TestDesigner.Models
{
    [XmlRoot(ElementName = "Answer")]
    public class Answer
    {
        [XmlElement(ElementName = "TextAnswer")]
        public string TextAnswer { get; set; }
        [XmlElement(ElementName = "IsRight")]
        public string IsRight { get; set; }
    }

    [XmlRoot(ElementName = "Answers")]
    public class Answers
    {
        [XmlElement(ElementName = "Answer")]
        public List<Answer> Answer { get; set; }
    }

    [XmlRoot(ElementName = "Question")]
    public class Question
    {
        [XmlElement(ElementName = "QuestionText")]
        public string QuestionText { get; set; }
        [XmlElement(ElementName = "Points")]
        public string Points { get; set; }
        [XmlElement(ElementName = "Img")]
        public string Img { get; set; }
        [XmlElement(ElementName = "Answers")]
        public Answers Answers { get; set; }
    }

    [XmlRoot(ElementName = "Questions")]
    public class Questions
    {
        [XmlElement(ElementName = "Question")]
        public List<Question> Question { get; set; }
    }

    [XmlRoot(ElementName = "Test")]
    public class Test
    {
        [XmlElement(ElementName = "Author")]
        public string Author { get; set; }
        [XmlElement(ElementName = "Title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "Info")]
        public string Info { get; set; }
        [XmlElement(ElementName = "PassPercent")]
        public string PassPercent { get; set; }
        [XmlElement(ElementName = "Questions")]
        public Questions Questions { get; set; }
        //[XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        //public string Xsi { get; set; }
        //[XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        //public string Xsd { get; set; }

    //    public static Test Deserialize(string path)
    //    {
    //        XElement test = XDocument.Parse(path).Root;
    //        string author = test.Element("Author")?.Value;
    //        string title = test.Element("Title")?.Value;
    //        string description = test.Element("Description")?.Value;
    //        string info = test.Element("Info")?.Value;
    //        int passPercent = Convert.ToInt32(test.Element("PassPercent")?.Value);

    //        var loadedTest = new Test
    //        {
    //            Author = author,
    //            Title = title,
    //            Description = description,
    //            Info = info,
    //            PassPercent = passPercent.ToString()
    //        };

    //        var questions = test.Element("Questions")?.Elements("Question")
    //            .Select(q => new Question
    //            {
    //                QuestionText = q.Element("QuestionText")?.Value,
    //                Points = q.Element("Points")?.Value,
    //                Img = q.Element("Img")?.Value,
    //                Answers = new Answers
    //                {
    //                    Answer = q.Element("Answers")?.Elements("Answer")
    //                        .Select(a => new Answer
    //                        {
    //                            TextAnswer = a.Element("TextAnswer")?.Value,
    //                            IsRight = a.Element("IsRight")?.Value
    //                        })
    //                        .ToList()
    //                }
    //            })
    //            .ToList();

    //        loadedTest.Questions = new Questions
    //        {
    //            Question = questions
    //        };

    //        return loadedTest;
    //    }

       
    //    public static string SerializeObjectToXml<T>(T obj)
    //{
    //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
    //    using (StringWriter writer = new StringWriter())
    //    {
    //        xmlSerializer.Serialize(writer, obj);
    //        return writer.ToString();
    //    }
    }



}
