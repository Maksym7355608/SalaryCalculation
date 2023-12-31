﻿using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Data.Enums;

namespace Organization.App.DtoModels;

public class OrganizationDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public long Edrpou { get; set; }
    public string Address { get; set; }
    public string FactAddress { get; set; }
    public IEnumerable<BankDto> BankAccounts { get; set; }

    public string Chief { get; set; }
    public string Accountant { get; set; }

    public IdNamePair? Manager { get; set; }

    public IEnumerable<EPermission> Permissions { get; set; }
}

public class OrganizationUnitDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int OrganizationId { get; set; }
    public int? OrganizationUnitId { get; set; }
}

public class PositionDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int OrganizationId { get; set; }
    public int OrganizationUnitId { get; set; }
}