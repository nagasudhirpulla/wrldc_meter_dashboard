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
import { addPlotMeasurementAction } from '../actions/addPlotMeasurementAction';
import { deletePlotMeasurementAction } from '../actions/deletePlotMeasurementAction';

function DashboardPage() {
    let [pageState, pageStateDispatch] = useDashboardPageReducer(pageInitState);

    const onPlotMeasAdded = (meas: IMeas) => {
        console.log(`Adding measurement = `)
        console.log(JSON.stringify(meas))
        pageStateDispatch(addPlotMeasurementAction(meas))
    }

    const onPlotMeasDel = (measIter: number) => {
        return (() => {
            console.log(`Deleting measurement iterator = ${measIter}`)
            pageStateDispatch(deletePlotMeasurementAction(measIter))
        });
    };

    const onScadaMeasTypeChanged = (measType: string) => {
        console.log(`Changing SCADA meas type to ${measType}`)
        pageStateDispatch(getScadaMeasListAction(measType))
    }

    let plotMeasBucketItems = []
    for (var plotMeasIter = 0; plotMeasIter < pageState.ui.plotData.length; plotMeasIter++) {
        let plotMeasBucketItem = (
            <div style={{ border: '1px dotted black', display: 'inline-block', marginRight: '1em' }}>
                <span>{pageState.ui.plotData[plotMeasIter].data.title}</span>
                <button className="btn btn-sm btn-link" onClick={onPlotMeasDel(plotMeasIter)} style={{ color: 'red' }}>x</button>
            </div>);
        plotMeasBucketItems.push(plotMeasBucketItem);
    }

    const onPlotDataClick = () => {
        // https://github.com/nagasudhirpulla/electron_react_dashboard/blob/master/src/components/TimeSeriesLinePlot.tsx
        // todo complete this
    }

    return (
        <>
            <h3>Meter Measurement</h3>
            <MeterMeasPicker measList={pageState.ui.meterMeasList} onMeasSelected={onPlotMeasAdded}></MeterMeasPicker>
            <h3>Schedule Archive Measurement</h3>
            <SchArchMeasPicker
                schTypesList={pageState.ui.schArchMeasTypes}
                utilNamesList={pageState.ui.schArchUtils}
                onMeasSelected={onPlotMeasAdded} />
            <h3>SCADA Archive Measurement</h3>
            <ScadaArchMeasPicker
                measList={pageState.ui.scadaMeasList}
                measTypes={pageState.ui.scadaMeasTypes}
                onMeasTypeChanged={onScadaMeasTypeChanged}
                onMeasSelected={onPlotMeasAdded}
            />
            <div>
                {plotMeasBucketItems}
            </div>
            <br />
            <button onClick={onPlotDataClick}>Plot Data</button>
        </>
    );
}
export default DashboardPage;