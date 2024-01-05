import {CustomModalDialog} from "../helpers/CustomModalDialog";
import React, {useEffect, useState} from "react";
import {SubmitHandler, useForm} from "react-hook-form";
import {user} from "../../store/actions";
import SelectList from "../helpers/SelectList";
import {IdNamePair} from "../../models/BaseModels";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";

export function AutoScheduleModal({show, setShow}: {show: boolean, setShow: (show: boolean) => void}) {
    const [units, setUnits] = useState<IdNamePair[]>([])
    const [selectedUnit, setUnit] = useState<number>();
    const [positions, setPositions] = useState<IdNamePair[]>([])
    const [selectedPos, setPos] = useState<number>();
    const [regimes, setRegimes] = useState<IdNamePair[]>([])
    const [selectedReg, setReg] = useState<number>();
    const {register, handleSubmit} = useForm({
        defaultValues: {
            dateFrom: new Date(Date.now()),
            dateTo: new Date(Date.now()),
            organizationId: user().organization,
        },
    });
    const submitHandler: SubmitHandler<any> = (data: any) => handleCalculate(data);

    const restClient = new RestUnitOfWork();
    useEffect(() => {
        restClient.organization.getPositionsShortAsync(user().organization).then(res => {
            setPositions(res);
        });
        restClient.organization.getOrganizationUnitsShortAsync(user().organization).then(res =>{
            setUnits(res);
        });
        restClient.schedule.getRegimesShortAsync(user().organization).then(res => {
            setRegimes(res);
        })
    }, [])
    const handleCalculate = (data: any) => {
        data['positionId'] = selectedPos != -1 ? selectedPos : undefined;
        data['organizationUnitId'] = selectedUnit != -1 ? selectedUnit : undefined;
        data['regimeId'] = selectedReg != -1 ? selectedReg : undefined;
        restClient.schedule.setWorkDaysByRegimeAsync(data).then(res => {
            if(res)
            {
                console.log('success setting');
                setShow(false);
            }
            else
                console.error('error setting');
        })
    }
    return (
        <CustomModalDialog id='auto-modal' show={show} handleChangeShow={(show) => setShow(show)} headerText='Автоматичне табелювання'
                           body={[
                               {id: 'organizationId', control: (<input type='hidden' {...register('organizationId')} value={user().organization}/>)},
                               {id: 'date-from', label: 'Дата з', control: (<input type='date' className='form-control' {...register('dateFrom')}/>)},
                               {id: 'date-to', label: 'Дата з', control: (<input type='date' className='form-control' {...register('dateTo')}/>)},
                               {id: 'organization-unit', label: 'Підрозділ', control: (<SelectList id='organization-units' items={units} setState={(state) => setUnit(state as number)} useEmpty emptyName='Оберіть підрозділ'/>)},
                               {id: 'position', label: 'Посада', control: (<SelectList id='positions' items={positions} setState={(state) => setPos(state as number)} useEmpty emptyName='Оберіть посаду'/>)},
                               {id: 'regime', label: 'Режим', control: (<SelectList id='regimes' items={regimes} setState={(state) => setReg(state as number)} useEmpty emptyName='Оберіть режим'/>)},
                           ]} handleActionBtn={handleSubmit(submitHandler)}/>
    );
}