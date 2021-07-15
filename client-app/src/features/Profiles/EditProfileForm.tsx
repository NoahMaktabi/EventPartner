import React, { useEffect, useState } from "react";
import { observer } from "mobx-react-lite";
import { Button, Form, Grid } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import { Profile, ProfileFormValues } from "../../app/models/profile";
import { Formik } from "formik";
import MyTextInput from "../../app/common/form/MyTextInput";
import MyTextArea from "../../app/common/form/MyTextArea";
import * as Yup from 'yup';

interface Props {
    profile: Profile;
    editMode: boolean;
}

export default observer(function EditProfileForm({profile, editMode}: Props) {
    const { profileStore: {  updateProfile } } = useStore();
    const [profileValues, setProfileValues] = useState<ProfileFormValues>(new ProfileFormValues(profile));


    const validationSchema = Yup.object({
        displayName: Yup.string().required('The display name is required'),
    })

    useEffect(() => {
        if (profile){
            setProfileValues(new ProfileFormValues(profile) )
        }
    }, [profile])

    function handleFormSubmit(values: ProfileFormValues){
        updateProfile(values);
        editMode = false;
    }

    return (
        <Grid>
            <Grid.Column width={16}>
                <Formik enableReinitialize
                    initialValues={profileValues}
                    validationSchema={validationSchema}
                    onSubmit={values => handleFormSubmit(values)} 
                >
                    {({ handleSubmit, isValid, isSubmitting, dirty }) => (
                    <Form className='ui form' onSubmit={handleSubmit} autoComplete='off'>
                        <MyTextInput name='displayName' placeholder='Display Name' />
                        <MyTextArea placeholder='Bio' name='bio' rows={3} />
                        <Button 
                            loading={isSubmitting} 
                            floated='right' 
                            positive 
                            type='submit' 
                            disabled={isSubmitting || !dirty || !isValid}
                            content='Submit' />
                    </Form>
                    )}
                </Formik>
            </Grid.Column>
        </Grid>
    )
})
