using HMSRoomManagement.Data;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMSRoomManagement.Repository.Repository
{
    public class RoomTypeRepository : GenericRepository<RoomType, int>, IRoomTypeRepository
    {
        public RoomTypeRepository(HMSRoomManagementDbContext context) : base(context) { }
    }
}
