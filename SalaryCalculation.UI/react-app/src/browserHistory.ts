import { createBrowserHistory } from "history";

const browserHistory = createBrowserHistory();

browserHistory.replace({ ...browserHistory.location });

export default browserHistory;