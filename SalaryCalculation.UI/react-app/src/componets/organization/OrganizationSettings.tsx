import BasePageModel from "../BasePageModel";

export class OrganizationSettings extends BasePageModel<{organization: number}> {
    constructor(props: number) {
        super({organization: props});
    }
    render() {
        return (
            <></>
        );
    }
}