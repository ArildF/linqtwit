using System;
using System.Collections.Generic;
using System.Windows.Input;
using LinqTwit.Infrastructure;
using LinqTwit.Twitter;
using Microsoft.Practices.Composite.Presentation.Commands;
using LinqTwit.Utilities;

namespace LinqTwit.QueryModule.ViewModels
{
    public class OAuthLoginViewModel : ViewModelBase
    {
        private readonly Func<IOauthSession> _sessionCreator;
        private readonly IAsyncManager _asyncManager;
        private readonly IProcessLauncher _processLauncher;
        private readonly DelegateCommand<object> _getAuthorizationUrlCommand;
        private IOauthSession _session;

        public OAuthLoginViewModel(Func<IOauthSession> sessionCreator, 
            IAsyncManager asyncManager, IProcessLauncher processLauncher)
        {
            _sessionCreator = sessionCreator;
            _asyncManager = asyncManager;
            _processLauncher = processLauncher;
            _getAuthorizationUrlCommand = new DelegateCommand<object>(ExecuteGetAuthorizationUrl);
        }

        private void ExecuteGetAuthorizationUrl(object obj)
        {
            _asyncManager.RunAsync(DoGetAuthorizationUrl());
        }


        private IEnumerable<Action> DoGetAuthorizationUrl()
        {
            _session = _sessionCreator();
            string url = null;
            yield return () => url = _session.GetAuthorizationUrl();

            if (url != null)
            {
                yield return () => _processLauncher.LaunchUrl(url);

                ShowPin = true;
            }
        }

        public ICommand GetAuthorizationUrlCommand
        {
            get {
                return _getAuthorizationUrlCommand;
            }
        }

        public bool ShowPin
        {
            get { return _showPin; }
            private set
            {
                if (_showPin != value)
                {
                    _showPin = value;
                    this.OnPropertyChanged(vm => vm.ShowPin);
                }
            }
        }

        private bool _showPin;
    }
}
