using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Logging;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Presentation.Regions;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using StructureMap;
using StructureMap.ServiceLocatorAdapter;

namespace CompositeWPFContrib.Composite.StructureMapExtensions
{
    /// <summary>
    /// Base class that provides a basic bootstrapper sequene that
    /// registers most of the Composite Application Library assets
    /// in a <see cref="StructureMap.IContainer"/>.
    /// </summary>
    /// <remarks>
    /// This class must be overriden to provide application specific configuration.
    /// </remarks>
    public abstract class StructureMapBootstrapper
    {
        private readonly ILoggerFacade _loggerFacade = new TraceLogger();

        /// <summary>
        /// Gets the default <see cref="ILoggerFacade"/> for the application.
        /// </summary>
        /// <value>A <see cref="ILoggerFacade"/> instance.</value>
        public ILoggerFacade LoggerFacade
        {
            get { return _loggerFacade; }
        }

        /// <summary>
        /// Gets the default <see cref="IContainer"/> for the application.
        /// </summary>
        /// <value>The default <see cref="IContainer"/> instance.</value>
        public IContainer Container { get; private set; }

        /// <summary>
        /// Runs the bootstrapper process.
        /// </summary>
        public void Run()
        {
            var logger = LoggerFacade;

            logger.Log("Creating StructureMap container", Category.Debug, Priority.Low);
            Container = CreateContainer();

            logger.Log("Configuring container", Category.Debug, Priority.Low);
            ConfigureContainer();

            logger.Log("Configuring region adapters", Category.Debug, Priority.Low);
            ConfigureRegionAdapterMappings();

            logger.Log("Creating shell", Category.Debug, Priority.Low);
            var shell = CreateShell();
            if (shell != null)
            {
                RegionManager.SetRegionManager(shell, Container.GetInstance<IRegionManager>());
            }

            logger.Log("Initializing modules", Category.Debug, Priority.Low);
            InitializeModules();

            logger.Log("Bootstrapper sequence completed", Category.Debug, Priority.Low);
        }

        /// <summary>
        /// Configures the <see cref="IContainer"/>. May be overwritten in a derived class to add specific
        /// type mappings required by the application.
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            Container.Configure(reg =>
                                    {
                                        reg.ForRequestedType<ILoggerFacade>().TheDefault.IsThis(LoggerFacade);
                                        reg.ForRequestedType<IContainer>().TheDefault.IsThis(Container);

                                        var catalog = GetModuleCatalog();
                                        if (catalog != null)
                                            reg.ForRequestedType<IModuleCatalog>().TheDefault.IsThis(catalog);
                                        RegisterTypeIfMissing
                                            <IModuleManager, ModuleManager>(true);

                                        RegisterTypeIfMissing<IServiceLocator, StructureMapServiceLocator>(true);
                                        RegisterTypeIfMissing<IEventAggregator, EventAggregator>(true);
                                        RegisterTypeIfMissing<RegionAdapterMappings, RegionAdapterMappings>(true);
                                        RegisterTypeIfMissing<IRegionManager, RegionManager>(true);
                                        RegisterTypeIfMissing<IModuleInitializer, ModuleInitializer>(true);
                                    });

            ServiceLocator.SetLocatorProvider(() => this.Container.GetInstance<IServiceLocator>());
        }

        /// <summary>
        /// Configures the default region adapter mappings to use in the application, in order
        /// to adapt UI controls defined in XAML to use a region and register it automatically.
        /// May be overwritten in a derived class to add specific mappings required by the application.
        /// </summary>
        /// <returns>The <see cref="RegionAdapterMappings"/> instance containing all the mappings.</returns>
        protected virtual RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var regionAdapterMappings = Container.GetInstance<RegionAdapterMappings>();
            if (regionAdapterMappings != null)
            {
                regionAdapterMappings.RegisterMapping(typeof (Selector), Container.GetInstance<SelectorRegionAdapter>());
                regionAdapterMappings.RegisterMapping(typeof(ItemsControl), Container.GetInstance <ItemsControlRegionAdapter>());
                regionAdapterMappings.RegisterMapping(typeof(ContentControl), Container.GetInstance<ContentControlRegionAdapter>());
            }

            return regionAdapterMappings;
        }

        /// <summary>
        /// Initializes the modules. May be overwritten in a derived class to use custom
        /// module loading and avoid using an <seealso cref="IModuleLoader"/> and
        /// <seealso cref="IModuleEnumerator"/>.
        /// </summary>
        protected virtual void InitializeModules()
        {
            IModuleManager manager;
            try
            {
                manager = this.Container.GetInstance<IModuleManager>();
            }
            catch (ResolutionFailedException ex)
            {
                if (ex.Message.Contains("IModuleCatalog"))
                {
                    throw new InvalidOperationException("No module catalog");
                }

                throw;
            }

            manager.Run();
        }

        /// <summary>
        /// Creates the <see cref="IContainer"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="IContainer"/>.</returns>
        protected virtual IContainer CreateContainer()
        {
            return new Container();
        }

        /// <summary>
        /// Returns the module enumerator that will be used to initialize the modules.
        /// </summary>
        /// <remarks>
        /// When using the default initialization behavior, this method must be overwritten by a derived class.
        /// </remarks>
        /// <returns>An instance of <see cref="IModuleEnumerator"/> that will be used to initialize the modules.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual ModuleCatalog GetModuleCatalog()
        {
            return null;
        }

        protected void RegisterTypeIfMissing<FROMTYPE, TOTYPE>(bool registerAsSingleton) 
            where FROMTYPE : class 
            where TOTYPE : class, FROMTYPE
        {
            var logger = LoggerFacade;

            if (Container.TryGetInstance<FROMTYPE>() != null)
            {
                logger.Log(string.Format("{0} is already registered", typeof(FROMTYPE).Name), 
                           Category.Debug, Priority.Low);
            }
            else
            {
                Container.Configure(x =>
                                        {
                                            var exp = x.BuildInstancesOf<FROMTYPE>().TheDefaultIsConcreteType<TOTYPE>();
                                            if (registerAsSingleton)
                                                exp.AsSingletons();
                                        });
            }
        }

        /// <summary>
        /// Creates the shell or main window of the application.
        /// </summary>
        /// <returns>The shell of the application.</returns>
        /// <remarks>
        /// If the returned instance is a <see cref="DependencyObject"/>, the
        /// <see cref="StructureMapBootstrapper"/> will attach the default <seealso cref="IRegionManager"/> of
        /// the application in its <see cref="RegionManager.RegionManagerProperty"/> attached property
        /// in order to be able to add regions by using the <seealso cref="RegionManager.RegionNameProperty"/>
        /// attached property from XAML.
        /// </remarks>
        protected abstract DependencyObject CreateShell();
    }
}