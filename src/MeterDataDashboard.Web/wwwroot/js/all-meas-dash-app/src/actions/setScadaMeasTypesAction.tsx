import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";

export interface ISetScadaMeasTypesAction extends IAction {
    type: ActionType.setScadaMeasTypes,
    payload: string[]
}

export function setScadaMeasTypesAction(measTypes: string[]): ISetScadaMeasTypesAction {
    return {
        type: ActionType.setScadaMeasTypes,
        payload: measTypes
    };
}
