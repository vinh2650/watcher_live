using System.Data.Entity;
using API.Infrastructure;
using API.Providers;
using Autofac;
using Autofac.Integration.WebApi;
using Common.Logs;
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
            #region logging
            
            builder.RegisterType<Log4NetService>()
             .As<INoisLoggingService>().InstancePerDependency();
            #endregion
            builder.RegisterType<AmsApplicationService>()
               .As<IAmsApplicationService>().InstancePerDependency();

            builder.RegisterType<RefreshTokenService>()
                .As<IRefreshTokenService>().InstancePerDependency();
            builder.RegisterType<RoleService>()
               .As<IRoleService>().InstancePerDependency();
            builder.RegisterType<UserService>()
                .As<IUserService>().InstancePerDependency();

            //work context
            builder.RegisterType<WorkContext>().As<IWorkContext>().InstancePerLifetimeScope();

            //user store
            builder.RegisterType<EfUserStore>().As<IUserStore<User>>().InstancePerDependency();
            builder.RegisterType<EfUserManager>().As<UserManager<User>>().InstancePerDependency();
            var dataProtectionProvider = new MachineKeyProtectionProvider();
            builder.Register<IDataProtectionProvider>(cc => dataProtectionProvider).InstancePerDependency();

            //business entity
            builder.RegisterType<NoisSearchEngine>()
              .As<INoisSearchEngine>().InstancePerDependency();
            builder.RegisterType<RelationshipService>()
              .As<IRelationshipService>().InstancePerDependency();
            builder.RegisterType<RelationshipRequestService>()
              .As<IRelationshipRequestService>().InstancePerDependency();

            #region ES
            //CSV
            //Elasticsearch
            builder.RegisterType<NoisSearchEngine>()
                .As<INoisSearchEngine>().InstancePerDependency();

            #endregion

            return builder;
        }
    }
}