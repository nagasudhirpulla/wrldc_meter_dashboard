import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";

export interface ISetPlotDataAction extends IAction {
    type: ActionType.setPlotData,
    payload: { data: number[], measIter: number }
}

export function setPlotDataAction(measIter: number, data: number[]): ISetPlotDataAction {
    return {
        type: ActionType.setPlotData,
        payload: { measIter: measIter, data: data }
    };
}
