using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace LinqTwit.Twitter
{
    [CollectionDataContract(Namespace = "", Name = "statuses")]
    [XmlType("statuses")]
    public class Statuses : List<Status>
    {
    }
}