using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubSentry.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for N-Tier relationships
        public List<Subscription> Subscriptions { get; set; } = new();
    }
}