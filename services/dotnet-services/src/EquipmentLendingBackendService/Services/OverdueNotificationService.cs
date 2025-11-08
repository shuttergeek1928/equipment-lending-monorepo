using EquipmentLendingBackendService.Models;
using EquipmentLendingDotnetServices.Data;
using Microsoft.EntityFrameworkCore;

namespace EquipmentLendingBackendService.Services
{
    public class OverdueNotificationService(IServiceProvider _serviceProvider) : BackgroundService
    {
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var mailerService = scope.ServiceProvider.GetRequiredService<INotificationSender>();

                        //Find all the items that needs to be notified
                        var equipments = await dbService.BorrowingsAndReturns
                            .Where(b => b.IsApproved == true
                                        && b.ReturnDueDate < DateTime.UtcNow
                                        && (b.IsReturned == false || b.IsReturned == null)
                                        && (b.IsOverdueNotified == false || b.IsOverdueNotified == null)
                                        && b.NotificationAttempts < 5)
                            .Include(u => u.Requester)
                            .ToListAsync(cancellationToken);


                        foreach (var request in equipments)
                        {
                            try
                            {
                                var user = request.Requester;
                                var message = $"Dear {user.UserName}, your borrowed equipment (Request ID: {request.RequestId}) is overdue for return since {request.ReturnDueDate?.ToString("yyyy-MM-dd")}. Please return it as soon as possible to avoid penalties.";
                                var messageSubject = "Overdue Equipment Return Notification";

                                var result = mailerService.SendEmailAsync(user.Email, messageSubject, message);

                                if(!result.IsCompleted)
                                    return;

                                request.LastNotifiedAt = DateTimeOffset.UtcNow;
                                request.NotificationAttempts += 1;
                                request.IsOverdueNotified = true;

                                var notification = new BorrowNotification
                                {
                                    Id = Guid.NewGuid(),
                                    BorrowRequestId = request.RequestId,
                                    NotifiedAt = DateTimeOffset.UtcNow,
                                    Channel = "email",
                                    Status = "sent",
                                    Attempts = request.NotificationAttempts,
                                    Message = message
                                };

                                // Build note lines
                                var noteBuilder = new System.Text.StringBuilder();
                                noteBuilder.AppendLine(request.Notes ?? string.Empty);
                                noteBuilder.AppendLine($"[{DateTimeOffset.UtcNow}] Notification sent to user {user.UserName} ({user.Email}).");

                                // If the due date is in the past, append an explicit overdue message
                                if (request.ReturnDueDate.HasValue && request.ReturnDueDate.Value < DateTime.UtcNow && (request.IsReturned == false))
                                {
                                    noteBuilder.AppendLine($"[{DateTimeOffset.UtcNow}] Item is overdue as of {request.ReturnDueDate.Value:yyyy-MM-dd}.");
                                }

                                request.Notes = noteBuilder.ToString();

                                dbService.Notifications.Add(notification);
                                dbService.BorrowingsAndReturns.Update(request);
                                await dbService.SaveChangesAsync(cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                // Log individual notification failure
                                Console.WriteLine($"Failed to notify for Request ID {request.RequestId}: {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (you can use any logging framework)
                    Console.WriteLine($"Error in OverdueNotificationService: {ex.Message}");
                }
                
                await Task.Delay(_interval, cancellationToken);
            }
        }
    }
}
