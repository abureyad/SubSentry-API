using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubSentry.DAL.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public string Action { get; set; } = string.Empty; // e.g., "Status Changed"
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Subscription? Subscription { get; set; }
    }
}
