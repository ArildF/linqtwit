using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using LinqTwit.TestUtilities;
using NUnit.Framework;

namespace LinqTwit.Infrastructure.Tests
{
    [TestFixture]
    public class ClipboardServiceTest : TestBase
    {
        private ClipboardService _service;

        private IDataObject _currentObject;

        protected override void OnSetup()
        {
            _currentObject = Clipboard.GetDataObject();

            _service = new ClipboardService();
        }

        [TearDown]
        public void TearDown()
        {
            Clipboard.SetDataObject(_currentObject);
            
        }

        [Test]
        public void SetUrl()
        {
            _service.SetUrl("http://ikke.no");

            var clipboardContents = Clipboard.GetText();

            Assert.That(clipboardContents, Is.EqualTo("http://ikke.no"));

        }

    }
}
