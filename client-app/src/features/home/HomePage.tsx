import { observer } from "mobx-react-lite";
import React from "react";
import { Link } from "react-router-dom";
import { Container, Header, Segment, Image, Button, Divider } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import LoginForm from "../users/LoginForm";
import RegisterForm from "../users/RegisterForm";

export default observer(function HomePage() {
    const { userStore, modalStore } = useStore();
    const url = window.location.href;
    const hasCode = url.includes("?code=");
    if (hasCode) {
        const newUrl = url.split("?code=");
        const code = newUrl[1]
        userStore.githubLogin(code);
    }



    return (
        <Segment inverted textAlign='center' vertical className='masthead' >
            <Container text >
                <Header as='h1' inverted >
                    <Image size='massive' src='/assets/logo.png' alt='logo' style={{ marginBottom: 12 }} />
                    Event-Partner
                </Header>
                {userStore.isLoggedIn ? (
                    <>
                        <Header as='h2' inverted content='Welcome to Event-Partner' />
                        <Button as={Link} to='/activities' size='huge' inverted >Go to Activities!</Button>
                    </>
                ) : (
                    <>
                        <Button
                            onClick={() => modalStore.openModal(<LoginForm />)}
                            size='huge' inverted >Login
                        </Button>
                        <Button
                            onClick={() => modalStore.openModal(<RegisterForm />)}
                            size='huge' inverted >Register
                        </Button>
                        <Divider horizontal inverted >Or</Divider>
                        {/* <Button
                            size='huge'
                            loading={userStore.fbLoading}
                            inverted
                            color='facebook'
                            content='Login with Facebook'
                            onClick={userStore.facebookLogin}
                        /> */}
                        <Button
                            size='huge'
                            loading={userStore.githubLoading}
                            inverted
                            icon='github'
                            color='facebook'
                            content='Login with Github'
                            as='a'
                            href='https://github.com/login/oauth/authorize?client_id=7aa057fd6473aad82e63'
                        />
                    </>

                )}
            </Container>
        </Segment>
    )
});