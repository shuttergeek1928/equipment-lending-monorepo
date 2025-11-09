namespace EquipmentLendingBackendService.Models.DTOs
{
    public class RequestEquipmentDTO
    {
        public int EquipmentId { get; set; }
        //public int RequesterId { get; set; }
        public int Quantity { get; set; }
        public DateOnly RequestedOn { get; set; } = DateOnly.FromDateTime(DateTime.Today);            
    }

    public class ApproveEquipmentDTO
    {
        public int RequestId { get; set; }
        public bool IsApproved { get; set; }
    }
}