import React, { useState, useEffect, Fragment } from 'react';
import { Container, Header, Icon, List } from 'semantic-ui-react';
import axios from 'axios';
import { IActivity } from 'app/models/activity';
import NavBar from 'features/nav/NavBar';
import ActivityDashboard from 'features/activities/ActivityDashboard';


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
        <ActivityDashboard activities={activities}/>
      </Container>      
    </Fragment>
  );
}

export default App;
