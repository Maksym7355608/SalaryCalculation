import React, {useEffect, useState} from "react";
import SelectList from "../../componets/helpers/SelectList";
import {EPermission, IdNamePair} from "../../models/BaseModels";
import {EmployeeShortModel} from "../../models/ShortModels";
import {RestUnitOfWork} from "../../store/rest/RestUnitOfWork";
import {hasPermission, user} from "../../store/actions";
import {SubmitHandler, useForm} from "react-hook-form";
import {IHomeForm} from "../../models/employees/forms";
import {mapToEmployeeShortModel, searchEmployees} from "../../store/employees";
import {DataTable} from "primereact/datatable";
import {Column} from "primereact/column";

export default function Home() {
    const [isLoaded, setLoaded] = useState<boolean>(false);
    const [organizationUnits, setUnits] = useState<IdNamePair[]>([]);
    const [positions, setPositions] = useState<IdNamePair[]>([]);
    const [employees, setEmployees] = useState<EmployeeShortModel[]>([]);
    const restClient = new RestUnitOfWork();

    useEffect(() => {
        if (!isLoaded) {
            restClient.organization.getPositionsShortAsync(user.organization).then(res => {
                setPositions(res);
            });
            restClient.organization.getOrganizationUnitsShortAsync(user.organization).then(res =>{
                setUnits(res);
            });
            setLoaded(true);
        }
    });
    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<IHomeForm>()

    const onSubmit: SubmitHandler<IHomeForm> = (data) => searchEmployees(data).then(res => {
        setEmployees(mapToEmployeeShortModel(res))
    });

    const createEmployeeActions = (employee: EmployeeShortModel) => {
        return (
            <></>
        );
    }

    const createEmployeeWithActions = () => {
        return employees.map(employee => {
            return {
                ...employee,
                actions: createEmployeeActions(employee)
            };
        });
    }

    return (
        <div className="container-fluid">
            <form className="form-search" onSubmit={handleSubmit(onSubmit)}>
                <div className="row w-100 ps-4">
                    <div className="col-4">
                        <label className="form-label" htmlFor="roll-number">
                            Табельний номер
                        </label>
                        <input {...register('rollNumber', {pattern: /^[0-9]+$/i })}
                               type="text" id="roll-number" className="form-control" placeholder="Введіть табельний номер"/>
                        <span id="roll-number-validation" className="text-danger"></span>
                    </div>
                    <div className="col-4">
                        <label className="form-label" htmlFor="position">
                            Посада
                        </label>
                        <SelectList {...register('position')} useEmpty={true} emptyName={"Оберіть посаду"} selectName={"position"} items={positions}/>
                    </div>
                    <div className="col-4">
                        <label className="form-label" htmlFor="salary-from">
                            Оклад з
                        </label>
                        <input {...register('salaryFrom', {min: 0, pattern: /^[0-9]+$/i})} type="text" id="salary-from" className="form-control" placeholder="Введіть оклад з"/>
                    </div>
                </div>
                <div className="row w-100 mt-1 ps-4">
                    <div className="col-4">
                        <label className="form-label" htmlFor="organization-unit">
                            Підрозділ
                        </label>
                        <SelectList {...register('organizationUnit')} useEmpty={true} emptyName={"Оберіть підрозділ"} selectName={"organization-unit"} items={organizationUnits}/>
                    </div>
                    <div className="col-4">
                        <label className="form-label" htmlFor="employment-date">
                            Дата прийняття
                        </label>
                        <input {...register('date')} type="date" id="employment-date" className="form-control" placeholder="Введіть табельний номер"/>
                    </div>
                    <div className="col-4">
                        <label className="form-label" htmlFor="salary-to">
                            Оклад по
                        </label>
                        <input {...register('salaryTo', {min: 0, pattern: /^[0-9]+$/i})} type="text" id="salary-to" className="form-control" placeholder="Введіть оклад по"/>
                    </div>
                </div>

                <div className="row-gap-sm-0 mt-3 mb-4 ps-4">
                    <button type="submit" className="col btn btn-sm btn-primary me-2" title="Пошук працівників">
                        <i className="material-icons small">search</i> Пошук
                    </button>
                    {
                        hasPermission(EPermission.createEmployees) &&
                        <button type="button" className="col btn btn-sm btn-success me-2" title="Додати працівника">
                            <i className="material-icons small">add</i> Додати
                        </button>
                    }
                    <button type="reset" className="col btn btn-sm btn-warning me-2" title="Очистити фільтри">
                        Очистити
                    </button>
                </div>
            </form>
            <div className="ibox mt-3 mb-3">
                <div className="ibox-content w-100">
                    <div className="justify-content-center">
                        <DataTable value={createEmployeeWithActions()} lazy paginator rows={10}
                                   rowsPerPageOptions={[5, 10, 20, 50]} size='small' scrollable
                                   rowHover>
                            <Column field='rollNumber' sortable header="Табельний номер"/>
                            <Column field='fullName' sortable header="ПІБ"/>
                            <Column field='unit' sortable header="Підрозділ"/>
                            <Column field='position' sortable header="Посада"/>
                            <Column field='employeeDate' sortable header="Дата прийняттяр"/>
                            <Column field='dismissDate' sortable header="Дата звільнення"/>
                            <Column field='salary' sortable header="Оклад"/>
                            <Column field='sex' sortable header="Стать"/>
                            <Column field='familyStatus' sortable header="Сімейний стан"/>
                            <Column field='benefits' sortable header="Пільги"/>
                            <Column field='actions' sortable header=""/>
                        </DataTable>
                    </div>
                </div>
            </div>
        </div>
    );
}