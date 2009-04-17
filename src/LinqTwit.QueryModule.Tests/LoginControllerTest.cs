using System;
using LinqTwit.Infrastructure;
using LinqTwit.QueryModule.Controllers;
using LinqTwit.Twitter;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Regions;
using NUnit.Framework;
using Moq;
using NUnit.Framework.SyntaxHelpers;
using TestUtilities;

namespace LinqTwit.QueryModule.Tests
{
    [TestFixture]
    public class LoginControllerTest
    {
        private LoginController loginController;

        private readonly MockFactory factory =
            new MockFactory(MockBehavior.Loose)
                {DefaultValue = DefaultValue.Mock, CallBase = true};

        private Mock<IRegionManager> regionManager;
        private Mock<IRegion> region;
        private Mock<IEventAggregator> aggregator;
        private Mock<ILoginView> view;
        private Mock<InitialViewActivatedEvent> initialViewEvent;
        private Mock<AuthorizationStateChangedEvent> asChangedEvent;
        private Mock<ILinqApi> api;

        [SetUp]
        public void SetUp()
        {
            regionManager = factory.Create<IRegionManager>();
            aggregator = factory.Create<IEventAggregator>();
            view = factory.Create<ILoginView>();
            region = factory.Create<IRegion>();
            api = factory.Create<ILinqApi>();
            asChangedEvent = factory.Create<AuthorizationStateChangedEvent>();

            initialViewEvent = factory.Create<InitialViewActivatedEvent>();

            regionManager.SetupGet(rm => rm.Regions[RegionNames.DialogRegion]).
                Returns(region.Object);

            aggregator.Setup(a => a.GetEvent<InitialViewActivatedEvent>()).
                Returns(initialViewEvent.Object);


            loginController = new LoginController(aggregator.Object, regionManager.Object, view.Object, api.Object);
        }

        [Test]
        public void ViewIsAddedToRegion()
        {
            region.Verify(r => r.Add(this.view.Object));
        }

        [Test]
        public void PropertyChanged()
        {
            PropertyChangedTester<LoginController> tester =
                new PropertyChangedTester<LoginController>(this.loginController);
            tester.VerifyAllPublicProperties();
        }

        [Test]
        public void ShowWhenInitialViewActivated()
        {
            initialViewEvent.Object.Publish(null);

            region.Verify(r => r.Activate(view.Object));
        }

        [Test]
        public void DisabledIfNoUsernameOrPassword()
        {
            Assert.That(loginController.ProvideCredentialsCommand.CanExecute(null), Is.False);

            loginController.Username = "fo";
            Assert.That(
                loginController.ProvideCredentialsCommand.CanExecute(null),
                Is.False);

            loginController.Password = "bal";
            Assert.That(loginController.ProvideCredentialsCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void SettingUserNameRaisesCanExecuteChanged()
        {
            TestRaisesCanExecuteChanged(() => loginController.Username = "foo");

        }

        [Test]
        public void SettingPasswordRaisesCanExecuteChanged()
        {
            TestRaisesCanExecuteChanged(() => loginController.Password = "foo");

        }

        [Test]
        public void ProvideCredentials()
        {
            loginController.Username = "user";
            loginController.Password = "pass";

            loginController.ProvideCredentialsCommand.Execute(null);

            api.Verify(a => a.SetCredentials("user", "pass"));
        }

        [Test]
        public void ViewDeactivatedWhenCredentialsValid()
        {
            loginController.ProvideCredentialsCommand.Execute(null);

            region.Verify(r => r.Deactivate(view.Object));
        }

        [Test]
        public void ViewNotDeactivatedWhenCredentialsBad()
        {
            api.Setup(a => a.FriendsTimeLine()).Throws(
                new TwitterAuthorizationException());

            loginController.ProvideCredentialsCommand.Execute(null);
            
            region.Verify(r => r.Deactivate(view.Object), Times.Never());
        }

        [Test]
        public void AuthorizationStateChangedEventPublishedOnSuccessfulLogin()
        {
            TestAuthorizationStateChanged(true, () =>
                {});
        }

        private void TestAuthorizationStateChanged(object expectedState, Action action)
        {
            bool published = false;
            this.asChangedEvent.Setup(evt => evt.Publish(true)).Callback((Action<bool>)(state =>
                {
                    published = true;
                    Assert.That(state, Is.EqualTo(expectedState));
                }));
            this.aggregator.Setup(
                a => a.GetEvent<AuthorizationStateChangedEvent>()).Returns(
                this.asChangedEvent.Object);

            action();

            this.loginController.ProvideCredentialsCommand.Execute(null);

            Assert.That(published, Is.True);
        }


        private void TestRaisesCanExecuteChanged(Action action)
        {
            bool raised = false;
            loginController.ProvideCredentialsCommand.CanExecuteChanged +=
                delegate { raised = true; };

            action();

            Assert.That(raised, Is.True);

        }
    }

    
}
