using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Identity.App.Abstract;
using Identity.App.Commands;
using Identity.Data.Data;
using Identity.Data.Entities;
using Identity.Data.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using SalaryCalculation.Shared.Extensions.EnumExtensions;

namespace Identity.App.Handlers;

public class IdentityCommandHandler : BaseIdentityCommandHandler, IIdentityCommandHandler
{
    public IdentityCommandHandler(IIdentityUnitOfWork work, ILogger logger, IMapper mapper) : base(work, logger, mapper)
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
            await Work.GetCollection<User>(nameof(User)).DeleteOneAsync(x => x.Id == userId);
        }
    }


    public async Task<string> AuthenticateAsync(string username, string password)
    {
        var user = await Work.GetCollection<User>(nameof(User)).Find(x => x.UserName == username).FirstOrDefaultAsync();
        if (user != null && VerifyPasswordHash(password, user.PasswordHash))
        {
            var roles = await Work.GetCollection<Role>()
                .Find(x => user.Roles.Contains(x.Id))
                .Project<RoleForClaim>(Builders<Role>.Projection
                    .Include(x => x.Name)
                    .Include(x => x.Permissions))
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
    
    private string GenerateJwtToken(User user, IEnumerable<RoleForClaim> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("secretKey");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };

        claims.AddRange(roles.SelectMany(role => role.Permissions.Select(x => new Claim(ClaimTypes.Role, ((EPermission)x).GetDescription()))));

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

    private record RoleForClaim(string Name, IEnumerable<int> Permissions);

}