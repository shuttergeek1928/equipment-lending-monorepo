using EquipmentLendingDotnetServices.Models;

namespace EquipmentLendingDotnetServices.DTOs
{
    public class AddEquipmentDTO
    {
        public string EquipmentName { get; set; } = null!;

        public string? Catgory { get; set; }

        public string? EquipmentCondition { get; set; }

        public int TotalQuantity { get; set; }

        public int AvailableQuantity { get; set; }

        public bool? IsAvailable { get; set; }
    }

    public class EditEquipmentDTO
    {
        public string? EquipmentCondition { get; set; }

        public int TotalQuantity { get; set; }

        public int AvailableQuantity { get; set; }

        public bool? IsAvailable { get; set; }
    }
}
