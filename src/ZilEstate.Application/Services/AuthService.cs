using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ZilEstate.Application.Common.Interfaces;
using ZilEstate.Application.DTOs;
using ZilEstate.Domain.Entities;
using BC = BCrypt.Net.BCrypt;

namespace ZilEstate.Application.Services;

public class AuthService
{
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email, cancellationToken))
            return null;

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BC.HashPassword(dto.Password),
            Role = "User"
        };

        if (dto.IsAgency)
        {
            var agency = new Agency
            {
                Name = dto.AgencyName ?? dto.FullName,
                Description = dto.AgencyDescription ?? string.Empty,
                LogoUrl = dto.AgencyLogoUrl,
                Website = dto.AgencyWebsite,
                Phone = dto.AgencyPhone ?? string.Empty,
                Email = dto.AgencyEmail ?? dto.Email,
                Address = dto.AgencyAddress,
                IsVerified = false
            };
            _context.Agencies.Add(agency);
            user.Agency = agency;
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email, cancellationToken);
        
        if (user == null || !BC.Verify(dto.Password, user.PasswordHash))
            return null;

        return GenerateAuthResponse(user);
    }

    private AuthResponseDto GenerateAuthResponse(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "zilestate_secret_key_long_enough_for_security");
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FullName", user.FullName),
                new Claim("AgencyId", user.AgencyId?.ToString() ?? string.Empty)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            AgencyId = user.AgencyId,
            Token = tokenHandler.WriteToken(token)
        };
    }
}
