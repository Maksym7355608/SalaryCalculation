import {useParams} from "react-router-dom";
import {useForm} from "react-hook-form";
import {RegimeModel} from "../../models/schedule";

export function Regime() {
    const { id } = useParams();
    const {register, handleSubmit} = useForm<RegimeModel>()
    return (
        <></>
    );
}