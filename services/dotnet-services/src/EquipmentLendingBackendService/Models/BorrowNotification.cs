using EquipmentLendingDotnetServices.Models;

namespace EquipmentLendingBackendService.Models
{
    public class BorrowNotification
    {
        public Guid Id { get; set; }
        public int BorrowRequestId { get; set; }
        public BorrowingsAndReturns BorrowRequest { get; set; }
        public DateTimeOffset NotifiedAt { get; set; }
        public string Channel { get; set; } // email, in_app
        public string Status { get; set; } // sent, failed
        public int Attempts { get; set; }
        public string Message { get; set; }
    }
}
