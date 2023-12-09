import React from "react";
import {DataTable, DataTableBaseProps} from 'primereact/datatable';
import {Column} from 'primereact/column';
import "primereact/resources/themes/lara-light-indigo/theme.css";
import {InputText} from "primereact/inputtext";

interface DataTableProps<TModel extends object> {
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
    header?: JSX.Element;
}

export default function CustomDataTable<TModel extends object>(props: DataTableProps<TModel>) {
    let config = {
        emptyMessage: 'Дані відсутні',
        rowHover: true,
        scrollable: true,
        scrollHeight: '400px',
        sortMode: 'single',
        sortOrder: 1,
        showGridlines: true,
        rowsPerPageOptions: [5, 10, 25, 50],
        paginator: true,
        rows: 5,
        lazy: true,
        paginatorLeft: <></>,
        paginatorRight: <></>,
    } as DataTableBaseProps<any>;

    if(props.config)
        config = { ...props.config };

    const renderHeader = () => {
        if (props.header && !(props.header.enableHeader ?? true))
            return undefined;
        return (
            <div className="d-flex justify-content-between">
                <div>{props.header?.leftHead}</div>
                <div>{props.header?.centerHead}</div>
                <div>{(props.header?.rightHead ?? false) ?
                    props.header?.rightHead :
                    <span className="p-input-icon-left small">
                        <i className="material-icons small">search</i>
                        <InputText className="small" onChange={() => undefined} placeholder="Пошук"/>
                    </span>}
                </div>
            </div>
        );
    }

    const renderColumns = () => {
        return (
            <>
                {
                    props.columns.map(col =>
                        <Column key={col.field} field={col.field} header={col.text} sortable={col.sortable ?? true}
                                className={col.class} align={col.align} hidden={col.hidden}/>
                    )
                }
            </>
        )
    }

    return (
        <div className="ibox-content w-100">
            <div className="justify-content-center">
                <DataTable value={props.rows} header={renderHeader()}
                           className={config.className} size={"small"}
                           currentPageReportTemplate="{first} по {last} з {totalRecords}"
                           paginator={config.paginator} paginatorRight={config.paginatorRight}
                           paginatorLeft={config.paginatorLeft}
                           rowHover={config.rowHover} scrollable={config.scrollable}
                           scrollHeight={config.scrollHeight}
                           emptyMessage={config.emptyMessage} rows={config.rows}
                           rowsPerPageOptions={config.rowsPerPageOptions}
                           sortMode={config.sortMode}
                           showGridlines={config.showGridlines}
                           lazy={config.lazy} loading={config.loading} footer={props.footer}>
                    {renderColumns()}
                </DataTable>
            </div>
        </div>
    );
}