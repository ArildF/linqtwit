using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LinqTwit.Infrastructure
{
    public class ClipboardService : IClipboardService
    {
        public void SetUrl(string url)
        {
            Clipboard.SetText(url);
        }
    }
}
