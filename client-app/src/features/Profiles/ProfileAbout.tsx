import React, { useState } from "react";
import { observer } from "mobx-react-lite";
import { Button, Header, Tab } from "semantic-ui-react";
import { Profile } from "../../app/models/profile";
import { useStore } from "../../app/stores/store";
import EditProfileForm from "./EditProfileForm";

interface Props {
        profile: Profile
}

export default observer(function ProfileAbout({ profile }: Props) {
        const { profileStore: { isCurrentUser } } = useStore();
        const [editMode, setEditMode] = useState(false);

        return (
                <Tab.Pane>
                        {isCurrentUser &&
                                <Button
                                        basic
                                        content={editMode ? 'Cancel' : 'Edit Profile'}
                                        floated='right'
                                        onClick={() => setEditMode(!editMode)}
                                />}
                        {!editMode &&
                                <>
                                        <Header as='h3' content={'About ' + profile.displayName} />
                                        <p style={{ whiteSpace: 'pre-wrap' }} 
                                        >{profile.bio}</p>
                                </>
                        }
                        {editMode && 
                                <EditProfileForm profile={profile} editMode={editMode} />

                        }
                </Tab.Pane>
        )
})