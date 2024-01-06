export interface OperationData {
    id: string;
    code: number;
    name: string;
    sign: number;
    organizationId: number;
    description: string;
}

export interface BaseAmount {
    id: string
    name: string;
    expressionName: string;
    value: number;
    note: string;
    dateFrom: Date;
    dateTo?: Date;
}

export interface Formula {
    id: string;
    organizationId: number;
    name: string;
    condition: string;
    expressionName: string;
    expression: string;
    dateFrom: Date;
    dateTo?: Date;
    code: number;
}