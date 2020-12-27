import { runInAction } from "mobx";

export interface IActivity {
    id: string;
    title: string;
    description: string;
    category: string;
    date: Date;
    city: string;
    venue: string;
    isGoing: boolean;
    isHost: boolean;
    attendees: IAttendee[]
}

//Partial: to have properties as optional while inheriting
export interface IActivityFormValues extends Partial<IActivity> {
    time?: Date;
}

export class ActivityFormValues implements IActivityFormValues {
    id?: string = undefined;
    title: string = '';
    category: string = '';
    description: string = '';
    date?: Date = undefined;
    time?: Date = undefined;
    city: string = '';
    venue: string = '';

    //init?: being able to use top default values
    constructor(init?: IActivityFormValues) {
        runInAction('loading activities', () => {        
            if (init && init.date) {
                init.time = init.date;
            }  
            //copy values from init to current instance
            Object.assign(this, init);
        })
    }
}

export interface IAttendee {
    username: string;
    displayName: string;
    image: string;
    isHost: boolean;
}