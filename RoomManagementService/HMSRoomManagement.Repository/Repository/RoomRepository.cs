using HMSRoomManagement.Data;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMSRoomManagement.Repository.Repository
{
    public class RoomRepository : GenericRepository<Room, int>, IRoomRepository
    {
        public RoomRepository(HMSRoomManagementDbContext context) : base(context) { }
    }
}
