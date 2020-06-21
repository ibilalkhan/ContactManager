using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using ContactManager.Business.Models; 
using Contact = ContactManager.Business.Models.Contact;
using ContactserviceContext = ContactManager.Business.Models.ContactserviceContext;
using EmailAddressTypeEnum = ContactManager.Business.Enums.EmailAddressTypeEnum;

namespace ContactManager.Business.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<ContactserviceContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            Database.SetInitializer<ContactserviceContext>(null);
        }

        protected override void Seed(ContactserviceContext context)
        {
            Seeder.Seed(context);
        }
    }

    public class Seeder
    {
        public static void Seed(ContactserviceContext context)
        {
            context.Contacts.AddOrUpdate(x => x.Id,
                new Contact()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailAddresses = new List<EmailAddress>()
                    {
                        new EmailAddress
                                    {
                                        Id = 1,
                                        EmailId = "test1@test.com",
                                        EmailType = EmailAddressTypeEnum.Personal
                                    },
                        new EmailAddress
                                    {
                                        Id = 2,
                                        EmailId = "test2@test.com",
                                        EmailType = EmailAddressTypeEnum.Business
                                    }
                    }
                },
                new Contact()
                {
                    Id = 2,
                    FirstName = "Jason",
                    LastName = "Smith",
                    EmailAddresses = new List<EmailAddress>()
                    {
                                                new EmailAddress
                                    {
                                        Id = 3,
                                        EmailId = "test3@test.com",
                                        EmailType = EmailAddressTypeEnum.Personal
                                    },
                        new EmailAddress
                                    {
                                        Id = 4,
                                        EmailId = "test4@test.com",
                                        EmailType = EmailAddressTypeEnum.Business
                                    }
                    }
                });
        }
    }
}