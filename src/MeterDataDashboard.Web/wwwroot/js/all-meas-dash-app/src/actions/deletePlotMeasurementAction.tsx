import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";

export interface IDeletePlotMeasurementAction extends IAction {
    type: ActionType.deletePlotMeasurement,
    payload: number
}

export function deletePlotMeasurementAction(measIter: number): IDeletePlotMeasurementAction {
    return {
        type: ActionType.deletePlotMeasurement,
        payload: measIter
    };
}
