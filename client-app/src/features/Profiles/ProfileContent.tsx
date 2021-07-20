import React from "react";
import { observer } from "mobx-react-lite";
import { Tab } from "semantic-ui-react";
import ProfilePhotos from "./ProfilePhotos";
import { Profile } from "../../app/models/profile";
import ProfileAbout from "./ProfileAbout";
import ProfileFollowings from "./ProfileFollowings";
import { useStore } from "../../app/stores/store";
import ProfileActivites from "./ProfileActivites";

interface Props {
    profile: Profile
}

export default observer (function ProfileContent({profile}: Props) {
    const {profileStore} = useStore();

    const panes = [
        {menuItem: 'About', render: () => <ProfileAbout profile={profile} /> },
        {menuItem: 'Photos', render: () => < ProfilePhotos profile={profile} />},
        {menuItem: 'Events', render: () => <ProfileActivites />},
        {menuItem: 'Followers', render: () => <ProfileFollowings /> },
        {menuItem: 'Following', render: () => <ProfileFollowings />},
    ]
    return (
        <Tab 
            menu={{fluid: true, vertical: true}}
            menuPosition='right'
            panes={panes}
            onTabChange={(e, data) => profileStore.setActiveTab(data.activeIndex)}
        />
    )
})