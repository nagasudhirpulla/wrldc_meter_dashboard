import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";
import { IMeas } from "../type_defs/IMeas";

export interface IAddPlotMeasurementAction extends IAction {
    type: ActionType.addPlotMeasurement,
    payload: IMeas
}

export function addPlotMeasurementAction(meas: IMeas): IAddPlotMeasurementAction {
    return {
        type: ActionType.addPlotMeasurement,
        payload: meas
    };
}
