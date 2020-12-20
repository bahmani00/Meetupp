import { action, computed, observable } from "mobx";
import { createContext } from "react";
import agent from "../api/agent";
import { IActivity } from "../models/activity";

class ActivityStore{
    @observable activities: IActivity[] = [];
    @observable loadingInitial = false;
    @observable selectedActivity: IActivity | undefined;
    @observable editMode = false;
    @observable submitting = false;

    @action loadActivities = async () => {
        this.loadingInitial = true;
        try {
            const activities = await agent.Activities.list();
            activities.forEach(activity => {
                activity.date = activity.date.split('.')[0];
                this.activities.push(activity);
            });
        } catch (error) {
            console.log(error);
        }finally{
            this.loadingInitial = false;
        }
    }

    @action selectActivity = (id: string) => {
        this.selectedActivity = this.activities.find(a => a.id === id);
        this.editMode = false;
    };

    @action createActivity = async (activity: IActivity) => {
        this.submitting = true;
        agent.Activities.create(activity)
        .then(() => {
            this.activities.push(activity);
            this.selectedActivity = activity;
            this.editMode = false;
        })
        .catch(error => console.log(error))
        .finally(() => this.submitting = true);  
    }
    @action openCreateForm = () => {
        this.editMode = true;       
        this.selectedActivity = undefined;
    }
    @computed get activitiesByDate() {
        return this.activities.sort(
          (a, b) => Date.parse(a.date) - Date.parse(b.date)
        );
    }    
}

export default createContext(new ActivityStore())