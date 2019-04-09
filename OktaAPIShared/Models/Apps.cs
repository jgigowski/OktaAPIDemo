using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OktaAPIShared.Models
{
    [DataContract]
    public class App
    {
        [DataMember(Name = "label")]
        public string label { get; set; }

        [DataMember(Name = "_links")]
        public _links _links { get; set; }
    }

    [DataContract]
    public class _links
    {
        [DataMember(Name = "appLinks")]
        public List<appLinks> appLinks { get; set; }

        [DataMember(Name = "logo")]
        public List<logo> logo { get; set; }
    }

    [DataContract]
    public class appLinks
    {
        [DataMember(Name = "href")]
        public string href { get; set; }
    }

    [DataContract]
    public class logo
    {
        [DataMember(Name = "href")]
        public string href { get; set; }
    }
    
    [DataContract]
    public class AppResponse
    {
        public List<App> App { get; set; }
    }
}