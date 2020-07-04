using System;
using System.Collections.Generic;
using System.Linq;
using VkBot.Data.Entities;

namespace DataBaseAccess.Data.Repositories
{
    public class WordAndAnswerRepository
    {
        private readonly DataBaseContext dbContext;

        public WordAndAnswerRepository(DataBaseContext dbContext)
        {
            this.dbContext = dbContext;
            dbContext.SaveChanges();
        }

        public WordAndAnswer InsertWandA(WordAndAnswer wanda)
        {
            var userEntity = dbContext.WordsAndAnswers.Add(wanda);
            dbContext.SaveChanges();
            return userEntity.Entity;
        }

        public WordAndAnswer FindById(int qandaId)
        {
            return dbContext.WordsAndAnswers.FirstOrDefault(s => s.Id == qandaId);
        }

        public IEnumerable<WordAndAnswer> FindAnswersByWord(string word)
        {
            List<WordAndAnswer> result = null;
            try
            {
                result = dbContext.WordsAndAnswers.Where(w => w.Word.Equals(word)).ToList();
            }
            catch (InvalidOperationException)
            {
            }

            return result;
        }

        public IEnumerable<string> FindSeveralByPhrase(string phrase)
        {
            var maxFrequency = 0;
            var answerAndRepeatCount = new Dictionary<string, int>();
            var words = phrase.Split(new[] {' ', '?', '.', ',', '!'},
                StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                var answers = FindAnswersByWord(word);
                foreach (var answer in answers)
                {
                    if (answerAndRepeatCount.ContainsKey(answer.Answer))
                        answerAndRepeatCount[answer.Answer]++;
                    else answerAndRepeatCount.Add(answer.Answer, 0);
                }

                maxFrequency = answerAndRepeatCount.Select(pair => pair.Value).Concat(new[] {0}).Max();
            }

            return answerAndRepeatCount
                .Where(a => maxFrequency - a.Value <= 2)
                .OrderBy(answer => answer.Value)
                .Select(answer => answer.Key)
                .Take(3);
        }


        public void Update(QuestionAndAnswer updatedQandA)
        {
            dbContext.QuestionsAndAnswers.Update(updatedQandA);
            dbContext.SaveChanges();
        }

        public bool Delete(int qandaId)
        {
            var student = FindById(qandaId);
            if (student == null)
                return false;
            dbContext.WordsAndAnswers.Remove(student);
            dbContext.SaveChanges();
            return true;
        }
    }
}