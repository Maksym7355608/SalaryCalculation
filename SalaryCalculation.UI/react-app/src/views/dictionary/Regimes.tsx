import React, {useEffect, useState} from "react";
import {RegimeModel} from "../../models/schedule";
import {Button, Container} from "react-bootstrap";
import CustomDataTable from "../../componets/helpers/CustomDataTable";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {toShortDateString, user} from "../../store/actions";
import {Icon} from "../../componets/helpers/Icon";

export function Regimes() {
    const restClient = new RestUnitOfWork();
    const [regimes, setRegimes] = useState<RegimeModel[]>([]);

    useEffect(() => {
        restClient.schedule.getRegimesAsync(user().organization).then(r => {
            setRegimes(r);
        });
    }, []);

    const getTableRows = () => {
        return regimes.map(r => {
            let row: any = {...r};
            row["isCircle"] = r.isCircle ? "Так" : "Ні";
            row["startDateInCurrentYear"] = toShortDateString(r.startDateInCurrentYear);
            row["startDateInPreviousYear"] = toShortDateString(r.startDateInPreviousYear);
            row["startDateInNextYear"] = toShortDateString(r.startDateInNextYear);
            row["workDaysCount"] = r.workDays.map(x => x.daysOfWeek.trim().split(',').length).reduce((a, b) => a + b);
            row["restDaysCount"] = r.restDays.trim().split(',').length;
            row["actions"] = (<>
                <Button type='button' variant='light' size='sm'><Icon name='edit_square' small/></Button>
                <Button type='button' variant='light' size='sm'><Icon name='close' small/></Button>
            </>);
            return row;
        })
    }

    const columnDefs = [
        {field: 'code', text: 'Код'},
        {field: 'name', text: 'Назва'},
        {field: 'isCircle', text: 'Циклічність'},
        {field: 'workDaysCount', text: 'Робочих днів'},
        {field: 'restDaysCount', text: 'Вихідних днів'},
        {field: 'shiftsCount', text: 'Кількість змін'},
        {field: 'startDateInCurrentYear', text: 'Початок у поточному році'},
        {field: 'startDateInPreviousYear', text: 'Початок у попередньому році'},
        {field: 'startDateInNextYear', text: 'Початок у наступному році'},
        {field: 'actions', text: '', sortable: false},
    ]
    return (
        <Container fluid>
            <CustomDataTable columns={columnDefs} rows={getTableRows()}/>
        </Container>
    );
}