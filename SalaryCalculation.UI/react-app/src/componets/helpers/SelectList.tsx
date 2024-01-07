import React, {useState} from "react";
import { MultiSelect } from 'primereact/multiselect';
import {IdNamePair} from "../../models/BaseModels";

interface SelectListProps {
    id: string;
    items: IdNamePair[];
    setState: (set: number | number[] | undefined) => void;
    useEmpty?: boolean;
    emptyName?: string;
    value?: number | number[];
    multiple?: boolean;
    disabled?: boolean;
    className?: string;
}

function SelectList(props: SelectListProps) {
    const [selected, setSelected] = useState<number | number[] | undefined>();
    const handleSelect = (value: number) => {
        props.setState(value);
        setSelected(value);
    }

    const handleSelectMultiple = (value: number[]) => {
        props.setState(value);
        setSelected(value);
    }

    return props.multiple ?
        (
            <MultiSelect onChange={(e) => handleSelectMultiple(e.value)} id={props.id} display='chip'
                         options={props.items.map(i => {
                             return {label: i.name, value: i.id}
                         })} disabled={props.disabled} value={selected} className={`w-100 from-picker ${props.className}`} itemClassName='small'/>
        ) :
        (
            <select onChange={(e) => handleSelect(parseInt(e.target.value))} id={props.id} className={`form-select ${props.className}`} multiple={props.multiple}
                    disabled={props.disabled}>
                {props.useEmpty && <option value={-1}
                                           key={-1}
                                           selected={-1 === props.value}>{props.emptyName ? props.emptyName : "--- Оберіть опцію ---"}</option>}
                {props.items.map(item => {
                    return (
                        <option value={item.id} selected={item.id === props.value} key={item.id}>{item.name}</option>
                    );
                })}
            </select>
        );
}

export default SelectList;