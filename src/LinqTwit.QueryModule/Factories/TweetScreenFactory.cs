using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Core;
using StructureMap;

namespace LinqTwit.QueryModule.Factories
{
    public class TweetScreenFactory : ITweetScreenFactory
    {
        private readonly IContainer _container;

        public TweetScreenFactory(IContainer container)
        {
            _container = container;
        }

        public IQueryResultsViewModel Create(string title, ITimeLineService service)
        {
            return _container.With("caption").EqualTo(title).With(service).GetInstance<IQueryResultsViewModel>();
        }
    }
}
