using System.Data.Entity;
using API.Providers;
using Autofac;
using Autofac.Integration.WebApi;
using Core.Domain.Authentication;
using Data;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using Service.CachingLayer;
using Service.Implement.Authentication;
using Service.Implement.Business;
using Service.Implement.Search;
using Service.Interface.Authentication;
using Service.Interface.Business;
using Service.Interface.Search;
using Membership;
namespace API.App_Start
{
    /// <summary>
    /// 
    /// </summary>
    public static class AutofacConfig
    {
        public static ContainerBuilder Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            // Register your Web API controllers.

           // builder.RegisterAssemblyTypes(typeof(ApiController).Assembly);
          
            //builder.RegisterControllers(typeof(ApiController).Assembly);

            builder.RegisterApiControllers(typeof(WebApiApplication).Assembly);

            builder.Register(c => new NoisContext())
                .As<DbContext>().InstancePerDependency();
            #region cache

            builder.RegisterType<MemoryCacheManager>()
                .As<ICacheManager>().InstancePerDependency();

            #endregion

            builder.RegisterType<ApplicationService>()
               .As<IAmsApplicationService>().InstancePerDependency();

            builder.RegisterType<RefreshTokenService>()
                .As<IRefreshTokenService>().InstancePerDependency();
            builder.RegisterType<RoleService>()
               .As<IRoleService>().InstancePerDependency();
            builder.RegisterType<UserService>()
                .As<IUserService>().InstancePerDependency();

            //user store
            builder.RegisterType<EfUserStore>().As<IUserStore<User>>().InstancePerDependency();
            builder.RegisterType<EfUserManager>().As<UserManager<User>>().InstancePerDependency();
            var dataProtectionProvider = new MachineKeyProtectionProvider();
            builder.Register<IDataProtectionProvider>(cc => dataProtectionProvider).InstancePerDependency();

            //business entity
            builder.RegisterType<SearchEngine>()
              .As<ISearchEngine>().InstancePerDependency();
            builder.RegisterType<RelationshipService>()
              .As<IRelationshipService>().InstancePerDependency();
            builder.RegisterType<RelationshipRequestService>()
              .As<IRelationshipRequestService>().InstancePerDependency();

            #region ES
            //CSV
            //Elasticsearch
            builder.RegisterType<SearchEngine>()
                .As<ISearchEngine>().InstancePerDependency();
            builder.RegisterType<UserSearchService>()
                .As<IUserSearchService>().InstancePerDependency();
            #endregion

            return builder;
        }
    }
}