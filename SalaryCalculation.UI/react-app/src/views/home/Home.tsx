import React, {useEffect, useState} from "react";
import SelectList from "../../componets/helpers/SelectList";
import {IdNamePair} from "../../models/BaseModels";
import {EmployeeShortModel} from "../../models/ShortModels";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {hasPermission, user} from "../../store/actions";
import {SubmitHandler, useForm} from "react-hook-form";
import {IHomeForm} from "../../models/employees/forms";
import {mapToEmployeeShortModel, searchEmployees} from "../../store/employees";
import {EPermission} from "../../models/Enums";
import CustomDataTable from "../../componets/helpers/CustomDataTable";
import { Link } from "react-router-dom";
import DeleteEmployeeModal from "../../componets/employee/DeleteEmployeeModal";

export default function Home() {
    const [isLoaded, setLoaded] = useState<boolean>(false);
    const [organizationUnits, setUnits] = useState<IdNamePair[]>([]);
    const [selectedUnit, setUnit] = useState<number>();
    const [positions, setPositions] = useState<IdNamePair[]>([]);
    const [selectedPos, setPos] = useState<number>();
    const [employees, setEmployees] = useState<EmployeeShortModel[]>([]);
    const [showDelete, setShowDelete] = useState(false);
    const [deleteId, setDeleteId] = useState<number | undefined>();

    const restClient = new RestUnitOfWork();

    useEffect(() => {
        if (!isLoaded) {
            restClient.organization.getPositionsShortAsync(user().organization).then(res => {
                setPositions(res);
            });
            restClient.organization.getOrganizationUnitsShortAsync(user().organization).then(res =>{
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

    const onSubmit: SubmitHandler<IHomeForm> = (data) => {
        data.organizationUnit = selectedUnit != undefined && selectedUnit != -1 ? selectedUnit : undefined;
        data.position = selectedPos != undefined && selectedPos != -1 ? selectedPos : undefined;
        searchEmployees(data).then(res => {
            setEmployees(mapToEmployeeShortModel(res))
        });
    }

    const createEmployeeActions = (employee: EmployeeShortModel) => {
        return (
            <div className='inline-flex'>
                <Link to={`/employee/${employee.id}`} className='btn btn-sm btn-light'><i className='material-icons small'>info</i></Link>
                <Link to={`/employee/${employee.id}?handler=edit`} className='btn btn-sm btn-light'><i className='material-icons small'>edit_square</i></Link>
                <button onClick={() => {
                    setShowDelete(true);
                    setDeleteId(employee.id);
                }} className='btn btn-sm btn-light'><i className='material-icons small'>close</i></button>
            </div>
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

    const columnDefs = [
        {field: 'rollNumber', text: 'Табельний номер'},
        {field: 'fullName', text: 'ПІБ'},
        {field: 'unit', text: 'Підрозділ'},
        {field: 'position', text: 'Посада'},
        {field: 'employeeDate', text: 'Дата прийняття'},
        {field: 'dismissDate', text: 'Дата звільнення'},
        {field: 'salary', text: 'Оклад'},
        {field: 'sex', text: 'Стать'},
        {field: 'familyStatus', text: 'Сімейний стан'},
        {field: 'benefits', text: 'Пільги'},
        {field: 'actions', text: '', sortable: false},
    ];
    return (
        <div className="container-fluid">
            <form className="form-search pt-1 pb-2 pe-2" onSubmit={handleSubmit(onSubmit)}>
                <div className="row w-100 ps-4">
                    <div className="col-4">
                        <label className="form-label" htmlFor="roll-number">
                            Табельний номер
                        </label>
                        <input {...register('rollNumber', {pattern: /^[0-9]+$/i })}
                               type="text" id="roll-number" className="form-control" placeholder="Введіть табельний номер"/>
                        <span id="roll-number-validation" className="text-danger"></span>
                        {errors.rollNumber && <span className='text-danger'>невірно введені дані</span>}
                    </div>
                    <div className="col-4">
                        <label className="form-label" htmlFor="position">
                            Посада
                        </label>
                        <SelectList setState={(state) => setPos(state as number)} useEmpty={true} emptyName={"Оберіть посаду"} id={"position"} items={positions}/>
                    </div>
                    <div className="col-4">
                        <label className="form-label" htmlFor="salary-from">
                            Оклад з
                        </label>
                        <input {...register('salaryFrom', {min: 0, pattern: /^[0-9]+$/i})} type="text" id="salary-from" className="form-control" placeholder="Введіть оклад з"/>
                        {errors.salaryFrom && <span className='text-danger'>невірно введені дані</span>}
                    </div>
                </div>
                <div className="row w-100 mt-1 ps-4">
                    <div className="col-4">
                        <label className="form-label" htmlFor="organization-unit">
                            Підрозділ
                        </label>
                        <SelectList setState={(state) => setUnit(state as number)} useEmpty={true} emptyName={"Оберіть підрозділ"} id={"organization-unit"} items={organizationUnits}/>
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
                        {errors.salaryTo && <span className='text-danger'>невірно введені дані</span>}
                    </div>
                </div>

                <div className="row-gap-sm-0 mt-3 ps-4">
                    <button type="submit" className="col btn btn-sm btn-primary me-2" title="Пошук працівників">
                        <i className="material-icons small">search</i> Пошук
                    </button>
                    {
                        hasPermission(EPermission.createEmployees) &&
                        <Link to='/employee/create' className="col btn btn-sm btn-success me-2" title="Додати працівника">
                            <i className="material-icons small">add</i> Додати
                        </Link>
                    }
                    <button type="reset" className="col btn btn-sm btn-warning me-2" title="Очистити фільтри">
                        Очистити
                    </button>
                </div>
            </form>
            <div className="mt-3 mb-3">
                <CustomDataTable columns={columnDefs} rows={createEmployeeWithActions()}/>
            </div>
            <DeleteEmployeeModal deleteId={deleteId} show={showDelete} setShow={setShowDelete}/>
        </div>
    );
}