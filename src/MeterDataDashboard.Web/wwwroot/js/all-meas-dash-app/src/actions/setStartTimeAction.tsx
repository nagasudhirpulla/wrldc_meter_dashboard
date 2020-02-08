import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";

export interface ISetStartTimeAction extends IAction {
    type: ActionType.setStartTime,
    payload: Date
}

export function setStartTimeAction(timeObj: Date): ISetStartTimeAction {
    return {
        type: ActionType.setStartTime,
        payload: timeObj
    };
}
