import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";
import { IMeas } from "../type_defs/IMeas";

export interface IGetAllMeasDataAction extends IAction {
    type: ActionType.getAllMeasData,
    payload: { startDate: string, endDate: string }
}

export function getAllMeasDataAction(startDate: string, endDate: string): IGetAllMeasDataAction {
    return {
        type: ActionType.getAllMeasData,
        payload: { startDate: startDate, endDate: startDate }
    };
}
