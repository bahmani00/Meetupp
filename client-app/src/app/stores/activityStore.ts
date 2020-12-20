import { observable } from "mobx";
import { createContext } from "react";

class ActivityStore{
    @observable title = 'Hello from MobX'
}

export default createContext(new ActivityStore())