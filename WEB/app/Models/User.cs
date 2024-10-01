﻿using System.ComponentModel.DataAnnotations;

namespace app.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public bool Status { get; set; }
    }
}
