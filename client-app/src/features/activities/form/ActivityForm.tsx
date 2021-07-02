import { observer } from "mobx-react-lite";
import React, {  useEffect, useState } from "react";
import { Link, useHistory, useParams } from "react-router-dom";
import { Segment, Button, Header } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { useStore } from "../../../app/stores/store";
import { v4 as uuid } from 'uuid';
import { Formik, Form } from "formik";
import * as Yup from 'yup';
import MyTextInput from "../../../app/common/form/MyTextInput";
import MyTextArea from "../../../app/common/form/MyTextArea";
import { categoryOptions } from "../../../app/common/options/categoryOptions";
import MySelectInput from "../../../app/common/form/MySelectInput";
import MyDateInput from "../../../app/common/form/MyDateInput";
import { Activity } from "../../../app/models/activity";



export default observer(function ActivityForm() {
    const history = useHistory();
    const { activityStore } = useStore();
    const { createActivity, updateActivity,
        loading, loadActivity, loadingInitial } = activityStore;

    const { id } = useParams<{ id: string }>();
    const [activity, setActivity] = useState<Activity>({
        id: '',
        title: '',
        category: '',
        description: '',
        city: '',
        date: null,
        venue: ''
    });

    useEffect(() => {
        if (id) loadActivity(id).then(activity => setActivity(activity!));

    }, [id, loadActivity])

    const validationSchema = Yup.object({
        title: Yup.string().required('The activity title is required'),
        description: Yup.string().required('The description  is required'),
        category: Yup.string().required('The category is required'),
        date: Yup.string().required('The date is required').nullable(),
        city: Yup.string().required('The city is required'),
        venue: Yup.string().required('The venue is required'),
    })


    function handleFormSubmit(activity: Activity){
        if (activity.id.length === 0) {
            let newActivity = {
                ...activity, 
                id: uuid()
            };
            createActivity(newActivity).then(() => history.push(`/activities/${newActivity.id}`));
        }
        else {
            updateActivity(activity).then(() => history.push(`/activities/${activity.id}`));
        }
    }


    if (id)
        if (loadingInitial) return <LoadingComponent content='Loading activity...' />

    return (
        <Segment clearing>
            <Header content='Activity Details' sub color='teal' />
            <Formik enableReinitialize
                initialValues={activity}
                validationSchema={validationSchema}
                onSubmit={values => handleFormSubmit(values)} >
                {({ handleSubmit, isValid, isSubmitting, dirty }) => (
                    <Form className='ui form' onSubmit={handleSubmit} autoComplete='off'  >
                        <MyTextInput name='title' placeholder='Title' />
                        <MyTextArea placeholder='Description' name='description' rows={3} />
                        <MySelectInput placeholder='Category' name='category' options={categoryOptions} />
                        <MyDateInput placeholderText='Date'
                          name='date' 
                          showTimeSelect
                          timeCaption='time'
                          dateFormat='d  MMMM yyyy -- hh:mm'

                          />
                        <Header content='Location Details' sub color='teal' />
                        <MyTextInput placeholder='City' name='city' />
                        <MyTextInput placeholder='Venue' name='venue' />

                        <Button 
                            loading={loading} 
                            floated='right' 
                            positive 
                            type='submit' 
                            disabled={isSubmitting || !dirty || !isValid}
                            content='Submit' />
                        <Button as={Link} to='/activities' floated='right' type='button' content='Cancel' />

                    </Form>
                )}
            </Formik>

        </Segment>
    )
})