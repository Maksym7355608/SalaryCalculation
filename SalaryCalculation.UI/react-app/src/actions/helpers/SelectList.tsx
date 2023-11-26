import {Component} from "react";
import {IdNamePair} from "../../models/BaseModels";

interface SelectListProps {
    useEmpty?: boolean;
    emptyName?: string;
    selectName: string;
    items: IdNamePair[];
    value?: number;
}

export default class SelectList extends Component<SelectListProps> {
    constructor(props: SelectListProps) {
        super(props);
    }

    render() {
        return (
            <select id={this.props.selectName} className="form-control">
                {this.props.useEmpty && <option value={-1}
                                                key={-1} selected={-1 == this.props.value}>{this.props.emptyName ? this.props.emptyName : "--- Оберіть опцію ---"}</option>}
                {this.props.items.map(item => {
                    return (
                        <option value={item.id} selected={item.id == this.props.value} key={item.id}>{item.name}</option>
                    );
                })}
            </select>
        );
    }
}