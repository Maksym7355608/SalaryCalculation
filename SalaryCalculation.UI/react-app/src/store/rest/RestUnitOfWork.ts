import IdentityApiClient from "./IdentityApiClient";
import OrganizationApiClient from "./OrganizationApiClient";
import ScheduleApiClient from "./ScheduleApiClient";

export default class RestUnitOfWork {
    public identity: IdentityApiClient;
    public organization: OrganizationApiClient;
    public schedule: ScheduleApiClient;

    constructor() {
        this.identity = new IdentityApiClient();
        this.organization = new OrganizationApiClient();
        this.schedule = new ScheduleApiClient();
    }
}