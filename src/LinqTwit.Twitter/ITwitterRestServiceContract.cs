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

        //[OperationContract]
        //[WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/statuses/friends_timeline.xml")]
        //Statuses FriendsTimeLine();

        [OperationContract]
        [WebInvoke(
            ResponseFormat = WebMessageFormat.Xml, UriTemplate = "/statuses/update.xml?status={status}")]
        Status Update(string status);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml, 
            UriTemplate = "/statuses/friends_timeline.xml?since_id={sinceId}&count={count}&max_id={maxId}&page={page}")]
        Statuses FriendsTimeLine(string sinceId, string count, string maxId, string page);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/statuses/user_timeline/{user}.xml?since_id={sinceId}&count={count}&max_id={maxId}&page={page}")]
        Statuses UserTimeLine(string user, string sinceId, string count, string maxId, string page);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Xml,
            UriTemplate = "/statuses/mentions.xml?since_id={sinceId}&count={count}&max_id={maxId}&page={page}")]
        Statuses MentionsTimeLine(string sinceId, string count, string maxId, string page);
    }
}
