using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using CompositeWPFContrib.Composite.StructureMapExtensions;
using Microsoft.Practices.Composite.Modularity;

namespace LinqTwit.Shell
{
    class BootStrapper : StructureMapBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            var presenter = Container.GetInstance<IShellPresenter>();

            IShellView view = presenter.View;
            view.ShowView();

            return (DependencyObject) view;
        }

        protected override Microsoft.Practices.Composite.Modularity.ModuleCatalog GetModuleCatalog()
        {
            return new ModuleCatalog().AddModule(typeof(QueryModule.QueryModule));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.Configure(x =>
                                    {
                                        x.ForRequestedType<IShellView>().
                                            TheDefaultIsConcreteType
                                            <Shell.ShellView>();

                                        x.ForRequestedType<IShellPresenter>().
                                            TheDefaultIsConcreteType
                                            <ShellPresenter>();
                                    });

            Container.AssertConfigurationIsValid();
        }
    }
    
}
