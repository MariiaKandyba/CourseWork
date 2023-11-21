using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TestServices
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
        public byte[]? Img { get; set; }
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
        
    }



}
