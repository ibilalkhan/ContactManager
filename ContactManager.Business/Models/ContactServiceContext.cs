using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;
using System.Reflection;

namespace ContactManager.Business.Models
{
    public class ContactserviceContext : DbContext
    {
        public ContactserviceContext() : base("name=ContactserviceContext")
        {
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public System.Data.Entity.DbSet<ContactManager.Business.Models.Contact> Contacts { get; set; }

        internal void RunMigration(DbContext context, DbMigration migration)
        {
            var prop = migration.GetType().GetProperty("Operations", BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop != null)
            {
                var operations = prop.GetValue(migration) as IEnumerable<MigrationOperation>;
                var generator = new SqlServerMigrationSqlGenerator();
                var statements = generator.Generate(operations, "2008");
                foreach (var item in statements)
                    context.Database.ExecuteSqlCommand(item.Sql);
            }
        }
    }
}