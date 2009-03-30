using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqTwit.QueryModule.ViewModels
{
    public class QueryResultsViewModel : IQueryResultsViewModel
    {
        public QueryResultsViewModel(IQueryResultsView view)
        {
            View = view;
        }

        public IQueryResultsView View
        {
            get;
            private set;
        }
    }
}
