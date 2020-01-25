import { ActionType } from "../actions/ActionType";

export interface IAction {
    type: ActionType;
    payload?: any;
}