using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LinqTwit.Infrastructure;
using LinqTwit.Utilities;
using StructureMap;

namespace LinqTwit.QueryModule.ViewModels
{
    public class QueryResultsContextMenu : ContextMenuRoot
    {
        public QueryResultsContextMenu(IContainer container)
        {
            new[]{MenuKeyNames.Refresh, MenuKeyNames.CopyTweeturl, MenuKeyNames.Exit}
                .Select(name => CreateItem(container, name))
                .Where(item => item != null)
                .ForEach(Add);

        }

        private static MenuViewModel CreateItem(IContainer container, string name)
        { 
            var command = container.GetInstance<ICommand>(name);
            if (command != null)
            {
                return new MenuViewModel(name, command);
            }

            return null;
        }
    }
}
