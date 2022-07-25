using System;
using System.Collections.Generic;

namespace TestWebApplication.Models
{
    public partial class UserInformation
    {
        public int UserId { get; set; }
        public string? UserAddress { get; set; }
        public string? UserIdentityCardNumber { get; set; }
        public int? UserPhone { get; set; }
    }
}
