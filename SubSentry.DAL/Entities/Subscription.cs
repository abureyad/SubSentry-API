using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubSentry.DAL.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionTierId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime NextBillingDate { get; set; }

        public string Status { get; set; } = "Active"; // status would allow us to trigger alerts if needed

        // Relationship properties
        public User? User { get; set; }
        public SubscriptionTier? Tier { get; set; }
    }
}