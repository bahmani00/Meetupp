import React, { useState, useEffect } from 'react';
import { Header, Icon, List } from 'semantic-ui-react';
import axios from 'axios';
import { IActivity } from 'app/models/activity';


const App = () => { //using arrow functions(Lymbda)
  const [activities, setState] = useState<IActivity[]>([])

  useEffect(() => {
     axios.get<IActivity[]>('http://localhost:5000/api/activity')
      .then(response => {
        setState(response.data);
      });  
  }, []); // provide empty array [] to prevent this method gets called repeatedly

  return (
    <div>
      <Header as='h2'>
        <Icon name='users' />
        <Header.Content>FaceBuk</Header.Content>
      </Header>
      <List>
        {
          activities.map((activity) => (
            <List.Item key={activity.id}>{activity.title}: {activity.description}</List.Item>
        ))}
      </List>
    </div>
  );
}

export default App;
