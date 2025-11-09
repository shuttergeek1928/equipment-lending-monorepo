using EquipmentLendingBackendService.Models;
using System;
using System.Collections.Generic;

namespace EquipmentLendingDotnetServices.Models;

public class BorrowingsAndReturns
{
    public Guid Id { get; set; }

    public int EquipmentId { get; set; }

    public int RequestId { get; set; }

    public int RequesterId { get; set; }

    public int RequestedQuantity { get; set; }

    public DateTime? RequestedOn { get; set; }

    public DateTime? ReturnDueDate { get; set; }

    public DateTime? ReturnedOn { get; set; }

    public bool? IsApproved { get; set; }

    public bool? IsReturned { get; set; }

    public string? Notes { get; set; }

    public DateTimeOffset? LastNotifiedAt { get; set; }
    public int NotificationAttempts { get; set; }
    public bool IsOverdueNotified { get; set; }
    public ICollection<BorrowNotification> Notifications { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;

    public virtual User Requester { get; set; } = null!;
}
