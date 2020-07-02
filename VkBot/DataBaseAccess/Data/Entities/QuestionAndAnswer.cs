using System.ComponentModel.DataAnnotations;

namespace VkBot.Data.Entities
{
    public class QuestionAndAnswer
    {
        [Key] public int id { get; set; }
        [Required] public string Question { get; set; }
        [Required] public string Answer { get; set; }
    }
}