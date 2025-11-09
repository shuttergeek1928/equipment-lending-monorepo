using System;
using System.Collections.Generic;

namespace EquipmentLendingDotnetServices.Models;

public partial class User
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public int UserType { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastAccessedAt { get; set; }

    public virtual ICollection<BorrowingsAndReturns> BorrowingsAndReturns { get; set; } = new List<BorrowingsAndReturns>();

    public virtual Usertype UserTypeNavigation { get; set; } = null!;
}
