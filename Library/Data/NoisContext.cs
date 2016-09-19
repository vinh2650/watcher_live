using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.IO;
using Common.Helpers;
using Core.Domain.Authentication;
using Data.Mapping.Authentication;
using Data.Mapping.Business;

namespace Data
{
    /// <summary>
    /// EF context for snapp
    /// </summary>
    public class NoisContext : DbContext
    {
        public NoisContext()
            : base("AmsContext")
        {
            
        }
        static NoisContext()
        {

            Database.SetInitializer<NoisContext>(new NoisContextInitializer());

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new AmsApplicationMapping());
            modelBuilder.Configurations.Add(new RefreshTokenMapping());
            modelBuilder.Configurations.Add(new RoleMapping());
            modelBuilder.Configurations.Add(new UserClaimMapping());
            modelBuilder.Configurations.Add(new UserMapping());
            modelBuilder.Configurations.Add(new UserRoleMapping());
            //business
            modelBuilder.Configurations.Add(new RelationshipMapping());
            modelBuilder.Configurations.Add(new RelationshipRequestMapping());
            modelBuilder.Configurations.Add(new UserHistoryPathMapping());
        }
    }

    /// <summary>
    /// Context initializer
    /// </summary>
    public class NoisContextInitializer : CreateDatabaseIfNotExists<NoisContext>
    {
        protected override void Seed(NoisContext context)
        {
            var secret = Guid.NewGuid().ToString();
            var applications = new List<AmsApplication>
            {

              new AmsApplication
              {
                  AppSecret =CommonSecurityHelper.GetHash(secret),
                  EncryptSecret = CommonSecurityHelper.Encrypt(secret, CommonSecurityHelper.KeyEncrypt),
                  Active = true,
                  AllowOrigin = "*",
                  Name = "WEB",
                  Description = "WEB app",
                  RefreshTokenLifeTime = 365*24*60,
                  Type = ApplicationType.Javascript,
              }
            };

            var appDbSets = context.Set<AmsApplication>();
            appDbSets.AddRange(applications);
            context.SaveChanges();
            
            //prepair roles
            //init "Insightus Administrator" role
            var insightusRole = new Role()
            {
                Name = RoleSystemName.InsightusAdministrator,
                NormalizeName = RoleSystemName.InsightusAdministrator
            };

            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = RoleSystemName.Administrator,
                    NormalizeName = RoleSystemName.Administrator
                },
                new Role()
                {
                    Name = RoleSystemName.User,
                    NormalizeName = RoleSystemName.User
                }
            };

            roles.Add(insightusRole);

            //Create Role
            var roleDbSet = context.Set<Role>();
            roleDbSet.AddRange(roles);
            context.SaveChanges();

            //prepair Insightus Admin
            string saltKey = CommonSecurityHelper.CreateSaltKey(5);
            var user = new User()
            {
                Email = "administrator@insightus.com.au",
                UserName = "administrator@insightus.com.au",
                FirstName = "admin",
                LastName = "ams",
                DigitCodeHash = CommonSecurityHelper.CreatePasswordHash("Insightus@2016", saltKey),
                SaltDigitCodeHash = saltKey,
                Active = true,
                IsEmailConfirmed = true
            };

            //create Insightus Admin
            var userDbSet = context.Set<User>();
            userDbSet.Add(user);
            context.SaveChanges();

            //prepair "Insightus Administrator" to user role 
            var userRole = new UserRole()
            {
                RoleId = insightusRole.Id,
                UserId = user.Id
            };

            //create "Insightus Administrator" to user role 
            var userRoleDbSet = context.Set<UserRole>();
            userRoleDbSet.Add(userRole);
            context.SaveChanges();
        }

        private void RunAllStoreProcedure(NoisContext context)
        {
            var path = System.AppDomain.CurrentDomain.BaseDirectory + @"App_Data\StoreProcedure\";

            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var sp = File.ReadAllText(file);
                context.Database.ExecuteSqlCommand(sp);
            }
        }
    }
}
