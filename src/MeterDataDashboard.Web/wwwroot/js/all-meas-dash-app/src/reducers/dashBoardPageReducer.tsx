import { IDashboardPageState } from "../type_defs/IDashboardPageState";
import { getMeterMeasList } from "../server_mediators/meterData";
import * as actionTypes from '../actions/actionTypes';
import { IAction } from "../type_defs/IAction";
import { useReducer, useCallback, useEffect } from "react";
import { createToast } from "../uitls/toastUtils";
import { IMeterMeas } from "../type_defs/IMeterMeas";
import { getSchUtils, getSchTypes } from "../server_mediators/schArchData";
import { ISchType } from "../type_defs/ISchType";

export const useDashboardPageReducer = (initState: IDashboardPageState): [IDashboardPageState, React.Dispatch<IAction>] => {
    // create the reducer function
    const reducer = (state: IDashboardPageState, action: IAction) => {
        switch (action.type) {
            case actionTypes.setMeterMeasListAction:
                return {
                    ...state,
                    ui: {
                        ...state.ui,
                        meterMeasList: action.payload as IMeterMeas[]
                    }
                } as IDashboardPageState;
            case actionTypes.setSchUtilsListAction:
                return {
                    ...state,
                    ui: {
                        ...state.ui,
                        schArchUtils: action.payload as string[]
                    }
                } as IDashboardPageState;
            case actionTypes.setSchTypesListAction:
                return {
                    ...state,
                    ui: {
                        ...state.ui,
                        schArchMeasTypes: action.payload as ISchType[]
                    }
                } as IDashboardPageState;
            default:
                throw new Error();
            // return state also works
        }
    }

    // create the reducer hook
    let [pageState, pageStateDispatch]: [IDashboardPageState, React.Dispatch<IAction>] = useReducer(reducer, initState)

    // set comment tag types from server
    useEffect(() => {
        (async function () {
            const measList = await getMeterMeasList(pageState.urls.meterServiceBaseAddress);
            pageStateDispatch({
                type: actionTypes.setMeterMeasListAction,
                payload: measList
            });
            const schUtilsList = await getSchUtils(pageState.urls.schArchServiceBaseAddress);
            pageStateDispatch({
                type: actionTypes.setSchUtilsListAction,
                payload: schUtilsList
            });
            const schTypesList = await getSchTypes(pageState.urls.schArchServiceBaseAddress);
            pageStateDispatch({
                type: actionTypes.setSchTypesListAction,
                payload: schTypesList
            });
        })();
    }, [pageState.urls.meterServiceBaseAddress]);

    // created middleware to intercept dispatch calls that require async operations
    const asyncDispatch: React.Dispatch<IAction> = useCallback(async (action) => {
        switch (action.type) {
            case actionTypes.getMeasDataAction: {
                break;
            }
            default:
                pageStateDispatch(action);
        }
    }, [pageState.urls.meterServiceBaseAddress]); // The empty array causes this callback to only be created once per component instance

    return [pageState, asyncDispatch];
}