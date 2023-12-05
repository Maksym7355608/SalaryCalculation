import IdentityApiClient from "./IdentityApiClient";
import {OrganizationApiClient} from "./OrganizationApiClient";

export class RestUnitOfWork {
    public identity: IdentityApiClient;
    public organization: OrganizationApiClient;

    constructor() {
        this.identity = new IdentityApiClient();
        this.organization = new OrganizationApiClient();
    }
}