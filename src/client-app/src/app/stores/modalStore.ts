import { RootStore } from "./rootStore";
import { observable, action } from "mobx";

export default class ModalStore {
    rootStore: RootStore;
    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;
    }

    //Decorator which creates an observable converts its value (objects, maps or arrays) into
    // a shallow observable structure
    @observable.shallow modal = {
        open: false,
        body: null // the body is keep changing no need to be observed always
    }
    
    @action openModal = (content: any) => {
        this.modal.open = true;
        this.modal.body = content;
    }

    @action closeModal = () => {
        this.modal.open = false;
        this.modal.body = null;
    }
}