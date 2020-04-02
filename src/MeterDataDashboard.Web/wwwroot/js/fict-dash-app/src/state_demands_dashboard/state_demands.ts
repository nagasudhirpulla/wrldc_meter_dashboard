import { getScadaMeasData } from "../scada_dashboard/scadaMeasUtils"
import { exportPlotData, PlotTrace, getPlotXYArrays, setPlotTraces } from "../plotUtils"
import { convertDateTimeToUrlDate } from "../timeUtils"
declare const Plotly: any

let intervalID = null
const WrDemPnt = { id: 'WRLDCMP.SCADA1.A0047000', name: 'WR Demand' }
const FreqPnt = { id: 'WRLDCMP.SCADA1.A0036324', name: 'Freq' }
const GujDemPnt = { id: 'WRLDCMP.SCADA1.A0046957', name: 'Gujarat Demand' }
const MPDemPnt = { id: 'WRLDCMP.SCADA1.A0046978', name: 'MP Demand' }
const MahDemPnt = { id: 'WRLDCMP.SCADA1.A0046980', name: 'Mah Demand' }
const ChhatDemPnt = { id: 'WRLDCMP.SCADA1.A0046945', name: 'Chhattisgarh Demand' }
const DdDemPnt = { id: 'WRLDCMP.SCADA1.A0046948', name: 'DD Demand' }
const DnhDemPnt = { id: 'WRLDCMP.SCADA1.A0046953', name: 'DNH Demand' }
const GoaDemPnt = { id: 'WRLDCMP.SCADA1.A0046962', name: 'GOA Demand' }
const MumDemPnt = { id: 'WRLDCMP.SCADA3.A0101733', name: 'Mumbai Demand' }

window.onload = async () => {
    intervalID = setInterval(refreshData, 1000 * 60 * 10);
    refreshData()
}

const refreshData = async () => {
    // https://plot.ly/javascript/subplots/#stacked-subplots
    const nowTime = new Date()

    let tomTime = new Date(nowTime)
    tomTime.setDate(tomTime.getDate() + 1)

    let yestTime = new Date(nowTime)
    yestTime.setDate(nowTime.getDate() - 1)

    const yestDate = convertDateTimeToUrlDate(yestTime)
    const startDate = convertDateTimeToUrlDate(nowTime)
    const endDate = convertDateTimeToUrlDate(tomTime)
    const tracePnts = [WrDemPnt, FreqPnt, GujDemPnt, MahDemPnt, MPDemPnt, ChhatDemPnt, DdDemPnt, DnhDemPnt, GoaDemPnt, MumDemPnt]
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
    setPlotTraces("plotDiv", traces, 'State Demands', axTitles, 5, 2, 1000);

    (document.getElementById("plotDiv") as any).on('plotly_hover', function (eventdata: { xvals: any[]; }) {
        let axHandles = []
        for (var i = 2; i <= tracePnts.length; i++) {
            axHandles.push(`x${i}y${i}`)
        }
        Plotly.Fx.hover('plotDiv', { xval: eventdata.xvals[0] }, ['xy', ...axHandles]);
    });
}

document.getElementById("plotExportBtn").onclick = () => {
    exportPlotData("plotDiv");
}