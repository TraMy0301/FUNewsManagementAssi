﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(List<Claim> claims);
        string GenerateRefreshToken(int userId);
        ClaimsPrincipal ValidateRefreshToken(string refreshToken);
        string GetJwtId(string token);
        string GetJwtId(ClaimsPrincipal principal);
    }
}
