﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Application.Features.Users.DTOs;
using TaskManagementApi.Domains.Wrapper;

namespace TaskManagementApi.Application.Common.Interfaces.IUser
{
    public interface IUserService
    {
        Task<ResponseType<UserProfileDto>> UserProfileAsync();
    }
}
