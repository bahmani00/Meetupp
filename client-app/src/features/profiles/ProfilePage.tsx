import React, { useContext, useEffect } from 'react'
import { Grid } from 'semantic-ui-react';
import ProfileHeader from './ProfileHeader';
import ProfileContent from './ProfileContent';
import { RouteComponentProps } from 'react-router';
import { RootStoreContext } from '../../app/stores/rootStore';
import LoadingComponent from '../../app/layout/LoadingComponent';
import { observer } from 'mobx-react-lite';

interface RouteParams {
    username: string
}

//to get username from Route
interface IProps extends RouteComponentProps<RouteParams> {}

const ProfilePage: React.FC<IProps> = ({match}) => {
    const rootStore = useContext(RootStoreContext);
    const {loadingProfile, profile, loadProfile} = rootStore.profileStore;

    useEffect(() => {
        loadProfile(match.params.username);
    }, [loadProfile, match])

    if (loadingProfile) return <LoadingComponent content='Loading profile...' />

    return (
        <Grid>
            <Grid.Column width={16}>
                <ProfileHeader profile={profile!} />
                <ProfileContent />
            </Grid.Column>
        </Grid>
    )
}
//it should be observable as profileStorage is observable
export default observer(ProfilePage);