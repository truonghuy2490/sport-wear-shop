import Header from "./Header";
import Sidebar from "./Sidebar";

function Layout({ children }) {
    return (
        <div className="bg-light min-vh-100">

            <Header />

            <div className="d-flex">
                <Sidebar />

                <main className="flex-grow-1">
                    <div className="container-fluid p-4">
                        {children}
                    </div>
                </main>
            </div>

        </div>
    );
}

export default Layout;