﻿using AutoMapper;
using FilePocket.Contracts.Services;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using FilePocket.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FilePocket.Shared.Claims;

namespace FilePocket.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;
    private readonly JwtConfigurationModel _jwtConfiguration;
    private readonly UserManager<User> _userManager;
    private User? _user;

    public AuthenticationService(IMapper mapper, ILoggerService logger, UserManager<User> userManager, IOptions<JwtConfigurationModel> options)
    {
        _mapper = mapper;
        _logger = logger;
        _userManager = userManager;
        _jwtConfiguration = options.Value;
    }

    public async Task<IdentityResult> RegisterUser(UserRegistrationModel userForRegistration)
    {
        var user = _mapper.Map<User>(userForRegistration);
        var result = await _userManager.CreateAsync(user, userForRegistration.Password!);

        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, userForRegistration.Roles!);
        }

        return result;
    }

    public async Task<bool> ValidateUser(UserLoginModel userLoginModel)
    {
        _user = await _userManager.FindByEmailAsync(userLoginModel.Email!);
        var result = _user is not null && await _userManager.CheckPasswordAsync(_user, userLoginModel.Password!);

        if (!result)
        {
            _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed. Wrong user name or password.");
        }

        return result;
    }

    public async Task<TokenModel> CreateToken(bool populateExp)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        var refreshToken = GenerateRefreshToken();
        _user!.RefreshToken = refreshToken;

        if (populateExp)
        {
            _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(3);
        }

        await _userManager.UpdateAsync(_user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new TokenModel { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    public async Task<TokenModel> RefreshToken(TokenModel tokenModel)
    {
        var principal = GetPrincipalFromExpiredToken(tokenModel.AccessToken!);
        var user = await _userManager.FindByNameAsync(principal.Identity!.Name!);

        if (user is null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new RefreshTokenBadRequest();
        }

        _user = user;

        return await CreateToken(populateExp: false);
    }


    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(_jwtConfiguration.TokenKey!);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, _user!.UserName!),
            new (CustomClaimTypes.UserId, _user!.Id.ToString())
        };

        var roles = await _userManager.GetRolesAsync(_user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtConfiguration.ValidIssuer,
            audience: _jwtConfiguration.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
            signingCredentials: signingCredentials);

        return tokenOptions;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.TokenKey!)),
            ValidateLifetime = false,
            ValidIssuer = _jwtConfiguration.ValidIssuer,
            ValidAudience = _jwtConfiguration.ValidAudience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}