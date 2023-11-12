import {IdNamePair} from "../../componets/identity/Singup";
import {Component} from "react";

interface SelectListProps {
    useEmpty: boolean;
    emptyName: string | undefined;
    selectName: string;
    items: IdNamePair[];
}
export default class SelectList extends Component<SelectListProps> {
    constructor(props: SelectListProps) {
        super(props);
    }

    render() {
        return (
            <select id={this.props.selectName} className="form-control">
                {this.props.useEmpty && <option value={-1} key={-1}>{this.props.emptyName ? this.props.emptyName : "--- Оберіть опцію ---"}</option> }
                {this.props.items.map(item => {
                    return (
                        <option value={item.id} key={item.id}>{item.name}</option>
                    );
                })}
            </select>
        );
    }
}