import BasePageModel from "../BasePageModel";
import {FormEvent} from "react";
import SelectList from "../../actions/helpers/SelectList";
import {EPermission, IdNamePair} from "../../models/BaseModels";
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell, {tableCellClasses} from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import {EmployeeShortModel} from "../../models/ShortModels";
import {styled} from "@mui/material";
import {EmployeeSearchCommand} from "../../actions/commands/OrganizationCommands";
import $ from 'jquery';
import {EmployeeDto} from "../../models/DTO";

interface HomeState {
    isLoaded: boolean;
    organizationUnits : IdNamePair[];
    positions : IdNamePair[];
    employees: EmployeeShortModel[];
}

class Home extends BasePageModel<any, HomeState> {

    constructor() {
        super(undefined);
        this.state = {
            isLoaded: false,
            organizationUnits: [],
            positions: [],
            employees: []
        }
    }

    async componentDidMount() {
        document.title = "Пошук працівників";
        if(this.state.isLoaded)
            return;
        const positionsResponse = await this.restClient.organization.getPositionsShortAsync(this.user.organization);
        const ogrUnitsResponse = await this.restClient.organization.getOrganizationUnitsShortAsync(this.user.organization);
        this.setState({
            isLoaded: true,
            positions: positionsResponse,
            organizationUnits: ogrUnitsResponse
        }, this.renderTable);
    }

    private mapToEmployeeShortModel(employees: EmployeeDto[]): EmployeeShortModel[] {
        return employees.map(employee => {
            let contacts : string = "";
            employee.contacts.map(c => {
                contacts+=c.value + ", "
            });
            return {
                id: employee.id,
                rollNumber: employee.rollNumber.toString(),
                unit: employee.organizationUnitId.name,
                position: employee.position.name,
                employeeDate: employee.dateFrom.toDateString(),
                dismissDate: employee.dateTo?.toDateString() ?? "",
                salary: employee.salaries[employee.salaries.length-1].amount,
                sex: employee.sex.toString(),
                familyStatus: employee.marriedStatus.toString(),
                kids: undefined,
                contacts: contacts
            } as EmployeeShortModel;
        });
    }

    searchEmployeesAsync(e: FormEvent<HTMLFormElement>) {
        e.preventDefault();
        this.clearErrors();
        const rollNumberVal = $('#roll-number').val() as string;
        if(rollNumberVal && isNaN(Number(rollNumberVal)))
        {
            this.handleError("roll-number", "Невірний формат Табельного номера");
            return;
        }
        const rollNumber = rollNumberVal ? parseInt(rollNumberVal) : null;
        const unit = $('#organization-unit').val() as number;
        const pos = $('#position').val() as number;
        const dF = $('#employment-date').val() as string;
        const sF = $('#salary-from').val() as number;
        const sT = $('#salary-to').val() as number;
        const command: EmployeeSearchCommand = {
            organizationId: this.user.organization,
            rollNumber: rollNumber,
            organizationUnitId: unit != -1 ? unit : null,
            positionId: pos != -1 ? pos : null,
            dateFrom: dF ? new Date(Date.parse(dF)) : null,
            dateTo: new Date(Date.now()),
            salaryFrom: sF ? sF : null,
            salaryTo: sT ? sT : null,
        };
        this.restClient.organization.searchEmployeesAsync(command)
            .then(response => {
                this.setState({
                    employees: this.mapToEmployeeShortModel(response)
                });
            });
    }

    renderTable() {
        const StyledTableCell = styled(TableCell)(({ theme }) => ({
            [`&.${tableCellClasses.head}`]: {
                backgroundColor: theme.palette.common.black,
                color: theme.palette.common.white,
            },
            [`&.${tableCellClasses.body}`]: {
                fontSize: 14,
            },
        }));

        const StyledTableRow = styled(TableRow)(({ theme }) => ({
            '&:nth-of-type(odd)': {
                backgroundColor: theme.palette.action.hover,
            },
            // hide last border
            '&:last-child td, &:last-child th': {
                border: 0,
            },
        }));


        return (
            <TableContainer component={Paper}>
                <Table sx={{ minWidth: 650 }} aria-label="simple table">
                    <TableHead>
                        <TableRow>
                            <TableCell>Табельний номер</TableCell>
                            <TableCell align="right">Працівник</TableCell>
                            <TableCell align="right">Підрозділ</TableCell>
                            <TableCell align="right">Посада</TableCell>
                            <TableCell align="right">Дата прийняття</TableCell>
                            <TableCell align="right">Дата звільнення</TableCell>
                            <TableCell align="right">Оклад</TableCell>
                            <TableCell align="right">Стать</TableCell>
                            <TableCell align="right">Сімейний стан</TableCell>
                            <TableCell align="right">Діти</TableCell>
                            <TableCell align="right">Контакти</TableCell>
                            <TableCell></TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {this.state.employees.map((row) => (
                            <TableRow
                                key={row.id}
                                aria-label="customized table"
                            >
                                <TableCell component="th" scope="row">
                                    {row.rollNumber}
                                </TableCell>
                                <TableCell align="right">{row.fullName}</TableCell>
                                <TableCell align="right">{row.unit}</TableCell>
                                <TableCell align="right">{row.position}</TableCell>
                                <TableCell align="right">{row.employeeDate}</TableCell>
                                <TableCell align="right">{row.dismissDate}</TableCell>
                                <TableCell align="right">{row.salary}</TableCell>
                                <TableCell align="right">{row.sex}</TableCell>
                                <TableCell align="right">{row.familyStatus}</TableCell>
                                <TableCell align="right">{row.kids}</TableCell>
                                <TableCell align="right">{row.contacts}</TableCell>
                                <TableCell align="right">{this.createActions(row)}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        );
    }

    render() {
        return (
            <div className="container-fluid">
                <form className="form-search" onSubmit={(event) => this.searchEmployeesAsync(event)}>
                    <div className="row w-100 ps-4">
                        <div className="col-4">
                            <label className="form-label" htmlFor="roll-number">
                                Табельний номер
                            </label>
                            <input type="text" id="roll-number" className="form-control" placeholder="Введіть табельний номер"/>
                            <span id="roll-number-validation" className="text-danger"></span>
                        </div>
                        <div className="col-4">
                            <label className="form-label" htmlFor="position">
                                Посада
                            </label>
                            <SelectList useEmpty={true} emptyName={"Оберіть посаду"} selectName={"position"} items={this.state.positions}/>
                        </div>
                        <div className="col-4">
                            <label className="form-label" htmlFor="salary-from">
                                Оклад з
                            </label>
                            <input type="number" id="salary-from" className="form-control" min={0} placeholder="Введіть оклад з"/>
                        </div>
                    </div>
                    <div className="row w-100 mt-1 ps-4">
                        <div className="col-4">
                            <label className="form-label" htmlFor="organization-unit">
                                Підрозділ
                            </label>
                            <SelectList useEmpty={true} emptyName={"Оберіть підрозділ"} selectName={"organization-unit"} items={this.state.organizationUnits}/>
                        </div>
                        <div className="col-4">
                            <label className="form-label" htmlFor="employment-date">
                                Дата прийняття
                            </label>
                            <input type="date" id="employment-date" className="form-control" placeholder="Введіть табельний номер"/>
                        </div>
                        <div className="col-4">
                            <label className="form-label" htmlFor="salary-to">
                                Оклад по
                            </label>
                            <input type="number" id="salary-to" className="form-control" min={0} placeholder="Введіть оклад по"/>
                        </div>
                    </div>

                    <div className="row-gap-sm-0 mt-3 mb-4 ps-4">
                        <button type="submit" className="col btn btn-sm btn-primary me-2" title="Пошук працівників">
                            <i className="material-icons small">search</i> Пошук
                        </button>
                        {this.hasPermission(EPermission.createEmployees) &&
                            <button type="button" className="col btn btn-sm btn-success me-2" title="Додати працівника">
                                <i className="material-icons small">add</i> Додати
                            </button>}
                        <button type="reset" className="col btn btn-sm btn-warning me-2" title="Очистити фільтри">
                            Очистити
                        </button>
                    </div>
                </form>

                {this.renderTable()}
            </div>
        );
    }

    private createActions(row: EmployeeShortModel) {
        return undefined;
    }
}

export default Home;