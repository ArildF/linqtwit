using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LinqTwit.Twitter.Wcf
{
    class PoxBehavior : IEndpointBehavior, IOperationBehavior
    {
        public void Validate(ServiceEndpoint endpoint)
        {
            
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            foreach (var operation in endpoint.Contract.Operations)
            {
                operation.Behaviors.Add(this);
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void Validate(OperationDescription operationDescription)
        {
            
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            clientOperation.Formatter = new PoxFormatter(
                operationDescription.Messages[1], clientOperation.Formatter);
        }

        internal class PoxFormatter : IClientMessageFormatter
        {
            private readonly MessageDescription _responseDescription;
            private readonly IClientMessageFormatter _formatter;

            public PoxFormatter(MessageDescription responseDescription, IClientMessageFormatter formatter)
            {
                _responseDescription = responseDescription;
                _formatter = formatter;
            }

            public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
            {
                return _formatter.SerializeRequest(messageVersion, parameters);
            }

            public object DeserializeReply(Message message, object[] parameters)
            {
                XmlDictionaryReader reader = message.GetReaderAtBodyContents();
                var serializer = new XmlSerializer(_responseDescription.Body.ReturnValue.Type);
                return serializer.Deserialize(reader);
            }
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }
    }
}
