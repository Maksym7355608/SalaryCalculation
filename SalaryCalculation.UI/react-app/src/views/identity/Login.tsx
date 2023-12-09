import {Link} from "react-router-dom";
import { useForm, SubmitHandler } from "react-hook-form"
import {useDispatch} from "react-redux";
import {get_user} from "../../store/actions/userActions";
import {LoginForm} from "../../models/identity/identity";
import {logIn} from "../../store/identity";
import {login} from "../../auth";

export default function Login() {
    const dispatch = useDispatch();
    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<LoginForm>()

    const onSubmit: SubmitHandler<LoginForm> = (data) => logIn(data).then(res => {
        const userModel = res;
        dispatch(get_user(userModel))
        localStorage.setItem("user", JSON.stringify(userModel));
        login(userModel.token);
    });

    return (
        <div className="screen-auth">
            <div className="container">
                <div className="text-end">укр/eng</div>
                <div className="text-auth-1 pt-5 pb-2">Вхід у систему</div>
                <form className="form-auth p-5" onSubmit={handleSubmit(onSubmit)}>
                    <div className="form-group mb-2">
                        <label htmlFor="username" className="form-label text-auth-2">Логін</label>
                        <input {...register("username", {required: true})}
                               type="text" id="username" className="form-control" placeholder="Введіть логін"/>
                        {errors.username && <span>{errors.username.message}</span>}
                    </div>
                    <div className="form-group mb-2">
                        <label htmlFor="password" className="form-label text-auth-2">Пароль</label>
                        <input {...register("password", { required: true})}
                               type="password" id="password" className="form-control" placeholder="Введіть пароль"/>
                        {errors.password && <span>{errors.password.message}</span>}
                    </div>
                    <div className="btn-group w-100 mt-3 d-flex justify-content-between">
                        <div className="div-btn-login">
                            <button type="submit" className="btn btn-primary">
                                Вхід
                            </button>
                        </div>
                        <div className="div-link-signup">
                            <Link to="/signup" className="link-signup">Реєстрація</Link>
                        </div>
                        <span id="requestInvalid" className="text-danger"></span>
                        <span id="responseInvalid" className="text-danger"></span>
                    </div>
                </form>
            </div>
        </div>
    );
}