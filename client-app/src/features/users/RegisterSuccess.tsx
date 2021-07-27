import React from "react";
import { observer } from "mobx-react-lite";
import useQuery from "../../app/common/util/hooks";
import agent from "../../app/api/agent";
import { toast } from "react-toastify";
import { Button, Header, Icon, Segment } from "semantic-ui-react";

export default observer(function RegisterSuccess() {
    const email = useQuery().get('email') as string;

    function handleConfirmEmailResend() {
        agent.Account.resendEmailConfirm(email).then(() => {
            toast.success('Verification email resent. Please check your inbox');
        }).catch(error => console.log(error))
    }

    return (
        <Segment placeholder textAlign='center' >
            <Header icon color='green'>
                <Icon name='check' />
                Successfully Registered!
            </Header>
            <p>Please check your email (including junk email) for the verification email</p>
            {email &&
                <>
                    <p>Didn't receive the email? Click the below button to resend</p>
                    <Button primary 
                    onClick={handleConfirmEmailResend} 
                    content='Resend email' size='huge' />
                </>
            }
        </Segment>
    )
})