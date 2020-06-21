using EmailAddressTypeEnum = ContactManager.Business.Enums.EmailAddressTypeEnum;

namespace ContactManager.Business.Models
{
    public class ContactDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int EmailId { get; set; }
        public string EmailAddress { get; set; }
        public EmailAddressTypeEnum EmailAddressType { get; set; }
    }
}