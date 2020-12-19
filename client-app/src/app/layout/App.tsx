import React, { useState, useEffect, Fragment } from 'react';
import { Container, Header, Icon, List } from 'semantic-ui-react';
import axios from 'axios';
import { IActivity } from 'app/models/activity';
import NavBar from 'features/nav/NavBar';


const App = () => { //using arrow functions(Lymbda)
  const [activities, setState] = useState<IActivity[]>([])

  useEffect(() => {
     axios.get<IActivity[]>('http://localhost:5000/api/activity')
      .then(response => {
        setState(response.data);
      });  
  }, []); // provide empty array [] to prevent this method gets called repeatedly

  return (
    <Fragment> {/* A return allways single component */}
      <NavBar/>
      <Container style={{marginTop: '7em'}} >
        <List>
          {
            activities.map((activity) => (
              <List.Item key={activity.id}>{activity.title}:&nbsp;&nbsp;{activity.description}</List.Item>
          ))}
        </List>
      </Container>      
    </Fragment>
  );
}

export default App;
