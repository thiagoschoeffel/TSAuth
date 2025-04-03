using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TSAuth.Api.Application.Services;
using TSAuth.Api.Contracts.Auth;
using TSAuth.Api.Infrastructure;
using TSAuth.Api.Models;

namespace TSAuth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(TsAuthDbContext dbContext, AuthService authService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Auth([FromBody] AuthRequest credentials)
        {
            if (string.IsNullOrWhiteSpace(credentials.Email) || string.IsNullOrWhiteSpace(credentials.Password))
            {
                return BadRequest(new
                {
                    error = "Invalid credentials"
                });
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == credentials.Email);
            if (user == null) return Unauthorized();

            var validPassword = HashService.VerifyPassword(credentials.Password, user.Password);
            if (!validPassword) return Unauthorized();

            var accessToken = authService.GenerateToken(user.Id.ToString(), user.Email);
            var refreshTokenValue = authService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            dbContext.RefreshTokens.Add(refreshToken);
            await dbContext.SaveChangesAsync();

            return Ok(new AuthResponse(accessToken, refreshTokenValue));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var existingRefreshToken =
                await dbContext.RefreshTokens.FirstOrDefaultAsync(refreshToken =>
                    refreshToken.Token == request.RefreshToken);
            if (existingRefreshToken == null) return Unauthorized();
            if (existingRefreshToken.IsRevoked) return Unauthorized();
            if (existingRefreshToken.ExpiryDate < DateTime.UtcNow) return Unauthorized();

            var user = await dbContext.Users.FindAsync(existingRefreshToken.UserId);
            if (user == null) return Unauthorized();

            var newAccessToken = authService.GenerateToken(user.Id.ToString(), user.Email);
            var newRefreshTokenValue = authService.GenerateRefreshToken();

            existingRefreshToken.IsRevoked = true;

            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshTokenValue,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            dbContext.RefreshTokens.Add(newRefreshToken);
            await dbContext.SaveChangesAsync();

            return Ok(new AuthResponse(newAccessToken, newRefreshTokenValue));
        }
    }
}