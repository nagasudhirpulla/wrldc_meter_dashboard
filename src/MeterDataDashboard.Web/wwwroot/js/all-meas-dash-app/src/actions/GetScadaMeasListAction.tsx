import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";

export interface IGetScadaMeasListPayload {
    measType: string
}

export interface IGetScadaMeasListAction extends IAction {
    type: ActionType.getScadaMeasList,
    payload: IGetScadaMeasListPayload
}

export function getScadaMeasListAction(measType: string): IGetScadaMeasListAction {
    return {
        type: ActionType.getScadaMeasList,
        payload: { measType: measType }
    };
}
