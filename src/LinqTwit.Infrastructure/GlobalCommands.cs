using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Composite.Presentation.Commands;

namespace LinqTwit.Infrastructure
{
    public static class GlobalCommands
    {
        private static CompositeCommand upCommand = new CompositeCommand();

        public static CompositeCommand UpCommand
        {
            get { return upCommand; }
            set { upCommand = value; }
        }

        private static CompositeCommand downCommand = new CompositeCommand();

        public static CompositeCommand DownCommand
        {
            get { return downCommand; }
            set { downCommand = value; }
        }
    }
}
