import {Component, ReactElement} from "react";
import {DataTable, DataTableBaseProps} from 'primereact/datatable';
import {Column} from 'primereact/column';
import "primereact/resources/themes/lara-light-indigo/theme.css";
import {Toolbar} from "primereact/toolbar";
import {InputText} from "primereact/inputtext";

interface DataTableProps<TModel> {
    columns: DataConfig[];
    rows: TModel[];
    footer?: any;
    config?: any;
    header?: HeaderSettings;
}

interface DataConfig {
    field: string;
    text: string;
    class?: string;
    sortable?: boolean;
    hidden?: boolean;
    align?: 'left' | 'right' | 'center';
}

interface HeaderSettings {
    enableHeader?: boolean;
    rightHead?: JSX.Element;
    centerHead?: JSX.Element;
    leftHead?: JSX.Element;
    header?: Element;
}

export class CustomDataTable<TModel extends object> extends Component<DataTableProps<TModel>> {

    private readonly config = {
        emptyMessage: 'Дані відсутні',
        rowHover: true,
        scrollable: true,
        scrollHeight: '400px',
        sortMode: 'single',
        sortOrder: 1,
        showGridlines: true,
        value: [],
        rowsPerPageOptions:[5, 10, 25, 50],
        paginator: true,
        rows: 5,
        lazy: true,
        paginatorLeft: <></>,
        paginatorRight: <></>,
        header: this.renderHeader(),
    } as DataTableBaseProps<any>;

    constructor(props : DataTableProps<TModel>) {
        super(props);
        if(props.config)
            this.config = $.extend(this.config, props.config)
    }

    private renderColumns() {
        return [
            this.props.columns.map(col =>
                <Column key={col.field} field={col.field} header={col.text} sortable={col.sortable ?? true}
                        className={col.class} align={col.align} hidden={col.hidden}/>
            )
        ]
    }

    private renderHeader() {
        if (this.props.header && !(this.props.header.enableHeader ?? true))
            return undefined;
        return (
            <div className="d-flex justify-content-between">
                <div>{this.props.header?.leftHead}</div>
                <div>{this.props.header?.centerHead}</div>
                <div>{(this.props.header?.rightHead ?? false) ?
                    this.props.header?.rightHead :
                    <span className="p-input-icon-left small">
                        <i className="material-icons small">search</i>
                        <InputText className="small" onChange={() => undefined} placeholder="Пошук"/>
                    </span>}
                </div>
            </div>
        );
    }

    render() {
        return (
            <div className="ibox-content w-100">
                <div className="justify-content-center">
                        <DataTable value={this.props.rows} header={this.renderHeader()}
                                   className={this.config.className} size={"small"}
                                   currentPageReportTemplate="{first} по {last} з {totalRecords}"
                                   paginator={this.config.paginator} paginatorRight={this.config.paginatorRight}
                                   paginatorLeft={this.config.paginatorLeft}
                                   rowHover={this.config.rowHover} scrollable={this.config.scrollable}
                                   scrollHeight={this.config.scrollHeight}
                                   emptyMessage={this.config.emptyMessage} rows={this.config.rows}
                                   rowsPerPageOptions={this.config.rowsPerPageOptions}
                                   sortMode={this.config.sortMode}
                                   showGridlines={this.config.showGridlines}
                                   lazy={this.config.lazy} loading={this.config.loading} footer={this.props.footer}>
                            {this.renderColumns()}
                        </DataTable>
                </div>
            </div>
        );
    }
}