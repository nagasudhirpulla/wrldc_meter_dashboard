import { exportPlotData, PlotTrace, getPlotXYArrays, setPlotTraces } from "../plotUtils"
import { convertDateTimeToPmuUrlDate } from "../timeUtils"
import { getPmuMeasData } from "./pmuUtils"
declare const Plotly: any
let intervalID = null
const FreqPnt = { id: 13206, name: 'Frequency' }

window.onload = async () => {
    intervalID = setInterval(refreshData, 1000 * 10);
    refreshData()
}

const refreshData = async () => {
    const nowTime = new Date()
    const minsOffset = +((document.getElementById('minsOffsetInp') as HTMLInputElement).value);
    const startDate = convertDateTimeToPmuUrlDate(new Date(nowTime.getTime() - minsOffset * 60000))
    const endDate = convertDateTimeToPmuUrlDate(nowTime)
    const tracePnts = [FreqPnt]
    let traces = [] as PlotTrace[];
    let axTitles = [];
    // iterate over selected entities
    for (let traceInd = 0; traceInd < tracePnts.length; traceInd++) {
        const traceSuffix = (traceInd + 1) + ""
        //const traceSuffix = traceInd
        const tracePnt = tracePnts[traceInd]
        let fetchedMeasData = await getPmuMeasData(tracePnt.id, startDate, endDate);
        let traceData = getPlotXYArrays(fetchedMeasData);
        let traceObj: PlotTrace = {
            timestamps: traceData.timestamps.map(t => { t.setDate(t.getDate() + 1); return t; }), vals: traceData.vals, title: `${tracePnt.name}`, xaxis: `x${traceSuffix}`, yaxis: `y${traceSuffix}`, line: { color: 'magenta' }
        }
        traces.push(traceObj)
        axTitles.push({
            name: tracePnt.name,
            axisStr: 'yaxis' + (traceInd + 1)
        })
    }
    // render plot data
    setPlotTraces("plotDiv", traces, 'Real Time PMU Frequency', axTitles, 1, 1, 1000);

    (document.getElementById("plotDiv") as any).on('plotly_hover', function (eventdata: { xvals: any[]; }) {
        // https://codepen.io/duyentnguyen/pen/LRVbyY
        Plotly.Fx.hover('plotDiv', { xval: eventdata.xvals[0] }, ['xy']);
    });
}

document.getElementById("plotExportBtn").onclick = () => {
    exportPlotData("plotDiv");
}