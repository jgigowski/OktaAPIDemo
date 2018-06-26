using System;
using System.Runtime.Serialization;

namespace OktaAPIShared.Models {
    [DataContract]
    public class Authentication {

        [DataMember(Name = "username")]
        public String UserName { get; set; }

        [DataMember(Name = "password")]
        public String Password { get; set; }

    }
}