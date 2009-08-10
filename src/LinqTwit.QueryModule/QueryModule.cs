using System;
using System.Windows.Input;
using LinqTwit.Infrastructure;
using LinqTwit.QueryModule.Controllers;
using LinqTwit.QueryModule.Factories;
using LinqTwit.QueryModule.ViewModels;
using LinqTwit.QueryModule.Views;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Regions;
using StructureMap;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Pipeline;

namespace LinqTwit.QueryModule
{
    public class QueryModule : IModule
    {
        private readonly IContainer container;
        private readonly IRegionManager regionManager;
        private ITweetScreenController _controller;


        public QueryModule(IContainer locator, IRegionManager regionManager)
        {
            this.container = locator;
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.RegisterViewsAndServices();

            var entry = this.container.GetInstance<IQueryEntryViewModel>();

            var ignored = this.container.GetInstance<ILoginController>();

            this.regionManager.Regions[RegionNames.QueryEntryRegion].Add(entry.View);

            var dispatcherFacade =
                this.container.GetInstance<IDispatcherFacade>();
            var eventAggregator = this.container.GetInstance<IEventAggregator>();

            dispatcherFacade.CreateRecurringEvent(TimeSpan.FromMinutes(1), 
                () => eventAggregator.GetEvent<RefreshEvent>().Publish(null));

            _controller = container.GetInstance<ITweetScreenController>();
        }

        private void RegisterViewsAndServices()
        {

            this.container.Configure(c =>
                {
                    c.InstanceOf<IQueryResultsViewModel>().Is.OfConcreteType
                        <QueryResultsViewModel>()
                        .CtorDependency<ContextMenuRoot>().Is
                        <QueryResultsContextMenu>();

                    c.Scan(x =>
                        {
                            x.TheCallingAssembly();
                            x.WithDefaultConventions();
                        });

                    c.InstanceOf<ITweetScreenController>().
                        Is.OfConcreteType<TweetScreenController>()
                        .CtorDependency<IRegion>().Is(regionManager.Regions[RegionNames.QueryResultsRegion]);
                });
        }

        private void CreateContextMenu(IInstanceExpression<ContextMenuRoot> expr)
        {
            Func<string, ICommand> gc =
                name => this.container.GetInstance<ICommand>(name);

            expr.IsThis(
                new ContextMenuRoot()
                    {
                        new MenuViewModel("Refresh", gc("Refresh"))
                    });
        }
    }
}
