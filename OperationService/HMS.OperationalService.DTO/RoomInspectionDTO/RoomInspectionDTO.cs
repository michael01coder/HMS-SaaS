using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.DTO.RoomInspectionDTO
{
    public class RoomInspectionDTO
    {
        public int InspectionId { get; set; }
        public string InspectionCode { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public Guid InspectorId { get; set; }
        public string InspectionType { get; set; } = string.Empty;
        public int ConditionRating { get; set; }
        public string? Remarks { get; set; }
        public DateTime InspectionDate { get; set; }
    }
}
