import { action, computed, observable } from "mobx";
import { createContext } from "react";
import agent from "../api/agent";
import { IActivity } from "../models/activity";

class ActivityStore{
    @observable activityRegistry = new Map();
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
                this.activityRegistry.set(activity.id, activity);
            });
        } catch (error) {
            console.log(error);
        }finally{
            this.loadingInitial = false;
        }
    }

    @action selectActivity = (id: string) => {
        this.selectedActivity = this.activityRegistry.get(id);
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
        return Array.from(this.activityRegistry.values()).sort(
          (a, b) => Date.parse(a.date) - Date.parse(b.date)
        );
    }
    
    @action editActivity = async (activity: IActivity) => {
        this.submitting = true;
        try {
          await agent.Activities.update(activity);
          //runInAction('editing activity', () => {
            this.activityRegistry.set(activity.id, activity);
            this.selectedActivity = activity;
            this.editMode = false;
            this.submitting = false;
          //})
    
        } catch (error) {
          //runInAction('edit activity error', () => {
            this.submitting = false;
          //})
          console.log(error);
        }
      };
      @action showCloseEditForm = (id: string, show: boolean) => {
          if(show){
            this.selectedActivity = this.activityRegistry.get(id);
            this.editMode = true;
          }else{
            this.selectedActivity = undefined;
            this.editMode = false;
          }
      }      
}

export default createContext(new ActivityStore())