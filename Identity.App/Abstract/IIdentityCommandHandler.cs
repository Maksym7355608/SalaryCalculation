using Identity.App.Commands;
using Identity.App.DtoModels;
using MongoDB.Bson;

namespace Identity.App.Abstract;

public interface IIdentityCommandHandler
{
    /// <summary>
    /// Створює нового користувача в системі.
    /// </summary>
    /// <param name="command">Інформація про користувача для створення.</param>
    Task CreateUserAsync(UserCreateCommand command);

    /// <summary>
    /// Оновлює інформацію про користувача.
    /// </summary>
    /// <param name="command">Інформація про користувача для оновлення.</param>
    Task UpdateUserAsync(UserUpdateCommand command);

    /// <summary>
    /// Видаляє користувача з системи за його ідентифікатором.
    /// </summary>
    /// <param name="userId">Ідентифікатор користувача, якого слід видалити.</param>
    Task DeleteUserAsync(ObjectId userId);

    /// <summary>
    /// Додає роль до користувача.
    /// </summary>
    /// <param name="userId">Ідентифікатор користувача, до якого слід додати роль.</param>
    /// <param name="roleId">Ідентифікатор ролі, яку слід додати.</param>
    Task AddRoleToUserAsync(ObjectId userId, ObjectId roleId);

    /// <summary>
    /// Видаляє роль з користувача.
    /// </summary>
    /// <param name="userId">Ідентифікатор користувача, з якого слід видалити роль.</param>
    /// <param name="roleId">Ідентифікатор ролі, яку слід видалити.</param>
    Task RemoveRoleFromUserAsync(ObjectId userId, ObjectId roleId);
    
    /// <summary>
    /// Аутентифікація користувача за ім'ям користувача та паролем і повертає токен доступу (Bearer token).
    /// </summary>
    /// <param name="username">Ім'я користувача.</param>
    /// <param name="password">Пароль користувача.</param>
    /// <returns>Bearer token для авторизації користувача.</returns>
    Task<AuthorizationDto> AuthenticateAsync(string username, string password);
}

