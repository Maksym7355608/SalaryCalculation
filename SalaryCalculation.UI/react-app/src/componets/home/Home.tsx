import BasePageModel from "../BasePageModel";

class Home extends BasePageModel {

    componentDidMount() {
        document.title = "Пошук працівників";
    }

    render() {
        return (
            <div className="display-1">Home Page</div>
        );
    }
}

export default Home;