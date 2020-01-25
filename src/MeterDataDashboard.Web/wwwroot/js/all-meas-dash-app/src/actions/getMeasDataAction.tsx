import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";
import { IMeas } from "../type_defs/IMeas";

export interface IGetMeasDataAction extends IAction {
    type: ActionType.getMeasData,
    payload: { measIter: number, startDate: string, endDate: string }
}

export function getMeasDataAction(measIter: number, startDate: string, endDate: string): IGetMeasDataAction {
    return {
        type: ActionType.getMeasData,
        payload: { measIter: measIter, startDate: startDate, endDate: startDate }
    };
}
