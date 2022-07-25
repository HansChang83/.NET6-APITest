using System;
using System.Collections.Generic;
namespace DataBaseTest.Models
{
    public partial class UserInfoModel
    {
        public int UserId { get; set; }
        public string? UserAddress { get; set; }
        public string? UserIdentityCardNumber { get; set; }
        public int? UserPhone { get; set; }
    }
}