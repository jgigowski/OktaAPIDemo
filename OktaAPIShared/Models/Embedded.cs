using System;
using System.Runtime.Serialization;

namespace OktaAPIShared.Models {
    [DataContract]
    public class Embedded {

        [DataMember(Name = "user")]
        public User User { get; set; }
    }
}