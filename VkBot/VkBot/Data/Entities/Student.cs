using System;
using System.ComponentModel.DataAnnotations;

namespace VkBot.Data.Entities
{
    public class Student
    {
        [Key] public int idStudents { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
    }
}