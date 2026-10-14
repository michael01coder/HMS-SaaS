namespace HMS.OperationalService.Common.Enums
{
    public enum TaskPriority { Low, Medium, High, Urgent }
    public enum TaskStatus { Pending, InProgress, Completed, Cancelled, Resolved }
    public enum MaintenanceCategory { Plumbing, Electrical, HVAC, Carpentry, Appliance, General }
    public enum ServiceRequestType { Dining, HousekeepingAmenity, Transport, Concierge, Luggage }
    public enum InspectionTyperoutine { Daily, Checkout, DeepClean, QualityAudit }
    public enum OperationalTaskType { Housekeeping, Maintenance, ServiceRequest, Inspection }
}