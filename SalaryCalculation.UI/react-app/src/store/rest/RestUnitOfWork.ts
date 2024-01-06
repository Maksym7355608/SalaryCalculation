import IdentityApiClient from "./IdentityApiClient";
import OrganizationApiClient from "./OrganizationApiClient";
import ScheduleApiClient from "./ScheduleApiClient";
import CalculationApiClient from "./CalculationApiClient";
import DictionaryApiClient from "./DictionaryApiClient";

export default class RestUnitOfWork {
    public identity: IdentityApiClient;
    public organization: OrganizationApiClient;
    public schedule: ScheduleApiClient;
    public calculation: CalculationApiClient;
    public dictionary: DictionaryApiClient;

    constructor() {
        this.identity = new IdentityApiClient();
        this.organization = new OrganizationApiClient();
        this.schedule = new ScheduleApiClient();
        this.calculation = new CalculationApiClient();
        this.dictionary = new DictionaryApiClient();
    }


}