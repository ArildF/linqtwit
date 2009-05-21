using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LinqTwit.Twitter
{
    [CollectionDataContract(Namespace = "", Name = "statuses")]
    public class Statuses : List<Status>
    {
    }
}