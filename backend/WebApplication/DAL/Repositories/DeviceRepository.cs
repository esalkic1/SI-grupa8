﻿using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class DeviceRepository : Repository<Device>, IDeviceRepository
    {
        private readonly AppDbContext _context;

        public DeviceRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Device>> GetAllByCompanyUsersIds(List<int> usersIds)
        {
            return await _context.Devices
                .Where(d => usersIds.Contains((int)d.UserID!))
                .Include(d => d.User)
                .Include(x => x.DeviceType)
                .ToListAsync();
        }

        public async Task<Device> GetByMacAddress(string macAddress)
        {
            return await _context.Devices.FirstAsync(x => x.Reference == macAddress);
        }

        public async Task<Device> GetByUserID(int userID)
        {
            return await _context.Devices.FirstAsync(x => x.UserID == userID);
        }

        public async Task<List<Device>> GetFilteredDevicesByUserIds(List<int> userIds, List<int>? deviceTypeIDs = null)
        {
            var data = _context.Devices.Where(x => userIds.Contains((int)x.UserID!)).AsQueryable();

            if (deviceTypeIDs != null)
            {
                data = data.Where(x => deviceTypeIDs.Contains((int)x!.DeviceTypeID!));
            }

            return await data.ToListAsync();

        }

        public async Task<Device> GetWithUser(int deviceId)
        {
            var result =  await _context.Devices
                .Include(x => x.User)
                .FirstAsync(x => x.DeviceID == deviceId);

            if (result != null)
            {
                _context.Entry(result).State = EntityState.Detached;
            }

            return result!;
        }

    }
}
