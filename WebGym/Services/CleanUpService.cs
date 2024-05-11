using Gym.Model;
using Microsoft.EntityFrameworkCore;
namespace WebGym.Services
{
    public class CleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DBContext>();
                    var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
                    var expiredFreezes = await db.ActiveFreezes
                        .Include(f => f.MembershipInstance)
                        .Where(f => f.EndDate < today)
                        .ToListAsync();
                    foreach (var freeze in expiredFreezes)
                    {
                        freeze.MembershipInstance.Status = Status.Active;
                        //db.ActiveFreezes.Remove(freeze);
                    }
                    //db.ActiveFreezes.RemoveRange(expiredFreezes);

                    // Clear expired memberships
                    var expiredMemberships = await db.MembershipInstances
                        .Where(m => m.EndDate < today)
                        .ToListAsync();
                   db.MembershipInstances.RemoveRange(expiredMemberships);

                    //Activate meberships purchased 30 days ago
                   

                    var membershipsToActivate = await db.MembershipInstances
                        .Include(mi => mi.Membership)
                        .ThenInclude(m => m.Freeze) // Include the Freeze related to the Membership
                        .Include(mi => mi.User) // Include the User related to the MembershipInstance
                        .Where(m => m.PurchaseDate.Value.AddDays(30) < today && m.Status == Status.Inactive)
                        .ToListAsync();

                    foreach (var membershipInstance in membershipsToActivate)
                    {
                        membershipInstance.StartDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
                        membershipInstance.EndDate = membershipInstance.StartDate.Value.AddMonths(membershipInstance.Membership.Months.Value);
                        membershipInstance.Status = Status.Active;
                        FreezeActive freezeActive = new FreezeActive
                        {
                            DaysLeft = membershipInstance.Membership.Freeze.Days,
                            MembershipInstanceId = membershipInstance.Id,
                           
                        };
                        db.ActiveFreezes.Add(freezeActive);

                    }
                    await db.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
