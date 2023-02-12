import React, { useState, useEffect } from "react";
import { BrowserRouter, Switch, Route } from "react-router-dom";
import { UserManager } from "oidc-client";

function App() {
	return (
		<BrowserRouter>
			<Switch>
				<Route path="/signin-oidc" component={Callback} />
				<Route path="/" component={HomePage} />
			</Switch>
		</BrowserRouter>
	);
}

const IDENTITY_CONFIG = {
    authority: "https://localhost:7097",
    client_id: "interactive",
    redirect_uri: "http://localhost:3000/signin-oidc",
    post_logout_redirect_uri: "http://localhost:3000",
    response_type: "code",
    scope: "openid profile api.read",
};

function HomePage() {
    const [state, setState] = useState(null);
    var mgr = new UserManager(IDENTITY_CONFIG);

    useEffect(() => {
        mgr.getUser().then((user) => {
            if (user) {
                fetch("https://localhost:7280/weatherforecast", {
                    headers: {
                        Authorization: "Bearer " + user.access_token,
                    },
                })
                    .then((resp) => resp.json())
                    .then((data) => setState({user, data}));
            }
        });
    }, []);

    return (
        <div>
            {state ? (
                <>
                    <h3>Welcome {state?.user?.profile?.sub}</h3>
                    <pre>{JSON.stringify(state?.data, null, 2)}</pre>
                    <button onClick={() => mgr.signoutRedirect()}>
                        Log out
                    </button>
                </>
            ) : (
                <>
                    <h3>React Weather App</h3>
                    <button onClick={() => mgr.signinRedirect()}>
                        Login
                    </button>
                </>
            )}
        </div>
    );
}

function Callback() {
    useEffect(() => {
        var mgr = new UserManager({
            response_mode: "query",
        });

        mgr.signinRedirectCallback().then(() => (window.location.href = "/"));
    }, []);

    return <p>Loading...</p>;
}

export default App;