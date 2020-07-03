using System.ComponentModel.DataAnnotations;

namespace VkBot.Data.Entities
{
    public class QuestionAndAnswer
    {
        public int Id { get; set; }
        [Required] public string Question { get; set; }
        [Required] public string Answer { get; set; }
    }
}