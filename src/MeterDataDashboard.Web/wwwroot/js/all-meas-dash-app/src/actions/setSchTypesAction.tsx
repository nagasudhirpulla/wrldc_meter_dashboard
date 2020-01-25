import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";
import { ISchType } from "../type_defs/ISchType";

export interface ISetSchTypesAction extends IAction {
    type: ActionType.setSchTypes,
    payload: ISchType[]
}

export function setSchTypesAction(schTypes: ISchType[]): ISetSchTypesAction {
    return {
        type: ActionType.setSchTypes,
        payload: schTypes
    };
}
