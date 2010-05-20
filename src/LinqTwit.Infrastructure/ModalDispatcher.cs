using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using System.Windows.Threading;

namespace LinqTwit.Infrastructure
{
    public class ModalDispatcher : IModalDispatcher
    {
        public IModalFrame CreateModalFrame()
        {
            return new ModalFrame(new DispatcherFrame());
        }

        public void Run(IModalFrame frame)
        {
            var modalFrame = (ModalFrame)frame;
            try
            {
                ComponentDispatcher.PushModal();

                Dispatcher.PushFrame(modalFrame.DispatcherFrame);
            }
            finally
            {
                ComponentDispatcher.PopModal();
            }
        }

        private class ModalFrame : IModalFrame
        {
            private readonly DispatcherFrame _dispatcherFrame;

            public ModalFrame(DispatcherFrame dispatcherFrame)
            {
                _dispatcherFrame = dispatcherFrame;
            }

            public DispatcherFrame DispatcherFrame
            {
                get { return _dispatcherFrame; }
            }

            public void Stop()
            {
                DispatcherFrame.Continue = false;
            }
        }
    }

}
