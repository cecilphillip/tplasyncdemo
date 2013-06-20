using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Core;
using Autofac.Integration.WebApi;
using WebSearch.API.API;
using WebSearch.API.WebSearch;

namespace WebSearch.API
{
    public static class WebApiConfig
    {
        //private const string accountKey = "lHn1H6sN95avNx0zuS+ckOMKfptz3eQw9EM/UpyCnWI";
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{searchTerm}"
            );


            config.EnableSystemDiagnosticsTracing();
            RegisterDepedencies(config);
        }

        private static void RegisterDepedencies(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            //builder.Register(c => new BingSearchClient(accountKey))
            //       .As<ISearchEngine>().Named<ISearchEngine>("BING")
            //       .InstancePerApiRequest();
            builder.RegisterType<BingSearchClient>().As<ISearchEngine>()
                   .Named<ISearchEngine>("BING")
                   .InstancePerApiRequest();

            builder.RegisterType<GoogleSearchClient>().As<ISearchEngine>()
                .Named<ISearchEngine>("GOOGLE")
                .InstancePerApiRequest();

            builder.RegisterType<BingSearchController>()
                .WithParameter(ResolvedParameter.ForNamed<ISearchEngine>("BING"))
                .InstancePerApiRequest();

            builder.RegisterType<GoogleSearchController>()
              .WithParameter(ResolvedParameter.ForNamed<ISearchEngine>("GOOGLE"))
              .InstancePerApiRequest();

            builder.RegisterType<CompositeSearchClient>()
                .Named<ISearchEngine>("COMPOSITE")
                .InstancePerApiRequest();

            builder.RegisterType<CompositeSearchController>()
               .WithParameter(ResolvedParameter.ForNamed<ISearchEngine>("COMPOSITE"))
               .InstancePerApiRequest();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
