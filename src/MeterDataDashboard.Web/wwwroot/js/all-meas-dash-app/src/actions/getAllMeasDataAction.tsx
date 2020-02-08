import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";

export interface IGetAllMeasDataAction extends IAction {
    type: ActionType.getAllMeasData,
    payload: {}
}

export function getAllMeasDataAction(): IGetAllMeasDataAction {
    return {
        type: ActionType.getAllMeasData,
        payload: {}
    };
}
