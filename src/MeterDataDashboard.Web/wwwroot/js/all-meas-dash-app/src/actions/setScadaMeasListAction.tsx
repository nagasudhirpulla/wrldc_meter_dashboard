import { IAction } from "../type_defs/IAction";
import { ActionType } from "./ActionType";
import { IScadaMeas } from "../type_defs/IScadaMeas";

export interface ISetScadaMeasListAction extends IAction {
    type: ActionType.setScadaMeasList,
    payload: IScadaMeas[]
}

export function setScadaMeasListAction(measList: IScadaMeas[]): ISetScadaMeasListAction {
    //console.log(new Date());
    //console.log(measList);
    return {
        type: ActionType.setScadaMeasList,
        payload: measList
    };
}
