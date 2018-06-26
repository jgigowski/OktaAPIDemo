using System;
using System.Runtime.Serialization;

namespace OktaAPIShared.Models {
    [DataContract]
    public class TokenIntrospectionResponse {

        [DataMember(Name = "active")]
        public bool Active { get; set; }

        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "exp")]
        public string Exp { get; set; }

        [DataMember(Name = "iat")]
        public string IAT { get; set; }

        [DataMember(Name = "sub")]
        public string Sub { get; set; }

        [DataMember(Name = "aud")]
        public string Aud { get; set; }

        [DataMember(Name = "iss")]
        public string ISS { get; set; }

        [DataMember(Name = "jti")]
        public string JTI { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "client_id")]
        public string ClientId { get; set; }

        [DataMember(Name = "uid")]
        public string UID { get; set; }
    }
}
 