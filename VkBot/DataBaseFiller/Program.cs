using System;
using DataBaseAccess.Data;
using DataBaseAccess.Data.Repositories;
using DataBaseFiller;

namespace DBFiller
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=localhost;Port=3306;User=u1090_113default;Password=E51wd#2i;Database=u1090113_default;";

            var db = new MySqlDataBase(connectionString);
            var parser = new CSVParser("csvData.csv",
                new QuestionAndAnswerRepository(db), new WordAndAnswerRepository(db));
            // parser.Parse();
            var qr = new QuestionAndAnswerRepository(db);
            var answers = new WordAndAnswerRepository(db).FindSeveralByPhrase("что курсовая когда зачем баллы");
            foreach (var answer in qr.FindSeveralByAnswers(answers))
            {
                Console.WriteLine(answer.Question);
            }
        }
    }
}