using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubSentry.DAL.Entities;

namespace SubSentry.DAL.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // 1. Seed the Subscription Tiers if they don't exist
            if (!context.SubscriptionTiers.Any())
            {
                var tiers = new List<SubscriptionTier>
                {
                    new SubscriptionTier { Name = "Basic", Price = 20.00m, Description = "Standard access" },
                    new SubscriptionTier { Name = "Pro", Price = 50.00m, Description = "Advanced FinTech tools" },
                    new SubscriptionTier { Name = "Enterprise", Price = 90.00m, Description = "Full suite for teams" }
                };
                context.SubscriptionTiers.AddRange(tiers);
                context.SaveChanges();
            }

            // 2. Read the Kaggle CSV and map it to Users & Subscriptions
            if (!context.Users.Any())
            {
                // Find the CSV file (Ensure it's set to "Copy if newer" in VS Properties)
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "churn_data.csv");

                if (File.Exists(path))
                {
                    // Read file, skip header, take exactly 100 rows for a clean portfolio demo
                    var lines = File.ReadAllLines(path).Skip(1).Take(100);
                    var random = new Random();

                    var tiers = context.SubscriptionTiers.ToList();
                    var basic = tiers.First(t => t.Name == "Basic");
                    var pro = tiers.First(t => t.Name == "Pro");
                    var enterprise = tiers.First(t => t.Name == "Enterprise");

                    var newUsers = new List<User>();

                    foreach (var line in lines)
                    {
                        var cols = line.Split(',');

                        // Telco CSV Columns: 
                        // cols[0] = customerID, cols[5] = tenure, cols[18] = MonthlyCharges, cols[20] = Churn
                        var customerId = cols[0];
                        var tenureMonths = int.TryParse(cols[5], out int t) ? t : 1;
                        var charge = decimal.TryParse(cols[18], out decimal c) ? c : 20m;
                        var churn = cols[20].Trim();

                        // Map to User
                        var user = new User
                        {
                            FullName = $"Client {customerId}",
                            Email = $"{customerId.ToLower()}@example.com"
                        };

                        // Determine Tier based on real Kaggle MonthlyCharges
                        var tierId = charge > 70 ? enterprise.Id : (charge > 40 ? pro.Id : basic.Id);

                        // Map Churn to Status. If not churned, randomly assign some as "Overdue" for your Risk Alerts
                        var status = churn == "Yes" ? "Cancelled" : (random.Next(0, 10) > 8 ? "Overdue" : "Active");

                        // Map to Subscription
                        user.Subscriptions.Add(new Subscription
                        {
                            SubscriptionTierId = tierId,
                            StartDate = DateTime.UtcNow.AddMonths(-tenureMonths),
                            // If overdue, set date in the past. Otherwise, future date.
                            NextBillingDate = status == "Overdue" ? DateTime.UtcNow.AddDays(-5) : DateTime.UtcNow.AddDays(random.Next(1, 28)),
                            Status = status
                        });

                        newUsers.Add(user);
                    }

                    // Batch save to the database (Highly efficient N-Tier practice)
                    context.Users.AddRange(newUsers);
                    context.SaveChanges();
                }

                else
                {
                    // If the file is missing, THROW an error so we can see exactly where it is looking
                    throw new FileNotFoundException($"CRITICAL: Could not find the Kaggle CSV file. I am looking here: {path}");
                }
            }
        }
    }
}