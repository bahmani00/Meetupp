import axios, { AxiosResponse } from 'axios';
import { IActivity } from '../models/activity';
import { history } from '../..';
import { toast } from 'react-toastify';

axios.defaults.baseURL = 'http://localhost:5000/api';

axios.interceptors.response.use(undefined, error => {
    if (error.message === 'Network Error' && !error.response) {
        toast.error('Network error - make sure API is running!')
    }
    const {status, data, config} = error.response;
    if (status === 404) {
        history.push('/notfound')
    }
    if (status === 400 && config.method === 'get' && data.errors.hasOwnProperty('id')) {
        history.push('/notfound')
    }
    if (status === 500) {
        toast.error('Server error - check the terminal for more info!')
    }
})

const responseBody = (response: AxiosResponse) => response.data;

const sleep = (ms: number) => (response: AxiosResponse) => 
    new Promise<AxiosResponse>(resolve => setTimeout(() => resolve(response), ms));
const sleepInMs = 500;        

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