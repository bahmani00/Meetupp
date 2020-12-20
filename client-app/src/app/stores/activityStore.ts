import { action, computed, configure, observable, runInAction } from "mobx";
import { createContext, SyntheticEvent } from "react";
import agent from "../api/agent";
import { IActivity } from "../models/activity";

configure({enforceActions: 'always'});

class ActivityStore{
    @observable activityRegistry = new Map();
    @observable activities: IActivity[] = [];
    @observable loadingInitial = false;
    @observable selectedActivity: IActivity | undefined;
    @observable editMode = false;
    @observable submitting = false;
    @observable target = "";

    @action loadActivities = async () => {
        this.loadingInitial = true;
        try {
            const activities = await agent.Activities.list();
            //name the runInAction for MobX devtools
            runInAction('loading Activities', () => {
                activities.forEach(activity => {
                    activity.date = activity.date.split('.')[0];
                    this.activityRegistry.set(activity.id, activity);
                });
            });
        } catch (error) {
            console.log(error);
        }finally{
            runInAction('Error loading Activities',() => {
                this.loadingInitial = false;
            });
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
            runInAction('create activity', () => {
                this.activityRegistry.set(activity.id, activity);
                this.editMode = false;
                this.submitting = false;
              })
        })
        .catch(error => console.log(error))
        .finally(() => runInAction('create activity error', () => {
            this.submitting = false;
          }));  
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
          runInAction('editing activity', () => {
            this.activityRegistry.set(activity.id, activity);
            this.selectedActivity = activity;
            this.editMode = false;
            this.submitting = false;
          })
        } catch (error) {
          runInAction('edit activity error', () => {
            this.submitting = false;
          })
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
      

      @action deleteActivity = async (event: SyntheticEvent<HTMLButtonElement>, id: string) => {
        this.submitting = true;
        this.target = event.currentTarget.name;
        try {
            await agent.Activities.delete(id);
            runInAction('deleting activity', () => {
                this.activityRegistry.delete(id);
                this.submitting = false;
                this.target = '';
              })
        } catch (error) {
            runInAction('delete activity error', () => {
                this.submitting = false;
                this.target = '';
              })
            console.log(error);
        }
      }      
}

export default createContext(new ActivityStore())