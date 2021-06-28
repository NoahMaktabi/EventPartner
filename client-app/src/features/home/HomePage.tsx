import { observer } from "mobx-react-lite";
import React from "react";
import { Link } from "react-router-dom";
import { Container } from "semantic-ui-react";

export default observer(function HomePage() {
    return (
        <Container style={{margingTop: '7em'}} >
            <h1>Home Page</h1>
            <h3>Go To <Link to='/activities' >Activities</Link> </h3>
        </Container>
    )
});