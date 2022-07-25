using System;
using System.Collections.Generic;

namespace DataBaseTest.Models
{
  

    public partial class UserModel
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPassWord { get; set; }
        public string? UserEmail { get; set; }
    }
}

