using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using LinqTwit.Infrastructure;
using LinqTwit.Twitter;
using LinqTwit.Utilities;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Regions;

namespace LinqTwit.QueryModule.Controllers
{
    public class LoginController : ILoginController, INotifyPropertyChanged,
        IRaisePropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string username;

        private string password;
        private readonly IEventAggregator eventAggregator;
        private readonly ILoginView view;
        private readonly ILinqApi api;
        private readonly ICredentialsStore store;
        private readonly IAsyncManager manager;
        private readonly DelegateCommand<object> provideCredentialsCommand;
        private readonly IRegion region;
        private bool authorizationState;


        public LoginController(IEventAggregator eventAggregator, 
            IRegionManager regionManager, ILoginView view, ILinqApi api, ICredentialsStore store, IAsyncManager manager)
        {
            eventAggregator.GetEvent<InitialViewActivatedEvent>().Subscribe(
                DoLogin);

            this.region = regionManager.Regions[RegionNames.DialogRegion];

            this.eventAggregator = eventAggregator;
            this.view = view;
            this.api = api;
            this.store = store;
            this.manager = manager;
            this.region.Add(this.view);

            this.username = store.Username;
            this.password = store.Password;

            this.view.DataContext = this;

            this.provideCredentialsCommand =
                new DelegateCommand<object>(this.ProvideCredentials,
                                            o =>
                                            !(String.IsNullOrEmpty(this.Username) ||
                                              String.IsNullOrEmpty(this.Password)));
        }

        private void ProvideCredentials(object obj)
        {
            manager.RunAsync(DoProvideCredentials());
        }

        private IEnumerable<Action> DoProvideCredentials()
        {

            api.SetCredentials(this.Username, this.Password);

            Status[] timeline = null;
            yield return () =>
                {
                    try
                    {
                        timeline = api.FriendsTimeLine(new FriendsTimeLineArgs());
                    }
                    catch (TwitterAuthorizationException)
                    {
                    }
                };

            if (timeline == null)
            {
                yield break;
            }

            this.region.Deactivate(this.view);
            this.AuthorizationState = true;


            yield return () => this.PersistCredentials();
        }

        private void PersistCredentials()
        {
            this.store.Username = username;
            this.store.Password = password;

            this.store.PersistCredentials();
        }

        private bool AuthorizationState
        {
            set
            {
                if (authorizationState != value)
                {
                    authorizationState = value;
                    this.eventAggregator.GetEvent<AuthorizationStateChangedEvent>().Publish(authorizationState);
                }
                ;
            }
        }

        private void DoLogin(object obj)
        {
            this.region.Activate(this.view);
        }

        public string Username
        {
            get { return this.username; }
            set
            {
                this.username = value;
                this.OnPropertyChanged(c => c.Username);
                this.provideCredentialsCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get { return this.password; }
            set
            {
                this.password = value;
                this.OnPropertyChanged(c => c.Password);
                this.provideCredentialsCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand ProvideCredentialsCommand
        {
            get { return this.provideCredentialsCommand; }
        }

        void IRaisePropertyChanged.RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
