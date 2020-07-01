using System;
using System.Linq;
using VkBot.Data.Entities;

namespace VkBot.Data
{
    public class UserRepository
    {
        private readonly DataBaseContext dbContext;

        public UserRepository(DataBaseContext dbContext)
        {
            this.dbContext = dbContext;
            dbContext.SaveChanges();
        }

        public Student InsertStudent(Student student)
        {
            var userEntity = dbContext.Students.Add(student);
            dbContext.SaveChanges();
            return userEntity.Entity;
        }

        public Student FindById(int studentId)
        {
            return dbContext.Students.FirstOrDefault(s => s.idStudents == studentId);
        }

        public Student FindByFirstName(string firstName)
        {
            Student result = null;
            try
            {
                result = dbContext.Students.First(s => s.FirstName.Equals(firstName));
            }
            catch (InvalidOperationException e)
            {
            }

            return result;
        }


        public void Update(Student updatedStudent)
        {
            dbContext.Students.Update(updatedStudent);
            dbContext.SaveChanges();
        }

        public bool Delete(int userId)
        {
            var student = FindById(userId);
            if (student == null)
                return false;
            dbContext.Students.Remove(student);
            dbContext.SaveChanges();
            return true;
        }
    }
}