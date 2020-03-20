import { getScadaMeasData } from "../scada_dashboard/scadaMeasUtils"
import { exportPlotData, PlotTrace, getPlotXYArrays, setPlotTraces } from "../plotUtils"
import { convertDateTimeToUrlDate } from "../timeUtils"
declare const Plotly: any
let intervalID = null
const WrDemPnt = { id: 'WRLDCMP.SCADA1.A0047000', name: 'WR Demand' }
const FreqPnt = { id: 'WRLDCMP.SCADA1.A0036324', name: 'Freq' }

window.onload = async () => {
    intervalID = setInterval(refreshData, 1000 * 60 * 10);
    refreshData()
}

const refreshData = async () => {
    const nowTime = new Date()

    let tomTime = new Date(nowTime)
    tomTime.setDate(tomTime.getDate() + 1)

    let yestTime = new Date(nowTime)
    yestTime.setDate(nowTime.getDate() - 1)

    const yestDate = convertDateTimeToUrlDate(yestTime)
    const startDate = convertDateTimeToUrlDate(nowTime)
    const endDate = convertDateTimeToUrlDate(tomTime)
    const tracePnts = [WrDemPnt, FreqPnt]
    let traces = [] as PlotTrace[];
    let axTitles = [];
    // iterate over selected entities
    for (let traceInd = 0; traceInd < tracePnts.length; traceInd++) {
        const traceSuffix = (traceInd + 1) + ""
        //const traceSuffix = traceInd
        const tracePnt = tracePnts[traceInd]
        let fetchedMeasData = await getScadaMeasData(tracePnt.id, yestDate, startDate);
        let traceData = getPlotXYArrays(fetchedMeasData);
        let traceObj: PlotTrace = {
            timestamps: traceData.timestamps.map(t => { t.setDate(t.getDate() + 1); return t; }), vals: traceData.vals, title: `${tracePnt.name} Yest`, xaxis: `x${traceSuffix}`, yaxis: `y${traceSuffix}`, line: { color: 'magenta' }
        }
        traces.push(traceObj)
        fetchedMeasData = await getScadaMeasData(tracePnt.id, startDate, endDate);
        traceData = getPlotXYArrays(fetchedMeasData);
        traceObj = { timestamps: traceData.timestamps, vals: traceData.vals, title: tracePnt.name, xaxis: `x${traceSuffix}`, yaxis: `y${traceSuffix}`, line: { color: '#7B68EE' } }
        traces.push(traceObj)
        axTitles.push({
            name: tracePnt.name,
            axisStr: 'yaxis' + (traceInd + 1)
        })
    }
    // render plot data
    setPlotTraces("plotDiv", traces, 'Demand Frequency', axTitles, 2, 1, 1000);

    (document.getElementById("plotDiv") as any).on('plotly_hover', function (eventdata: { xvals: any[]; }) {
        // https://codepen.io/duyentnguyen/pen/LRVbyY
        Plotly.Fx.hover('plotDiv', { xval: eventdata.xvals[0] }, ['xy', 'x2y2']);
    });
}

document.getElementById("plotExportBtn").onclick = () => {
    exportPlotData("plotDiv");
}