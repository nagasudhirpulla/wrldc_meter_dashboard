import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";

export interface ISetEndTimeAction extends IAction {
    type: ActionType.setEndTime,
    payload: Date
}

export function setEndTimeAction(timeObj: Date): ISetEndTimeAction {
    return {
        type: ActionType.setEndTime,
        payload: timeObj
    };
}
