﻿using BusinessObjects.Entities;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Authenticate(LoginRequestDto request);
        Task<User> FindOrCreateExternalUser(string email, string fullName);

    }
}
