using System;
using System.Text;
using DataBaseAccess.Data;
using DataBaseAccess.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using VkBot.Data.Entities;

namespace DataBaseFiller
{
    public class CSVParser
    {
        private QuestionAndAnswerRepository questionAndAnswerRepository;
        private WordAndAnswerRepository wordAndAnswerRepository;
        private string path;

        public CSVParser(string path, QuestionAndAnswerRepository questionAndAnswerRepository,
            WordAndAnswerRepository wordAndAnswerRepository)
        {
            this.questionAndAnswerRepository = questionAndAnswerRepository;
            this.wordAndAnswerRepository = wordAndAnswerRepository;
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
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                //Process row
                var fields = parser.ReadFields();
                for (var i = 0; i < fields.Length; i += 2)
                {
                    var questions = fields[i].Split('/');
                    var answer = fields[i + 1];
                    foreach (var question in questions)
                    {
                        var words = question.Split(new[] {' ', '?', '.', ',', '!'},
                            StringSplitOptions.RemoveEmptyEntries);
                        foreach (var word in words)
                            wordAndAnswerRepository.InsertWandA(new WordAndAnswer {Word = word, Answer = answer});
                        questionAndAnswerRepository.InsertQandA(new QuestionAndAnswer
                            {Answer = answer, Question = question});
                    }
                }
            }

            Console.WriteLine("Database filled");
        }
    }
}