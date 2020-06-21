using System.ComponentModel.DataAnnotations;
using EmailAddressTypeEnum = ContactManager.Business.Enums.EmailAddressTypeEnum;

namespace ContactManager.Business.Models
{
    public class EmailAddress
    {
        public int Id { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailId { get; set; }

        public EmailAddressTypeEnum EmailType { get; set; }
    }
}