import React from "react";
import {IdNamePair} from "../../models/BaseModels";
import {useForm} from "react-hook-form";

interface SelectListProps {
    id: string;
    items: IdNamePair[];
    register: string;
    useEmpty?: boolean;
    emptyName?: string;
    value?: number;
    multiple?: boolean;
    disabled?: boolean;
}

const SelectList : React.FC<SelectListProps> = (props) => {
    const {register} = useForm()

    return (
        <select {...register(props.register)} id={props.id} className="form-control" multiple={props.multiple} disabled={props.disabled}>
            {props.useEmpty && <option value={-1}
                                            key={-1} selected={-1 === props.value}>{props.emptyName ? props.emptyName : "--- Оберіть опцію ---"}</option>}
            {props.items.map(item => {
                return (
                    <option value={item.id} selected={item.id === props.value} key={item.id}>{item.name}</option>
                );
            })}
        </select>
    );
}

export default SelectList;