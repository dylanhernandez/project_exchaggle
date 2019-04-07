using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Exchaggle.Models
{
    public class Account
    {
        public Account()
        {
            Items = new List<Item>();
        }

        [Key]
        public int AccountId { get; set; }
        public string EmailAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ContactName { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}