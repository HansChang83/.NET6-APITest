using System;
using System.Collections.Generic;

namespace TestWebApplication.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPassWord { get; set; }
        public string? UserEmail { get; set; }
    }
}
