using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LunaWash.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LunaWash.API.HostedServices
{
    public class PriorityEscalationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PriorityEscalationService> _logger;

        public PriorityEscalationService(IServiceProvider serviceProvider, ILogger<PriorityEscalationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Priority Escalation Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Run the escalation logic
                    await RunEscalationAsync(stoppingToken);

                    // Calculate time until next midnight (00:00:00)
                    var now = DateTime.UtcNow;
                    var localNow = now.AddHours(7); // Vietnam Time (UTC+7)
                    var nextMidnightLocal = localNow.Date.AddDays(1);
                    var nextMidnightUtc = nextMidnightLocal.AddHours(-7);
                    
                    var delay = nextMidnightUtc - now;
                    
                    _logger.LogInformation($"Next priority escalation will run at {nextMidnightUtc} UTC (in {delay.TotalHours:F2} hours).");
                    
                    // Wait until midnight
                    await Task.Delay(delay, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing PriorityEscalationService.");
                    // If error occurs, wait 1 hour before retrying to prevent rapid failure loop
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }

        private async Task RunEscalationAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Get current UTC date in Vietnam timezone
                var todayVietnamDate = DateTime.UtcNow.AddHours(7).Date;

                // Find tasks that are not completed
                var pendingTasks = await context.MaintenanceTasks
                    .Where(t => t.Status != "Hoàn thành")
                    .ToListAsync(stoppingToken);

                int updatedCount = 0;

                foreach (var task in pendingTasks)
                {
                    var taskLastUpdateDate = task.CreatedAt.AddHours(7).Date;

                    // If the task was created/updated before today, escalate its priority
                    if (taskLastUpdateDate < todayVietnamDate)
                    {
                        var newPriority = task.Priority switch
                        {
                            "Thấp" => "Trung bình",
                            "Trung bình" => "Cao",
                            "Cao" => "Khẩn cấp",
                            _ => task.Priority // If already "Khẩn cấp", don't change
                        };

                        if (task.Priority != newPriority)
                        {
                            task.Priority = newPriority;
                            updatedCount++;
                        }
                    }
                }

                if (updatedCount > 0)
                {
                    await context.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation($"Priority Escalation Service successfully escalated {updatedCount} tasks.");
                }
                else
                {
                    _logger.LogInformation("Priority Escalation Service ran, but no tasks required escalation.");
                }
            }
        }
    }
}
