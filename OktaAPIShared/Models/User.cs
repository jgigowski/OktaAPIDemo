using System;
using System.Runtime.Serialization;

namespace OktaAPIShared.Models {
    [DataContract]
    public class User {

        [DataMember(Name = "id")]
        public String Id { get; set; }
    }
}