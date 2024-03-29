import { observer } from "mobx-react-lite";
import React from "react";
import { Link } from "react-router-dom";
import { Card, Icon, Image } from "semantic-ui-react";
import { Profile } from "../../app/models/profile";
import FollowButton from "./FollowButton";

interface Props {
    profile: Profile;
}

export default observer(function ProfileCard({ profile }: Props) {
    const truncate= (str: string) =>  {
        return str.length > 10 ? str.substring(0, 70) + "..." : str;
    }

    return (
        <Card as={Link} to={`/profiles/${profile.username}`} >
            <Image src={profile.image || '/assets/user.png'} />
            <Card.Content>
                <Card.Header>{profile.displayName}</Card.Header>
                <Card.Description>
                    <span style={{ overflow: 'hidden', textOverflow: 'ellipsis' }}>
                        {profile.bio? truncate(profile.bio!) : 'No profile bio yet' }
                    </span>
                </Card.Description>
            </Card.Content>
            <Card.Content extra>
                <Icon name='user' />
                {profile.followersCount} followers
            </Card.Content>
            <FollowButton profile={profile} />
        </Card>
    )
})