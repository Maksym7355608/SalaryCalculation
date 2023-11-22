using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Identity.App.Abstract;
using Identity.App.Commands;
using Identity.App.DtoModels;
using Identity.App.Helpers;
using Identity.Data.Data;
using Identity.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Data.Enums;
using SalaryCalculation.Shared.Common.Validation;
using SalaryCalculation.Shared.Extensions.EnumExtensions;

namespace Identity.App.Handlers;

public class IdentityCommandHandler : BaseIdentityCommandHandler, IIdentityCommandHandler
{
    private readonly IConfiguration _configuration;
    
    public IdentityCommandHandler(IIdentityUnitOfWork work, ILogger<IdentityCommandHandler> logger, IMapper mapper,
        IConfiguration configuration) : base(work, logger, mapper)
    {
        _configuration = configuration;
    }

    public async Task CreateUserAsync(UserCreateCommand command)
    {
        var newUser = Mapper.Map<User>(command);
        var existingUsers = await Work.GetCollection<User>()
            .Find(x => x.Username == command.Username || x.Email == command.Email)
            .AnyAsync();
        if (existingUsers)
            throw new EntityExistingException("Entity with username {0} or email {1} was exist", command.Username,
                command.Email);
        var encryptor = new PasswordEncryptor(_configuration["JwtSettings:SecretKey"] ?? string.Empty);
        newUser.Id = ObjectId.GenerateNewId();
        newUser.PasswordHash = encryptor.EncryptPassword(command.Password);
        await Work.GetCollection<User>(nameof(User)).InsertOneAsync(newUser);
    }

    public async Task UpdateUserAsync(UserUpdateCommand command)
    {
        var updatedUser = Mapper.Map<User>(command);
        var encryptor = new PasswordEncryptor(_configuration["JwtSettings:SecretKey"] ?? string.Empty);
        updatedUser.PasswordHash = encryptor.EncryptPassword(command.Password);
        await Work.GetCollection<User>(nameof(User)).ReplaceOneAsync(x => x.Id == command.Id, updatedUser);
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
            if (roles.Contains(roleId))
                throw new Exception("Role was exist");
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
            await Work.GetCollection<User>(nameof(User)).UpdateOneAsync(x => x.Id == userId, Builders<User>.Update.Set(x => x.Roles, roles));
        }
    }


    public async Task<AuthorizationDto> AuthenticateAsync(string username, string password)
    {
        var user = await Work.GetCollection<User>(nameof(User)).Find(x => x.Username == username).FirstOrDefaultAsync();
        var decryptor =  new PasswordEncryptor(_configuration["JwtSettings:SecretKey"] ?? string.Empty);
        if (user != null && decryptor.DecryptPassword(user.PasswordHash) == password)
        {
            var roles = await Work.GetCollection<Role>()
                .Find(x => user.Roles.Contains(x.Id))
                .Project<RoleForClaim>(Builders<Role>.Projection
                    .Include(x => x.Id)
                    .Include(x => x.Name)
                    .Include(x => x.Permissions))
                .ToListAsync();
            // Generate and return the JWT token
            var token = GenerateJwtToken(user, roles);
            return new AuthorizationDto()
            {
                Token = token,
                User = new AuthorizedUserDto()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    OrganizationId = user.OrganizationId,
                    Username = username,
                    Roles = roles.Select(role => new IdNamePair<string, string>(role.Id.ToString(), role.Name)).ToArray(),
                    Permissions = roles.SelectMany(x => x.Permissions).Distinct().ToArray()
                }
            };
        }
        else
        {
            // Authentication failed
            throw new EntityNotFoundException("User with username {0} or password does not exist", username);
        }
    }

    private string GenerateJwtToken(User user, IEnumerable<RoleForClaim> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"] ?? string.Empty);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new("Organization", user.OrganizationId.ToString()),
        };

        claims.AddRange(roles.SelectMany(role => role.Permissions.Select(x => new Claim(ClaimTypes.Role, ((EPermission)x).GetDescription()))));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _configuration["JwtSettings:Audience"],
            Issuer = _configuration["JwtSettings:Issuer"],
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private record RoleForClaim(ObjectId Id, string Name, IEnumerable<int> Permissions);

}