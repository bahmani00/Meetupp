import React from 'react';
import { observer } from 'mobx-react-lite';
import { NavLink } from 'react-router-dom';
import { Menu, Container, Button } from 'semantic-ui-react';

const NavBar: React.FC = () => {
  return (
    <Menu fixed='top' inverted>
      <Container>
        <Menu.Item header as={NavLink} exact to='/'>
          <img src='/assets/logo.png' alt='logo' style={{ marginRight: 10 }} />
          MeetUppy
        </Menu.Item>
        <Menu.Item name='Activities' as={NavLink} to='/activities' />
        <Menu.Item>
          <Button
            as={NavLink} to='/createActivity'
            positive
            content='Create Activity'
          />
        </Menu.Item>
      </Container>
    </Menu>
  );
};

export default observer(NavBar);
