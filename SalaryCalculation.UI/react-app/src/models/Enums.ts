export enum EPermission {
    roleControl = 1,
    organizationSettings = 2,
    searchEmployees = 3,
    createEmployees = 4,
    deleteEmployees = 5,
    viewSchedule = 6,
    searchSchedules = 7,
    calculateSchedules = 8,
    viewCalculation = 9,
    calculationSalaries = 10,
    viewDictionary = 11,
    createDocuments = 12,
}

export enum EBenefit{
    none = 0,
    kids = 1,
    widow = 2,
    kidsWithDisability = 3,
    chernobyl = 4,
    student = 5,
    firstDisability = 6,
    secondDisability = 7,
    assignedLifetimeScholarship = 8,
    militaryAfterWWII = 9,
    heroTitul = 10,
    fourMedalForCourage = 11,
    militaryInWWII = 12,
    prisonersOfConcentrationCamps = 13,
    personRecognizedAsRepressedOrRehabilitated = 14,
}
export enum ESex{
    male = 1,
    female = 2
}
export enum EMarriedStatus{
    unmarried = 1,
    married = 2,
    civilMarried = 3,
    divorced = 4
}

export enum EContactKind {
    phone = 1,
    email,
    telegram
}