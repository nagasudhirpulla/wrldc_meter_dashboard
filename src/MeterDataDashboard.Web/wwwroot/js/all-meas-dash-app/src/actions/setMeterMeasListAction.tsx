import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";
import { IMeterMeas } from "../type_defs/IMeterMeas";

export interface ISetMeterMeasListAction extends IAction {
    type: ActionType.setMeterMeasList,
    payload: IMeterMeas[]
}

export function setMeterMeasListAction(measList: IMeterMeas[]): ISetMeterMeasListAction {
    return {
        type: ActionType.setMeterMeasList,
        payload: measList
    };
}
