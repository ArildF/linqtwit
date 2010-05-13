using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using DevDefined.OAuth.Consumer;

namespace LinqTwit.Twitter.OAuth
{
    class OAuthBehavior : IEndpointBehavior
    {
        private readonly IOAuthSession _authSession;

        public OAuthBehavior(IOAuthSession authSession)
        {
            _authSession = authSession;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new OauthMessageInspector(_authSession));
        }

        internal class OauthMessageInspector : IClientMessageInspector
        {
            private readonly IOAuthSession _authSession;

            public OauthMessageInspector(IOAuthSession authSession)
            {
                _authSession = authSession;
            }

            public object BeforeSendRequest(ref Message message, IClientChannel channel)
            {
                var httpRequest = (HttpRequestMessageProperty)message.Properties[HttpRequestMessageProperty.Name]; 

                var description =
                    _authSession.Request()
                    .ForMethod(httpRequest.Method)
                    .ForUri(message.Headers.To)
                    .GetRequestDescription();
                
                message.Headers.To = description.Url;

                foreach (var key in description.Headers.AllKeys)
                {
                    httpRequest.Headers.Add(key, description.Headers[key]);
                }

                return null;
            }

            public void AfterReceiveReply(ref Message reply, object correlationState)
            {
            }
        }
    }
}
