using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Common;
using LinqTwit.Infrastructure;

namespace LinqTwit.Commands
{
    public class CopyTweetUrlCommand : CommandBase
    {
        private readonly string _urlFormat;
        private readonly ISelection _selection;
        private readonly IClipboardService _clipboardService;

        public CopyTweetUrlCommand(string urlFormat, ISelection selection, IClipboardService clipboardService)
        {
            _urlFormat = urlFormat;
            _selection = selection;
            _clipboardService = clipboardService;
        }

        public override bool CanExecute(object parameter)
        {
            return _selection.SelectedTweet != null;
        }

        public override void Execute(object parameter)
        {
            var status = _selection.SelectedTweet;
            var url = _urlFormat.Replace("%user%", status.User.Name);
            url = url.Replace("%id%", status.Id);

            _clipboardService.SetUrl(url);
        }
    }
}
