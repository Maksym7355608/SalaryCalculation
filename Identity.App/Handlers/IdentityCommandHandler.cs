﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Identity.App.Abstract;
using Identity.App.Commands;
using Identity.Data.Data;
using Identity.Data.Entities;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Identity.App.Handlers;

public class IdentityCommandHandler : BaseIdentityCommandHandler, IIdentityCommandHandler
{
    public IdentityCommandHandler(IIdentityUnitOfWork work, IMapper mapper) : base(work, mapper)
    {
    }

    public async Task CreateUserAsync(UserCreateCommand command)
    {
        var newUser = Mapper.Map<User>(command);
        await Work.GetCollection<User>(nameof(User)).InsertOneAsync(newUser);
    }

    public async Task UpdateUserAsync(UserUpdateCommand command)
    {
        var updatedUser = Mapper.Map<User>(command);
        await Work.GetCollection<User>(nameof(User)).ReplaceOneAsync(x => x.Id == new ObjectId(command.Id), updatedUser);
    }

    public async Task DeleteUserAsync(ObjectId userId)
    {
        await Work.GetCollection<User>(nameof(User)).DeleteOneAsync(x => x.Id == userId);
    }

    public async Task AddRoleToUserAsync(ObjectId userId, ObjectId roleId)
    {
        var user = await Work.GetCollection<User>(nameof(User)).Find(x => x.Id == userId).FirstOrDefaultAsync();
        if (user != null)
        {
            var roles = user.Roles.ToList();
            roles.Add(roleId);
            user.Roles = roles;
            await Work.GetCollection<User>(nameof(User)).ReplaceOneAsync(x => x.Id == userId, user);
        }
    }

    public async Task RemoveRoleFromUserAsync(ObjectId userId, ObjectId roleId)
    {
        var user = await Work.GetCollection<User>(nameof(User)).Find(x => x.Id == userId).FirstOrDefaultAsync();
        if (user != null)
        {
            var roles = user.Roles.ToList();
            roles.RemoveAll(x => x == roleId);
            user.Roles = roles;
            await Work.GetCollection<User>(nameof(User)).ReplaceOneAsync(x => x.Id == userId, user);
        }
    }


    public async Task<string> AuthenticateAsync(string username, string password)
    {
        var user = await Work.GetCollection<User>(nameof(User)).Find(x => x.UserName == username).FirstOrDefaultAsync();
        if (user != null && VerifyPasswordHash(password, user.PasswordHash))
        {
            var roles = await Work.GetCollection<Role>()
                .Find(x => user.Roles.Contains(x.Id))
                .Project(x => x.Name)
                .ToListAsync();
            // Generate and return the JWT token
            var token = GenerateJwtToken(user, roles);
            return token;
        }
        else
        {
            // Authentication failed
            return null;
        }
    }
    
    private bool VerifyPasswordHash(string password, string storedHash)
    {
        using (var hmac = new HMACSHA512())
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i])
                    return false;
            }
        }

        return true;
    }
    
    private string GenerateJwtToken(User user, IEnumerable<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("secretKey");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}