using HMS.StaffService.Common.Exceptions;
using HMS.StaffService.Common.Service;
using HMS.StaffService.Domain.Data;
using HMS.StaffService.DTO.StaffNote;
using HMS.StaffService.Repository.Contracts;
using HMS.StaffService.Service.Contracts;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HMS.StaffService.Service
{
    public class StaffNotesService : IStaffNotesService
    {
        private readonly IStaffNotesRepository _noteRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public StaffNotesService(
            IStaffNotesRepository noteRepository,
            IStaffRepository staffRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _noteRepository = noteRepository;
            _staffRepository = staffRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StaffNoteDto>> GetAllAsync()
        {
            var hotelId = _currentUserService.HotelId
                ?? throw new ForbiddenException("Missing HotelId claim.");

            return await _noteRepository.Query()
                .Where(n => n.Staff != null && n.Staff.HotelId == hotelId)
                .ProjectToType<StaffNoteDto>()
                .ToListAsync();
        }

        public async Task<IEnumerable<StaffNoteDto>> GetByStaffIdAsync(Guid staffId)
        {
            var staff = await _staffRepository.GetAsync(staffId)
                ?? throw new NotFoundException($"Staff '{staffId}' not found.");

            if (staff.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            var notes = await _noteRepository.GetByStaffIdAsync(staffId);
            return _mapper.Map<IEnumerable<StaffNoteDto>>(notes);
        }

        public async Task<StaffNoteDto> GetByIdAsync(Guid id)
        {
            var note = await _noteRepository.GetAsync(id)
                ?? throw new NotFoundException($"Note '{id}' not found.");

            var staff = await _staffRepository.GetAsync(note.StaffId);
            if (staff?.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            return _mapper.Map<StaffNoteDto>(note);
        }

        public async Task<StaffNoteDto> CreateAsync(CreateStaffNoteDto dto)
        {
            var staff = await _staffRepository.GetAsync(dto.StaffId)
                ?? throw new NotFoundException($"Staff '{dto.StaffId}' not found.");

            if (staff.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            var currentStaffId = _currentUserService.StaffId
                ?? throw new ForbiddenException("Missing StaffId claim.");

            var entity = _mapper.Map<StaffNotes>(dto);
            entity.CreatedByStaffId = currentStaffId;

            var created = await _noteRepository.AddAsync(entity);
            return _mapper.Map<StaffNoteDto>(created);
        }

        public async Task UpdateAsync(Guid id, UpdateStaffNoteDto dto)
        {
            var note = await _noteRepository.GetAsync(id)
                ?? throw new NotFoundException($"Note '{id}' not found.");

            var staff = await _staffRepository.GetAsync(note.StaffId);
            if (staff?.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            await _noteRepository.UpdateAsync(id, dto);
        }

        public async Task DeleteAsync(Guid id)
        {
            var note = await _noteRepository.GetAsync(id)
                ?? throw new NotFoundException($"Note '{id}' not found.");

            var staff = await _staffRepository.GetAsync(note.StaffId);
            if (staff?.HotelId != _currentUserService.HotelId)
                throw new ForbiddenException("Access denied.");

            await _noteRepository.DeleteAsync(id);
        }
    }
}