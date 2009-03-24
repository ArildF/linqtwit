using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace LinqTwit.Twitter
{
    [ServiceContract]
    public interface ITwitterRestServiceContract
    {
        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/statuses/show/{id}.xml")]
        Status GetStatus(string id);

    }
}
