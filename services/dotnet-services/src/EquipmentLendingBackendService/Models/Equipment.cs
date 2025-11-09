using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EquipmentLendingDotnetServices.Models;

public class Equipment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int EquipmentId { get; set; }

    public string EquipmentName { get; set; } = null!;

    public string? Catgory { get; set; }

    public string? EquipmentCondition { get; set; } = "Good";

    public int TotalQuantity { get; set; } = 1;

    public int AvailableQuantity { get; set; } = 1;

    public bool IsAvailable { get; set; } = true;

    public DateOnly? AddedOn { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public virtual ICollection<BorrowingsAndReturns> BorrowingsAndReturns { get; set; } = new List<BorrowingsAndReturns>();
}
