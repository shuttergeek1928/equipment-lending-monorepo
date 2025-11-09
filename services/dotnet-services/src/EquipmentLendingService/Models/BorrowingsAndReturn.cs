using System;
using System.Collections.Generic;

namespace EquipmentLendingDotnetServices.Models;

public partial class BorrowingsAndReturns
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

    public virtual Equipment Equipment { get; set; } = null!;

    public virtual User Requester { get; set; } = null!;
}
