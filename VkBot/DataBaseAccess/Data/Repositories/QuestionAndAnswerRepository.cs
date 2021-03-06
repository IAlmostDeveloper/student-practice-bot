﻿using System;
using System.Collections.Generic;
using System.Linq;
using VkBot.Data.Entities;

namespace DataBaseAccess.Data.Repositories
{
    public class QuestionAndAnswerRepository
    {
        private readonly DataBaseContext dbContext;

        public QuestionAndAnswerRepository(DataBaseContext dbContext)
        {
            this.dbContext = dbContext;
            dbContext.SaveChanges();
        }

        public QuestionAndAnswer InsertQandA(QuestionAndAnswer qanda)
        {
            var userEntity = dbContext.QuestionsAndAnswers.Add(qanda);
            dbContext.SaveChanges();
            return userEntity.Entity;
        }

        public QuestionAndAnswer FindById(int qandaId)
        {
            return dbContext.QuestionsAndAnswers.FirstOrDefault(s => s.Id == qandaId);
        }

        public QuestionAndAnswer FindByQuestion(string question)
        {
            QuestionAndAnswer result = null;
            try
            {
                result = dbContext.QuestionsAndAnswers.First(s => s.Question.Equals(question));
            }
            catch (InvalidOperationException)
            {
            }

            return result;
        }

        public QuestionAndAnswer FindByAnswer(string answer)
        {
            QuestionAndAnswer result = null;
            try
            {
                result = dbContext.QuestionsAndAnswers.First(s => s.Answer.Equals(answer));
            }
            catch (InvalidOperationException)
            {
            }

            return result;
        }

        public IEnumerable<QuestionAndAnswer> FindSeveralByAnswers(IEnumerable<string> answers)
        {
            return answers.Select(FindByAnswer);
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
            dbContext.QuestionsAndAnswers.Remove(student);
            dbContext.SaveChanges();
            return true;
        }
    }
}