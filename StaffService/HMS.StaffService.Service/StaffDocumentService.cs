using HMS.StaffService.Common.Exceptions;
using HMS.StaffService.Common.Service;
using HMS.StaffService.DTO.StaffDocument;
using HMS.StaffService.Repository.Contracts;
using HMS.StaffService.Service.Contracts;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HMS.StaffService.Service
{
    public class StaffDocumentService : IStaffDocumentService
    {
        private readonly IStaffDocumentRepository _documentRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public StaffDocumentService(
            IStaffDocumentRepository documentRepository,
            IStaffRepository staffRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _documentRepository = documentRepository;
            _staffRepository = staffRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StaffDocumentDto>> GetAllAsync()
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            return await _documentRepository.Query()
                .Where(d => d.Staff != null && d.Staff.HotelId == hotelId)
                .ProjectToType<StaffDocumentDto>()
                .ToListAsync();
        }

        private void EnsureCanRead(Guid ownerStaffId)
        {
            var role = _currentUserService.Role;
            if ((role == "Staff" || role == "Manager") && ownerStaffId != _currentUserService.StaffId)
                throw new ForbiddenException("You can only view your own documents.");
        }

        public async Task<IEnumerable<StaffDocumentDto>> GetByStaffIdAsync(Guid staffId)
        {
            EnsureCanRead(staffId);
            var staff = await _staffRepository.GetAsync(staffId)
                ?? throw new NotFoundException($"Staff '{staffId}' not found.");

            if (staff.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            var docs = await _documentRepository.GetByStaffIdAsync(staffId);
            return _mapper.Map<IEnumerable<StaffDocumentDto>>(docs);
        }

        public async Task<StaffDocumentDto> GetByIdAsync(Guid id)
        {
            var doc = await _documentRepository.GetAsync(id)
                ?? throw new NotFoundException($"Document '{id}' not found.");

            var staff = await _staffRepository.GetAsync(doc.StaffId);
            if (staff?.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            EnsureCanRead(doc.StaffId);
            return _mapper.Map<StaffDocumentDto>(doc);
        }

        public async Task<StaffDocumentDto> CreateAsync(CreateStaffDocumentDto dto)
        {
            var staff = await _staffRepository.GetAsync(dto.StaffId)
                ?? throw new NotFoundException($"Staff '{dto.StaffId}' not found.");

            if (staff.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            return await _documentRepository.AddAsync<CreateStaffDocumentDto, StaffDocumentDto>(dto);
        }

        public async Task UpdateAsync(Guid id, UpdateStaffDocumentDto dto)
        {
            var doc = await _documentRepository.GetAsync(id)
                ?? throw new NotFoundException($"Document '{id}' not found.");

            var staff = await _staffRepository.GetAsync(doc.StaffId);
            if (staff?.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            await _documentRepository.UpdateAsync(id, dto);
        }

        public async Task DeleteAsync(Guid id)
        {
            var doc = await _documentRepository.GetAsync(id)
                ?? throw new NotFoundException($"Document '{id}' not found.");

            var staff = await _staffRepository.GetAsync(doc.StaffId);
            if (staff?.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            await _documentRepository.DeleteAsync(id);
        }
    }
}