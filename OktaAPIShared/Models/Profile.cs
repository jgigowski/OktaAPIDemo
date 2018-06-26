using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OktaAPIShared.Models {
    [DataContract]
    public class Profile {

        [Required]
        [Display(Name = "First Name")]
        [DataMember(Name = "firstName")]
        public String FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [DataMember(Name = "lastName")]
        public String LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [DataMember(Name = "email")]
        public String Email { get; set; }

        [Display(Name = "Login")]
        [DataMember(Name = "login")]
        public String Login { get; set; }
    }
}