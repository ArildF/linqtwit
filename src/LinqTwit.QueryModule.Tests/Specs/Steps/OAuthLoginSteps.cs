using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure;
using LinqTwit.QueryModule.ViewModels;
using LinqTwit.TestUtilities;
using LinqTwit.Twitter;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TestUtilities;

namespace LinqTwit.QueryModule.Tests.Specs.Steps
{
    [Binding]
    class OAuthLoginSteps : TestBase
    {
        private OAuthLoginViewModel _vm;
        private PropertyChangedTester<OAuthLoginViewModel> _tester;
        private const string AuthorizeUrl = "http://twitter.some/Url";

        [BeforeScenario]
        public void SetupContainer()
        {
            base.Setup();
        }

        [Given(@"a new OAuth login dialog")]
        public void GivenANewOAuthLoginDialog()
        {
            Register<Func<IOauthSession>>(() => GetMock<IOauthSession>().Object);
            Register<IAsyncManager>(new SynchronousAsyncManager());

            _vm = Create<OAuthLoginViewModel>();

            _tester = new PropertyChangedTester<OAuthLoginViewModel>(_vm);
        }

        [Given(@"the Oauth service returns an URL")]
        public void GivenTheOauthServiceReturnsAnUrl()
        {
            GetMock<IOauthSession>().Setup(s => s.GetAuthorizationUrl()).Returns(AuthorizeUrl);
        }

        [When(@"I have pressed the Authorize button")]
        public void WhenIHavePressedTheAuthorizeButton()
        {
            _vm.GetAuthorizationUrlCommand.Execute(null);
        }

        [Then(@"the system should retrieve the authorization URL")]
        public void ThenTheSystemShouldRetrieveTheAuthorizationUrl()
        {
            GetMock<IOauthSession>().Verify(s => s.GetAuthorizationUrl());
        }

        [Then(@"launch it in the browser")]
        public void ThenLaunchItInTheBrowser()
        {
            GetMock<IProcessLauncher>().Verify(pl => pl.LaunchUrl(AuthorizeUrl));
        }

        [Then(@"the system should display textbox for inputting pin")]
        public void ThenTheSystemShouldDisplayTextboxForInputtingPin()
        {
            Assert.That(_vm.ShowPin, Is.True);
            Assert.That(_tester.PropertyChanged(vm => vm.ShowPin));

        }
    }
}
