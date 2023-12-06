import {Component} from "react";

export class NotFound extends Component {
    render() {
        return (
            <div className="text-danger">
                <h1>404 - Сторінку не знайдено</h1>
                <p>Вибачте, але сторінку, яку ви шукали, не існує.</p>
            </div>
        );
    }
}