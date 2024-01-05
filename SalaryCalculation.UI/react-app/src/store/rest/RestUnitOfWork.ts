import IdentityApiClient from "./IdentityApiClient";
import OrganizationApiClient from "./OrganizationApiClient";
import ScheduleApiClient from "./ScheduleApiClient";
import {CalculationApiClient} from "./CalculationApiClient";

export default class RestUnitOfWork {
    public identity: IdentityApiClient;
    public organization: OrganizationApiClient;
    public schedule: ScheduleApiClient;
    public calculation: CalculationApiClient;

    constructor() {
        this.identity = new IdentityApiClient();
        this.organization = new OrganizationApiClient();
        this.schedule = new ScheduleApiClient();
        this.calculation = new CalculationApiClient();
    }
}