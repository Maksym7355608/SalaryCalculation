using Identity.App.Commands;
using MongoDB.Bson;

namespace Identity.App.Abstract;

public interface IRoleCommandHandler
{
    /// <summary>
    /// Створює нову роль.
    /// </summary>
    /// <param name="command">Об'єкт команди для створення ролі.</param>
    Task CreateRole(RoleCreateCommand command);

    /// <summary>
    /// Оновлює існуючу роль за її ідентифікатором.
    /// </summary>
    /// <param name="roleId">Ідентифікатор ролі.</param>
    /// <param name="command">Об'єкт команди для оновлення ролі.</param>
    /// <returns>Показник, що вказує, чи була оновлена роль.</returns>
    Task<bool> UpdateRole(ObjectId roleId, RoleUpdateCommand command);

    /// <summary>
    /// Видаляє роль за її ідентифікатором.
    /// </summary>
    /// <param name="roleId">Ідентифікатор ролі.</param>
    /// <returns>Показник, що вказує, чи була видалена роль.</returns>
    Task<bool> DeleteRole(ObjectId roleId);
}