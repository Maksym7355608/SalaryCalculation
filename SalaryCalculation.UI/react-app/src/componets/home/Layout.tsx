import Header from "./Header";
import Footer from "./Footer";
import BasePageModel from "../BasePageModel";
import Menu from "./Menu";
import React, {ComponentType, FC, ReactElement} from "react";

class Layout extends BasePageModel<{ children: any }> {
    constructor(props: { children: any }) {
        super(props);
    }

    render() {
        return (
            <div>
                <main>
                    <Header/>
                    <div>
                        <Menu/>
                        {this.props.children}
                        <span id="requestInvalid" className="text-danger"></span>
                        <span id="responseInvalid" className="text-danger"></span>
                    </div>
                </main>
                <footer>
                    <Footer/>
                </footer>
            </div>
        );
    }
}

const withLayout = <P extends object>(
    WrappedComponent: ComponentType<P>,
): FC<P> => (props: P): ReactElement => (
    <Layout>
        <WrappedComponent {...props} />
    </Layout>
);

export default withLayout;