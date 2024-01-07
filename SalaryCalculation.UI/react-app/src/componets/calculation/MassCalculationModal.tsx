import React, {useEffect, useState} from "react";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {IdNamePair} from "../../models/BaseModels";
import {handleError, toPeriod, user} from "../../store/actions";
import SelectList from "../helpers/SelectList";
import {CustomModalDialog} from "../helpers/CustomModalDialog";

const MassCalculationModal:React.FC<{ show: boolean, setShow: (value: boolean) => void }> = ({show, setShow}) => {
    const restClient = new RestUnitOfWork();
    const [units, setUnits] = useState<IdNamePair[]>([]);
    const [positions, setPositions] = useState<IdNamePair[]>([]);
    const [regimes, setRegimes] = useState<IdNamePair[]>([]);

    useEffect(() => {
        restClient.organization.getPositionsShortAsync(user().organization).then(res => {
            setPositions(res);
        });
        restClient.organization.getOrganizationUnitsShortAsync(user().organization).then(res =>{
            setUnits(res);
        });
        restClient.schedule.getRegimesShortAsync(user().organization).then(r => {
            setRegimes(r);
        })
    }, []);

    const [sPeriod, setPeriod] = useState<number>(toPeriod('2023-12'));
    const [sUnit, setUnit] = useState<number>();
    const [sPosition, setPosition] = useState<number>();
    const [sRegime, setRegime] = useState<number>();

    const handleSubmit = () => {
        if(sPeriod < 200001 || sPeriod > 209912){
            handleError('period', 'Некоректно введений період')
            return;
        }
        const cmd = {
            organizationId: user().organization,
            period: sPeriod,
            organizationUnitId: sUnit,
            positionId: sPosition,
            regimeId: sRegime
        };
        restClient.calculation.massCalculateEmployeeAsync(cmd);
    }

    const body = [
        {id: 'period', label: 'Період', control: <input type='month' min='2000-01' max='2099-12' className='form-control w-100' onChange={(event) => setPeriod(toPeriod(event.target.value))}/>},
        {id: 'unit', label: 'Підрозділи', control: <SelectList id='unit' items={units} setState={(state) => setUnit(state as number)} useEmpty emptyName='Оберіть підрозділ'/>},
        {id: 'position', label: 'Підрозділи', control: <SelectList id='position' items={positions} setState={(state) => setPosition(state as number)} useEmpty emptyName='Оберіть посаду'/>},
        {id: 'regime', label: 'Підрозділи', control: <SelectList id='regime' items={regimes} setState={(state) => setRegime(state as number)} useEmpty emptyName='Оберіть режим'/>}
    ];

    return (<CustomModalDialog id='mass-calculation-modal' show={show} headerText='Розрахунок зарплат'
    body={body} handleChangeShow={(show) => setShow(show)} handleActionBtn={() => handleSubmit()}
    footer={{actionBtnStyle: 'success', actionBtnText: 'Розрахувати'}}/>);
}

export default MassCalculationModal;