import { IDashboardPageState } from "../type_defs/IDashboardPageState";
import { getMeterMeasList } from "../server_mediators/meterData";
import { ActionType } from '../actions/ActionType';
import { IAction } from "../type_defs/IAction";
import { useReducer, useCallback, useEffect } from "react";
import { createToast } from "../uitls/toastUtils";
import { getSchUtils, getSchTypes } from "../server_mediators/schArchData";
import { getScadaMeasList, getScadaMeasTypes } from "../server_mediators/scadaData";
import { setMeterMeasListAction, ISetMeterMeasListAction } from "../actions/setMeterMeasListAction";
import { setSchUtilsAction, ISetSchUtilsAction } from "../actions/setSchUtilsAction";
import { setSchTypesAction, ISetSchTypesAction } from "../actions/setSchTypesAction";
import { setScadaMeasTypesAction, ISetScadaMeasTypesAction } from "../actions/setScadaMeasTypesAction";
import { IGetScadaMeasListAction } from "../actions/getScadaMeasListAction";
import { setScadaMeasListAction, ISetScadaMeasListAction } from "../actions/setScadaMeasListAction";
import { getMeasDataAction, IGetMeasDataAction } from "../actions/getMeasDataAction";
import { getMeasData } from "../server_mediators/measDataFetcher";
import { setPlotDataAction, ISetPlotDataAction } from "../actions/setPlotDataAction";
import { getPlotXYArrays, getPlotTitle } from "../uitls/plotUtils";
import { IAddPlotMeasurementAction } from "../actions/addPlotMeasurementAction";
import { IDeletePlotMeasurementAction } from "../actions/deletePlotMeasurementAction";

export const useDashboardPageReducer = (initState: IDashboardPageState): [IDashboardPageState, React.Dispatch<IAction>] => {
    // create the reducer function
    const reducer = (state: IDashboardPageState, action: IAction) => {
        switch (action.type) {
            case ActionType.setMeterMeasList:
                return {
                    ...state,
                    ui: {
                        ...state.ui,
                        meterMeasList: (action as ISetMeterMeasListAction).payload
                    }
                } as IDashboardPageState;
                break;
            case ActionType.setSchUtils:
                return {
                    ...state,
                    ui: {
                        ...state.ui,
                        schArchUtils: (action as ISetSchUtilsAction).payload
                    }
                } as IDashboardPageState;
                break;
            case ActionType.setSchTypes:
                return {
                    ...state,
                    ui: {
                        ...state.ui,
                        schArchMeasTypes: (action as ISetSchTypesAction).payload
                    }
                } as IDashboardPageState;
                break;
            case ActionType.setScadaMeasList:
                return {
                    ...state,
                    ui: {
                        ...state.ui,
                        scadaMeasList: (action as ISetScadaMeasListAction).payload
                    }
                } as IDashboardPageState;
                break;
            case ActionType.setScadaMeasTypes:
                return {
                    ...state,
                    ui: {
                        ...state.ui,
                        scadaMeasTypes: (action as ISetScadaMeasTypesAction).payload
                    }
                } as IDashboardPageState;
                break;
            case ActionType.setPlotData:
                const setPlotDataActionObj = action as ISetPlotDataAction
                const measIter = setPlotDataActionObj.payload.measIter
                const seriesData = getPlotXYArrays(setPlotDataActionObj.payload.data)
                return {
                    ...state,
                    ui: {
                        ...state.ui,
                        plotData: [
                            ...state.ui.plotData.slice(0, measIter),
                            { meas: state.ui.plotData[measIter].meas, data: seriesData },
                            ...state.ui.plotData.slice(measIter + 1)
                        ]
                    }
                } as IDashboardPageState;
                break;
            case ActionType.addPlotMeasurement:
                const actionMeas = (action as IAddPlotMeasurementAction).payload
                return {
                    ...state,
                    ui: {
                        ...state.ui,
                        plotData: [
                            ...state.ui.plotData,
                            { meas: actionMeas, data: { timestamps: [], vals: [], title: getPlotTitle(actionMeas) } }
                        ]
                    }
                } as IDashboardPageState;
                break;
            case ActionType.deletePlotMeasurement:
                const deleteMeasIter = (action as IDeletePlotMeasurementAction).payload;
                return {
                    ...state,
                    ui: {
                        ...state.ui,
                        plotData: [
                            ...state.ui.plotData.slice(0, deleteMeasIter),
                            ...state.ui.plotData.slice(deleteMeasIter + 1),
                        ]
                    }
                } as IDashboardPageState;
                break;
            default:
                console.log("unwanted action detected");
                console.log(JSON.stringify(action));
                throw new Error();
            // return state also works
        }
    }

    // create the reducer hook
    let [pageState, pageStateDispatch]: [IDashboardPageState, React.Dispatch<IAction>] = useReducer(reducer, initState)

    // set comment tag types from server
    useEffect(() => {
        (async function () {
            const meterMeasList = await getMeterMeasList(pageState.urls.meterServiceBaseAddress);
            pageStateDispatch(setMeterMeasListAction(meterMeasList));

            const schUtils = await getSchUtils(pageState.urls.schArchServiceBaseAddress);
            pageStateDispatch(setSchUtilsAction(schUtils));

            const schTypes = await getSchTypes(pageState.urls.schArchServiceBaseAddress);
            pageStateDispatch(setSchTypesAction(schTypes));

            const scadaMeasTypes = await getScadaMeasTypes(pageState.urls.scadaServiceBaseAddress);
            pageStateDispatch(setScadaMeasTypesAction(scadaMeasTypes));
        })();
    }, [pageState.urls.meterServiceBaseAddress]);

    // created middleware to intercept dispatch calls that require async operations
    const asyncDispatch: React.Dispatch<IAction> = useCallback(async (action) => {
        switch (action.type) {
            case ActionType.getScadaMeasList: {
                const scadaMeasList = await getScadaMeasList(pageState.urls.scadaServiceBaseAddress, (action as IGetScadaMeasListAction).payload.measType)
                pageStateDispatch(setScadaMeasListAction(scadaMeasList));
                break;
            }
            case ActionType.getMeasData: {
                const getMeasDataActionObj = action as IGetMeasDataAction;
                const measIter = getMeasDataActionObj.payload.measIter
                if (measIter < 0 || measIter >= pageState.ui.plotData.length) {
                    break;
                }
                const meas = pageState.ui.plotData[measIter].meas
                const measData = await getMeasData(pageState.urls, meas, getMeasDataActionObj.payload.startDate, getMeasDataActionObj.payload.endDate)
                pageStateDispatch(setPlotDataAction(measIter, measData));
                break;
            }
            case ActionType.getAllMeasData: {
                const getMeasDataActionObj = action as IGetMeasDataAction;
                for (let measIter = 0; measIter < pageState.ui.plotData.length; measIter++) {
                    const meas = pageState.ui.plotData[measIter].meas
                    const measData = await getMeasData(pageState.urls, meas, getMeasDataActionObj.payload.startDate, getMeasDataActionObj.payload.endDate)
                    pageStateDispatch(setPlotDataAction(measIter, measData));
                }
                break;
            }
            default:
                pageStateDispatch(action);
        }
    }, [pageState.urls.meterServiceBaseAddress]); // The empty array causes this callback to only be created once per component instance

    return [pageState, asyncDispatch];
}