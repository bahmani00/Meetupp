import React, { Component } from 'react';
import { Header, Icon, List } from 'semantic-ui-react';
import axios from 'axios';

class App extends Component {
  state = {
    weatherForecasts: []
  };

  componentDidMount() {
    axios.get('http://localhost:5000/api/WeatherForecast')
      .then(response => {
        this.setState({
          weatherForecasts: response.data
        });
      });
  }

  render() {
    return (
      <div>
        <Header as='h2'>
          <Icon name='users' />
          <Header.Content>Facebuk</Header.Content>
        </Header>
        <List>
          {
            this.state.weatherForecasts.map((value: any) => (
              <List.Item key={value.id}>{value.summary} - temperature: {value.temperatureC}</List.Item>
          ))}
        </List>
      </div>
    );
  }
}

export default App;
