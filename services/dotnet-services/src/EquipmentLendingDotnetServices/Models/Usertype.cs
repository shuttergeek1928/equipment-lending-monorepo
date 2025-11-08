using System;
using System.Collections.Generic;

namespace EquipmentLendingDotnetServices.Models;

public partial class Usertype
{
    public int TypeId { get; set; }

    public string TypeValue { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
