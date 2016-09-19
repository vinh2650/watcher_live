using System;
using Common.Helpers;
using Core.Domain.Authentication;

namespace Data.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Data.NoisContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Data.NoisContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.E.g.

            //context.People.AddOrUpdate(
            //  p => p.FullName,
            //  new Person { FullName = "Andrew Peters" },
            //  new Person { FullName = "Brice Lambson" },
            //  new Person { FullName = "Rowan Miller" }
            //);

            //var secret = Guid.NewGuid().ToString();
            //var applicationId = Guid.NewGuid().ToString();

            //context.Set<AmsApplication>().AddOrUpdate(
            //  p => p.Id, new AmsApplication
            //  {
            //      Id = applicationId,
            //      AppSecret = CommonSecurityHelper.GetHash(secret),
            //      EncryptSecret = CommonSecurityHelper.Encrypt(secret, CommonSecurityHelper.KeyEncrypt),
            //      Active = true,
            //      AllowOrigin = "*",
            //      Name = "WEB",
            //      Description = "WEB app",
            //      RefreshTokenLifeTime = 365 * 24 * 60,
            //      Type = ApplicationType.Javascript,
            //  }
            //);

            //string saltKey = CommonSecurityHelper.CreateSaltKey(5);
            //context.Set<User>().AddOrUpdate(
            //  p => p.Id, new User()
            //  {
            //      ApplicationId = applicationId,
            //      Email = "quangvinh2650@gmail.com",
            //      UserName = "quangvinh2650@gmail.com",
            //      FirstName = "vinh",
            //      LastName = "tran",
            //      DigitCodeHash = CommonSecurityHelper.CreatePasswordHash("123123", saltKey),
            //      SaltDigitCodeHash = saltKey,
            //      Active = true,
            //      IsEmailConfirmed = true
            //  }
            //);


        }
    }
}
