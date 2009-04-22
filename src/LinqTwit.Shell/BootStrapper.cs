﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using CompositeWPFContrib.Composite.StructureMapExtensions;
using LinqTwit.Infrastructure;
using LinqTwit.Infrastructure.ApplicationServices;
using LinqTwit.Twitter;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Presentation.Commands;

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


        protected override ModuleCatalog GetModuleCatalog()
        {
            return new ModuleCatalog().AddModule(typeof(QueryModule.QueryModule));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            TwitterRestClient client = new TwitterRestClient("twitterEndpoint");

            Container.Configure(x =>
                                    {
                                        x.ForRequestedType<IShellView>().
                                            TheDefaultIsConcreteType
                                            <ShellView>();

                                        x.ForRequestedType<IShellPresenter>().
                                            TheDefaultIsConcreteType
                                            <ShellPresenter>();

                                        x.ForRequestedType<ILinqApi>().
                                            TheDefault.IsThis(client);

                                        x.ForRequestedType<ICredentialsStore>().
                                            TheDefaultIsConcreteType
                                            <CredentialsStore>();

                                    });
        }
    }
    
}
