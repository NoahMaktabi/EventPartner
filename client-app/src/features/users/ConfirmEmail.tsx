import React, { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Button, Header, Icon, Segment } from "semantic-ui-react";
import agent from "../../app/api/agent";
import useQuery from "../../app/common/util/hooks";
import { useStore } from "../../app/stores/store";
import LoginForm from "./LoginForm";


export default function ConfirmEmail() {
    const {modalStore} = useStore();
    const email = useQuery().get('email') as string;
    const token = useQuery().get('token') as string;

    const Status = {
        Verifying: 'Verifying',
        Failed: 'Failed',
        Success: 'Success'
    }

    const [status, setStaus] = useState(Status.Verifying);
    function handleConfirmEmailResend() {
        agent.Account.resendEmailConfirm(email).then(() => {
            toast.success('Verification email resent. Please check your inbox');
        }).catch(error => console.log(error))
    }

    useEffect(() => {
        agent.Account.verifyEmail(token, email).then(() => {
            setStaus(Status.Success)
        }).catch(() => {
            setStaus(Status.Failed);
        })
    }, [Status.Failed, Status.Success, token, email])

    function getBody() {
        switch (status) {
            case Status.Verifying:
                return <p>Verifying...</p>;
            case Status.Failed:
                return (
                    <div>
                        <p>Verification failed. You can try resending the verify link to your email</p>
                        <Button primary onClick={handleConfirmEmailResend} size='huge' content='Resend Email' />
                    </div>
                );
            case Status.Success: 
                    return (
                        <div>
                            <p>Email has been verified. You can now login</p>
                            <Button positive onClick={() => modalStore.openModal(<LoginForm />)} size='huge' content='Login' /> 
                        </div>
                    );
        }
    }

    return (
        <Segment placeholder textAlign='center' >
            <Header icon>
                <Icon name='envelope' />
                Email verification
            </Header>
            <Segment.Inline>
                {getBody()}
            </Segment.Inline>
        </Segment>
    )
} 