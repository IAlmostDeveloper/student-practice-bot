using System.ComponentModel.DataAnnotations;

namespace VkBot.Data.Entities
{
    public class WordAndAnswer
    {
        public int Id { get; set; }
        [Required] public string Word { get; set; }
        [Required] public string Answer { get; set; }
    }
}