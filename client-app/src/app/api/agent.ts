import axios, { AxiosResponse } from 'axios';
import { IActivity } from '../models/activity';

axios.defaults.baseURL = 'http://localhost:5000/api';

const sleep = (ms: number) => 
    (response: AxiosResponse) =>
        new Promise<AxiosResponse>(resolve => setTimeout(() => resolve(response), ms));
const sleepInMs = 500;        

const responseBody = (response: AxiosResponse) => response.data;

const requests = {
    get:  (url: string) => axios.get(url).then(sleep(sleepInMs)).then(responseBody),
    post: (url: string, body: {}) => axios.post(url, body).then(sleep(sleepInMs)).then(responseBody),
    put:  (url: string, body: {}) => axios.put(url, body).then(sleep(sleepInMs)).then(responseBody),
    del:  (url: string) => axios.delete(url).then(sleep(sleepInMs)).then(responseBody),
};

const Activities = {
    list:    ():Promise<IActivity[]> => requests.get('/activity'),
    details: (id: string) => requests.get(`/activity/${id}`), //note: template string(string interpolation)
    create:  (activity: IActivity) => requests.post('/activity', activity),
    update:  (activity: IActivity) => requests.put(`/activity/${activity.id}`, activity),
    delete:  (id: string) => requests.del(`/activity/${id}`)
};

// eslint-disable-next-line import/no-anonymous-default-export
export default { Activities }