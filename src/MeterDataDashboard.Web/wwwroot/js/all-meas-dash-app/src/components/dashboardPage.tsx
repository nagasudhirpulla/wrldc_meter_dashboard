import pageInitState from '../initial_states/dashboardPageInitState';
import React from 'react';
import MeterMeasPicker from './meterMeasPicker';
import { useDashboardPageReducer } from '../reducers/dashBoardPageReducer';
import { IMeas } from '../type_defs/IMeas';
import SchArchMeasPicker from './schArchMeasPicker';
import ScadaArchMeasPicker from './scadaArchMeasPicker';
import { getScadaMeasListAction } from '../actions/GetScadaMeasListAction';
import { addPlotMeasurementAction } from '../actions/addPlotMeasurementAction';
import { deletePlotMeasurementAction } from '../actions/deletePlotMeasurementAction';
import { getAllMeasDataAction } from '../actions/getAllMeasDataAction';
import TimeSeriesLinePlot from './timeSeriesLinePlot';
import DateTime from 'react-datetime';
import moment from 'moment';
import { setStartTimeAction } from '../actions/setStartTimeAction';
import { setEndTimeAction } from '../actions/setEndTimeAction';
import { exportPlotData } from '../uitls/plotUtils';

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

    const onStartTimeChanged = (timeObj) => {
        if (timeObj instanceof moment) {
            let dateObj = moment(timeObj).toDate();
            pageStateDispatch(setStartTimeAction(dateObj))
        }
    }

    const onEndTimeChanged = (timeObj) => {
        if (timeObj instanceof moment) {
            let dateObj = moment(timeObj).toDate();
            pageStateDispatch(setEndTimeAction(dateObj))
        }
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
        pageStateDispatch(getAllMeasDataAction())
    }

    const onExportDataClick = () => {
        exportPlotData(pageState.ui.plotData)
    }

    return (
        <>
            <h3>Meter Measurement</h3>
            <MeterMeasPicker measList={pageState.ui.meterMeasList} onMeasSelected={onPlotMeasAdded}></MeterMeasPicker>
            <br/>
            <h3>Schedule Archive Measurement</h3>
            <SchArchMeasPicker
                schTypesList={pageState.ui.schArchMeasTypes}
                utilNamesList={pageState.ui.schArchUtils}
                onMeasSelected={onPlotMeasAdded} />
            <br/>
            <h3>SCADA Archive Measurement</h3>
            <ScadaArchMeasPicker
                measList={pageState.ui.scadaMeasList}
                measTypes={pageState.ui.scadaMeasTypes}
                onMeasTypeChanged={onScadaMeasTypeChanged}
                onMeasSelected={onPlotMeasAdded}
            />
            <br/>
            <div>
                <span>Start Time{" "}</span>
                <DateTime
                    value={pageState.ui.startTime}
                    dateFormat={'DD-MM-YYYY'}
                    timeFormat={'HH:mm:ss'}
                    onChange={onStartTimeChanged}
                    className={"timePicker"}
                />
            </div>
            <div>
                <span>End Time{"  "}</span>
                <DateTime
                    value={pageState.ui.endTime}
                    dateFormat={'DD-MM-YYYY'}
                    timeFormat={'HH:mm:ss'}
                    onChange={onEndTimeChanged}
                    className={"timePicker"}
                />
            </div>
            <br />
            <div>
                {plotMeasBucketItems}
            </div>
            <br />
            <button onClick={onPlotDataClick} className={"btn btn-success btn-sm btn-icon-split"}>
                <span className={"icon text-white-50"}>
                    <i className={"fas fa-sync"}></i>
                </span>
                <span className={"text"}>Plot Data</span>
            </button>
            <br />
            <br />
            <TimeSeriesLinePlot seriesList={pageState.ui.plotData} />
            <br />
            <button onClick={onExportDataClick} className={"btn btn-warning btn-sm btn-icon-split"}>
                <span className={"icon text-white-50"}>
                    <i className={"fas fa-download"}></i>
                </span>
                <span className={"text"}>Download CSV</span>
            </button>

            {/*<br />
            <pre>{JSON.stringify(pageState.ui.plotData, null, 2)}</pre>*/}
        </>
    );
}
export default DashboardPage;