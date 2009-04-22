using System;
using System.ComponentModel;
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
        private readonly DelegateCommand<object> provideCredentialsCommand;
        private readonly IRegion region;
        private bool authorizationState;


        public LoginController(IEventAggregator eventAggregator, 
            IRegionManager regionManager, ILoginView view, ILinqApi api, ICredentialsStore store)
        {
            eventAggregator.GetEvent<InitialViewActivatedEvent>().Subscribe(
                DoLogin);

            this.region = regionManager.Regions[RegionNames.DialogRegion];

            this.eventAggregator = eventAggregator;
            this.view = view;
            this.api = api;
            this.store = store;
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
            api.SetCredentials(this.Username, this.Password);


            try
            {
                var timeline = api.FriendsTimeLine();
            }
            catch (TwitterAuthorizationException)
            {
                return;
            }

            this.region.Deactivate(this.view);
            this.AuthorizationState = true;

            this.PersistCredentials();
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
