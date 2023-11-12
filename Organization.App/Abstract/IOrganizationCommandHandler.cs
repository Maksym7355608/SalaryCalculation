using Organization.App.Commands;
using Organization.App.DtoModels;
using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Abstract;

public interface IOrganizationCommandHandler
{
    Task<OrganizationDto> GetOrganizationAsync(int organizationId);
    Task<IEnumerable<OrganizationUnitDto>> SearchOrganizationUnitsAsync(OrganizationUnitSearchCommand command);
    Task<IEnumerable<PositionDto>> SearchPositionsAsync(PositionSearchCommand command);
    
    /// <summary>
    /// Створює нову організацію.
    /// </summary>
    Task CreateOrganizationAsync(OrganizationCreateCommand command);

    /// <summary>
    /// Оновлює існуючу організацію.
    /// </summary>
    Task<bool> UpdateOrganizationAsync(OrganizationUpdateCommand command);

    /// <summary>
    /// Видаляє організацію за заданим ідентифікатором.
    /// </summary>
    Task<bool> DeleteOrganizationAsync(int organizationId);

    /// <summary>
    /// Оновлює права доступу організації.
    /// </summary>
    Task<bool> UpdateOrganizationPermissionsAsync(OrganizationPermissionUpdateCommand command);

    /// <summary>
    /// Створює нову структурну одиницю організації.
    /// </summary>
    Task CreateOrganizationUnitAsync(OrganizationUnitCreateCommand command);

    /// <summary>
    /// Оновлює існуючу структурну одиницю організації.
    /// </summary>
    Task<bool> UpdateOrganizationUnitAsync(OrganizationUnitUpdateCommand command);

    /// <summary>
    /// Видаляє структурну одиницю організації за заданим ідентифікатором.
    /// </summary>
    Task<bool> DeleteOrganizationUnitAsync(int organizationId, int organizationUnitId);

    /// <summary>
    /// Створює нову посаду в організації.
    /// </summary>
    Task CreatePositionAsync(PositionCreateCommand command);

    /// <summary>
    /// Оновлює існуючу посаду в організації.
    /// </summary>
    Task<bool> UpdatePositionAsync(PositionUpdateCommand command);

    /// <summary>
    /// Видаляє посаду в організації за заданим ідентифікатором.
    /// </summary>
    Task<bool> DeletePositionAsync(int organizationId, int organizationUnitId, int positionId);

    Task<IEnumerable<OrganizationDto>> GetOrganizationsAsync();
    Task<IEnumerable<IdNamePair>> GetOrganizationsShortAsync();
    Task<OrganizationUnitDto> GetOrganizationUnitAsync(int organizationId, int id);
    Task<PositionDto> GetPositionAsync(int organizationId, int organizationUnitId, int id);
}
