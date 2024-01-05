export function Icon({name, small}:{name: string, small?: boolean}) {
    return (
        <i className={`material-icons ${(small ?? false) && "small"}`}>{name}</i>
    );
}