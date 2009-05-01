using LinqTwit.QueryModule.Controllers;
using LinqTwit.QueryModule.ViewModels;
using LinqTwit.QueryModule.Views;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Regions;
using StructureMap;

namespace LinqTwit.QueryModule
{
    public class QueryModule : IModule
    {
        private readonly IContainer container;
        private readonly IRegionManager regionManager;


        public QueryModule(IContainer locator, IRegionManager regionManager)
        {
            this.container = locator;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.RegisterViewsAndServices();

            var entry = this.container.GetInstance<IQueryEntryViewModel>();
            var results = this.container.GetInstance<IQueryResultsViewModel>();

            var ignored = this.container.GetInstance<ILoginController>();

            this.regionManager.Regions["QueryResults"].Add(results.View);
            this.regionManager.Regions["QueryEntry"].Add(entry.View);
        }

        private void RegisterViewsAndServices()
        {
            this.container.Configure(c =>
                                         {
                                             c.ForRequestedType
                                                 <IQueryEntryViewModel>().
                                                 TheDefaultIsConcreteType
                                                 <QueryEntryViewModel>();

                                             c.ForRequestedType
                                                 <IQueryResultsViewModel>().
                                                 TheDefaultIsConcreteType
                                                 <QueryResultsViewModel>();
                                             c.ForRequestedType<IQueryEntryView>
                                                 ().TheDefaultIsConcreteType
                                                 <QueryEntryView>();
                                             c.ForRequestedType
                                                 <IQueryResultsView>().
                                                 TheDefaultIsConcreteType
                                                 <QueryResultsView>();
                                             c.ForRequestedType
                                                 <ILoginController>().
                                                 TheDefaultIsConcreteType
                                                 <LoginController>();
                                             c.ForRequestedType<ILoginView>().
                                                 TheDefaultIsConcreteType
                                                 <LoginView>();


                                         });
        }
    }
}
