using ContactManager.Business.Models;
using System.Collections.Generic; 
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results; 

namespace Contactservice.Controllers
{
    public class ContactController : ApiController
    {
        private readonly ContactserviceContext _db = new ContactserviceContext();

        [HttpGet]
        public IQueryable<ContactDto> GetContacts()
        {
            var contacts = from c in _db.Contacts
                           from e in c.EmailAddresses
                           select new ContactDto()
                           {
                               Id = c.Id,
                               FirstName = c.FirstName,
                               LastName = c.LastName,
                               EmailId = e.Id,
                               EmailAddress = e.EmailId,
                               EmailAddressType = e.EmailType
                           };

            return contacts;
        } 

        [HttpPost]
        public JsonResult<object> PostContact([FromBody] ContactDto newContact)
        {
            using (_db)
            {
                var contact = new Contact()
                {
                    FirstName = newContact.FirstName,
                    LastName = newContact.LastName,
                    EmailAddresses = new List<EmailAddress>()
                    {
                        new EmailAddress()
                        {
                            EmailId = newContact.EmailAddress,
                            EmailType = newContact.EmailAddressType
                        }
                    }
                };

                Validate(contact);

                if (!ModelState.IsValid)
                {
                    return Json((object)new
                    {
                        success = false,
                        errors = ModelState.Values.Where(i => i.Errors.Count > 0)
                        .SelectMany(x => x.Errors.Select(u => u.ErrorMessage)).ToList()
                    });
                }

                _db.Contacts.Add(contact);
                _db.SaveChanges();
            }

            return Json((object)new { success = true });
        }

        [HttpPut]
        public JsonResult<object> PutContact([FromBody] ContactDto contact)
        {
            var existingContact = _db.Contacts.Find(contact.Id);

            if (existingContact != null)
            {
                existingContact.FirstName = contact.FirstName;
                existingContact.LastName = contact.LastName;
                existingContact.EmailAddresses.FirstOrDefault(x => x.Id == contact.EmailId).EmailId =
                    contact.EmailAddress;
                existingContact.EmailAddresses.FirstOrDefault(x => x.Id == contact.EmailId).EmailType =
                    contact.EmailAddressType;

                Validate(existingContact);
            }

            if (!ModelState.IsValid)
            {
                return Json((object)new
                {
                    success = false,
                    errors = ModelState.Values.Where(i => i.Errors.Count > 0)
                        .SelectMany(x => x.Errors.Select(u => u.ErrorMessage)).ToList()
                });
            }


            _db.SaveChanges();
            return Json((object)new { success = true });
        }

        [HttpPost]
        public JsonResult<object> PostContacts([FromBody] IEnumerable<ContactDto> newContacts)
        {
            var contacts = newContacts.Select(x => new Contact
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                EmailAddresses = new List<EmailAddress>
                {
                    new EmailAddress
                    {
                        EmailId =  x.EmailAddress,
                        EmailType =  x.EmailAddressType
                    }
                }
            });

            Validate(contacts);

            if (!ModelState.IsValid)
            {
                return Json((object)new
                {
                    success = false,
                    errors = ModelState.Values.Where(i => i.Errors.Count > 0)
                        .SelectMany(x => x.Errors.Select(u => u.ErrorMessage)).ToList()
                });
            }

            _db.Contacts.AddRange(contacts);
            _db.SaveChanges();
            return Json((object)new { success = true });
        }
    }
}