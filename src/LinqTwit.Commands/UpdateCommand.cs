using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure;
using LinqTwit.Twitter;

namespace LinqTwit.Commands
{
    public class UpdateCommand : CommandBase<UpdateArgs>
    {
        private readonly ILinqApi _api;
        private readonly IAsyncManager _manager;

        public UpdateCommand(ILinqApi api, IAsyncManager manager)
        {
            _api = api;
            _manager = manager;
        }

        public override void Execute(UpdateArgs status)
        {
            _manager.RunAsync(DoExecute(status));
        }

        private IEnumerable<Action> DoExecute(UpdateArgs status)
        {
            yield return () => _api.Update(status.Status);
        }
    }

    public class UpdateArgs
    {
        public string Status { get; private set; }

        public UpdateArgs(string status)
        {
            Status = status;
        }
    }
}
