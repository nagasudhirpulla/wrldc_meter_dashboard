import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";

export interface ISetSchUtilsAction extends IAction {
    type: ActionType.setSchUtils,
    payload: string[]
}

export function setSchUtilsAction(utilsList: string[]): ISetSchUtilsAction {
    return {
        type: ActionType.setSchUtils,
        payload: utilsList
    };
}
