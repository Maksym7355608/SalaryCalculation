import {Component} from "react";

interface HomeComponentProps {
    token: string;
}

class Home extends Component<HomeComponentProps, any> {
    constructor(props: HomeComponentProps) {
        super(props);
    }
    render() {
        return undefined;
    }
}

export default Home;