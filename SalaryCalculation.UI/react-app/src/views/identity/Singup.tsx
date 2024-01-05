import {useEffect, useState} from "react";
import {useNavigate} from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import SelectList from "../../componets/helpers/SelectList";
import OrganizationApiClient from "../../store/rest/OrganizationApiClient";
import {SubmitHandler, useForm} from "react-hook-form";
import {SignUpForm, SignupState} from "../../models/identity/identity";
import { signUp } from "../../store/identity";

export default function SignUp() {
    const navigate = useNavigate();
    const [state, setState] = useState<SignupState>({
        loaded: false,
        organizations: []
    })
    const [selected, setSelected] = useState<number>(-1);
    const {
        register,
        handleSubmit,
        getValues,
        formState: { errors },
    } = useForm<SignUpForm>()

    const onSubmit: SubmitHandler<SignUpForm> = (data) => {
        data.organization = selected;
        signUp(data).then(response => {
            if(response)
                navigate('/login');
        });
    }

    useEffect(() => {
        if (state.loaded)
            return;
        const organizationRestClient = new OrganizationApiClient();
        organizationRestClient.getOrganizationsShortAsync().then(orgResponse => {
            setState({
                organizations: orgResponse,
                loaded: true
            });
            setSelected(orgResponse[0].id);
        });
    });

    return (
        <div className="screen-auth">
            <div className="container">
                <div className="text-end">укр/eng</div>
                <div className="text-auth-1 pt-3 pb-2">Реєстрація</div>
                <form className="form-auth p-3" onSubmit={handleSubmit(onSubmit)}>
                    <div className="form-group mb-1">
                        <label htmlFor="username" className="form-label text-auth-2">Логін</label>
                        <input {...register('username', {required: true})}
                               type="text" id="username" className="form-control" placeholder="Введіть логін"/>
                    </div>
                    <div className="form-group mb-1">
                        <label htmlFor="password" className="form-label text-auth-2">Пароль</label>
                        <input {...register('password', {required: true})}
                               type="password" id="password" className="form-control" placeholder="Введіть пароль"/>
                    </div>
                    <div className="form-group mb-1">
                        <label htmlFor="passwordConfirm" className="form-label text-auth-2">Підтвердити
                            пароль</label>
                        <input {...register('passwordConfirm', {required: true, validate: {
                                matchesPassword: value =>
                                    value === getValues('password') || 'Паролі мають співпадати'
                            }})}
                               type="password" id="passwordConfirm" className="form-control"
                               placeholder="Введіть пароль ще раз"/>
                    </div>
                    <div className="form-group mb-1">
                        <label htmlFor="firstName" className="form-label text-auth-2">Ім'я</label>
                        <input {...register('firstName', {required: true})}
                               type="text" id="firstName" className="form-control" placeholder="Введіть ім'я"/>
                    </div>
                    <div className="form-group mb-1">
                        <label htmlFor="middleName" className="form-label text-auth-2">По-батькові</label>
                        <input {...register('middleName')}
                               type="text" id="middleName" className="form-control"
                               placeholder="Введіть по-батькові"/>
                    </div>
                    <div className="form-group mb-1">
                        <label htmlFor="lastName" className="form-label text-auth-2">Прізвище</label>
                        <input {...register('lastName', {required: true})}
                               type="text" id="lastName" className="form-control" placeholder="Введіть прізвище"/>
                    </div>
                    <div className="form-group mb-1">
                        <label htmlFor="phone" className="form-label text-auth-2">Телефон</label>
                        <input {...register('phone')} type="tel" id="phone" className="form-control" placeholder="Введіть номер телефону"/>
                    </div>
                    <div className="form-group mb-1">
                        <label htmlFor="email" className="form-label text-auth-2">Електронна адреса</label>
                        <input {...register('email', {required: true})} type="email" id="email" className="form-control"
                               placeholder="Введіть електронну адреса"/>
                    </div>
                    <div className="form-group mb-1">
                        <label htmlFor="organization" className="form-label text-auth-2">Організація</label>
                        <SelectList setState={(state) => setSelected(state as number)}
                                    useEmpty={false} emptyName={undefined} id="organization"
                                    items={state.organizations}/>
                    </div>
                    <div className="btn-group w-100 mt-2 d-flex justify-content-center">
                        <div className="div-btn-login">
                            <button className="btn btn-primary">
                                Реєстрація
                            </button>
                        </div>
                    </div>

                    <span id="requestInvalid" className="text-danger"></span>
                    <span id="responseInvalid" className="text-danger"></span>
                </form>
            </div>
        </div>
    );
}