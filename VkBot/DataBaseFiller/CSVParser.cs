using System;
using System.Text;
using DataBaseAccess.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using VkBot.Data.Entities;

namespace DataBaseFiller
{
    public class CSVParser
    {
        private QuestionAndAnswerRepository questionAndAnswerRepository;
        private string path;

        public CSVParser(string path, QuestionAndAnswerRepository questionAndAnswerRepository)
        {
            this.questionAndAnswerRepository = questionAndAnswerRepository;
            this.path = path;
        }

        public void Parse()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("windows-1251");
            using var parser = new TextFieldParser(path,
                Encoding.GetEncoding("windows-1251"))
            {
                TextFieldType = FieldType.Delimited
            };
            parser.SetDelimiters(";");
            while (!parser.EndOfData)
            {
                //Process row
                string[] fields = parser.ReadFields();
                for (int i = 0; i < fields.Length; i += 2)
                {
                    var questions = fields[i].Split('/');
                    var answer = fields[i + 1];
                    foreach (var question in questions)
                    {
                        questionAndAnswerRepository.InsertQandA(new QuestionAndAnswer{Answer = answer, Question = question});
                    }
                }
            }
        }
    }
}