using HMS.StaffService.Domain.Data;
using HMS.StaffService.DTO.Department;
using HMS.StaffService.DTO.Staff;
using HMS.StaffService.DTO.StaffDocument;
using HMS.StaffService.DTO.StaffNote;
using Mapster;

namespace HMS.StaffService.WebApi.Configurations
{
    public static class MapsterConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            // ----------------------------------------------------
            // Department
            // ----------------------------------------------------
            config.NewConfig<Department, DepartmentDto>();
            config.NewConfig<CreateDepartmentDto, Department>();
            config.NewConfig<BaseDepartmentDto, Department>();

            // ----------------------------------------------------
            // Staff Read Mapping
            // ----------------------------------------------------
            config.NewConfig<Staff, StaffDto>()
                .Map(dest => dest.DepartmentName, src => src.Department != null ? src.Department.DepartmentName : null)
                .Map(dest => dest.ManagerName, src => src.Manager != null ? $"{src.Manager.FirstName} {src.Manager.LastName}".Trim() : null);

            // ----------------------------------------------------
            // Staff Creation Mapping
            // ----------------------------------------------------
            config.NewConfig<CreateStaffDto, Staff>()
                .Ignore(dest => dest.StaffId)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.Department)
                .Ignore(dest => dest.Manager)
                .Ignore(dest => dest.Documents)
                .Ignore(dest => dest.Notes)
                .Ignore(dest => dest.Subordinates);

            // ----------------------------------------------------
            // Staff Update Mapping (Partial Update Safety)
            // ----------------------------------------------------
            config.NewConfig<UpdateStaffDto, Staff>()
                .IgnoreNullValues(true) // Ignore null properties in incoming DTO
                                        // Prevent Primary Key / Audit Overwrites
                .Ignore(dest => dest.StaffId)
                .Ignore(dest => dest.HotelId)
                .Ignore(dest => dest.CreatedAt)
                // PREVENT EF CORE NAVIGATION WIPE
                .Ignore(dest => dest.Department)
                .Ignore(dest => dest.Manager)
                .Ignore(dest => dest.Documents)
                .Ignore(dest => dest.Notes)
                .Ignore(dest => dest.Subordinates);

            // ----------------------------------------------------
            // Staff Document
            // ----------------------------------------------------
            config.NewConfig<StaffDocument, StaffDocumentDto>();
            config.NewConfig<CreateStaffDocumentDto, StaffDocument>();
            config.NewConfig<UpdateStaffDocumentDto, StaffDocument>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.DocumentId)
                .Ignore(dest => dest.StaffId)
                .Ignore(dest => dest.Staff);

            // ----------------------------------------------------
            // Staff Notes
            // ----------------------------------------------------
            config.NewConfig<StaffNotes, StaffNoteDto>();
            config.NewConfig<CreateStaffNoteDto, StaffNotes>();
            config.NewConfig<UpdateStaffNoteDto, StaffNotes>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.NoteId)
                .Ignore(dest => dest.StaffId)
                .Ignore(dest => dest.Staff);
        }
    }
}