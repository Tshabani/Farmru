using System.ComponentModel.DataAnnotations;

namespace Farmru.IotMonitoring.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}