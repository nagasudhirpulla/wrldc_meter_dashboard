import pageInitState from '../initial_states/dashboardPageInitState';
import React, { useReducer } from 'react';
import { useForm, Controller } from "react-hook-form";
import * as dateFormat from 'dateformat';
import MeterMeasPicker from './meterMeasPicker';
import { useDashboardPageReducer } from '../reducers/dashBoardPageReducer';
import { IMeas } from '../type_defs/IMeas';
import SchArchMeasPicker from './schArchMeasPicker';
import ScadaArchMeasPicker from './scadaArchMeasPicker';
import { getScadaMeasListAction } from '../actions/GetScadaMeasListAction';

function DashboardPage() {
    let [pageState, pageStateDispatch] = useDashboardPageReducer(pageInitState);
    const onMeasSelected = (meas: IMeas) => {
        console.log("Selected measurement = ")
        console.log(JSON.stringify(meas))
    }
    const onScadaMeasTypeChanged = (measType: string) => {
        console.log("Selected type = ")
        pageStateDispatch(getScadaMeasListAction(measType))
    }
    return (
        <>
            <h3>Meter Measurement</h3>
            <MeterMeasPicker measList={pageState.ui.meterMeasList} onMeasSelected={onMeasSelected}></MeterMeasPicker>
            <h3>Schedule Archive Measurement</h3>
            <SchArchMeasPicker
                schTypesList={pageState.ui.schArchMeasTypes}
                utilNamesList={pageState.ui.schArchUtils}
                onMeasSelected={onMeasSelected} />
            <h3>SCADA Archive Measurement</h3>
            <ScadaArchMeasPicker
                measList={pageInitState.ui.scadaMeasList}
                measTypes={pageState.ui.scadaMeasTypes}
                onMeasTypeChanged={onScadaMeasTypeChanged}
                onMeasSelected={onMeasSelected}
            />
        </>
    );
}
export default DashboardPage;