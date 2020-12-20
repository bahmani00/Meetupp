import { IActivity } from 'app/models/activity';
import axios, { AxiosResponse } from 'axios';

axios.defaults.baseURL = 'http://localhost:5000/api';

const responseBody = (response: AxiosResponse) => response.data;

const requests = {
    get:  (url: string) => axios.get(url).then(responseBody),
    post: (url: string, body: {}) => axios.post(url, body).then(responseBody),
    put:  (url: string, body: {}) => axios.put(url, body).then(responseBody),
    del:  (url: string) => axios.delete(url).then(responseBody),
};

const Activities = {
    list:    ():Promise<IActivity[]> => requests.get('/activity'),
    details: (id: string) => requests.get(`/activity/${id}`), //note: template string(string interpolation)
    create:  (activity: IActivity) => requests.post('/activity', activity),
    update:  (activity: IActivity) => requests.put(`/activity/${activity.id}`, activity),
    delete:  (id: string) => requests.del(`/activity/${id}`)
};

export default { Activities }