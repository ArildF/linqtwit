using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqTwit.QueryModule.ViewModels
{
    public class QueryEntryViewModel : IQueryEntryViewModel
    {
        public QueryEntryViewModel(IQueryEntryView view)
        {
            View = view;
        }

        public IQueryEntryView View
        {
            get; private set;
        }

        public string QueryText { get; set; }
    }
}
