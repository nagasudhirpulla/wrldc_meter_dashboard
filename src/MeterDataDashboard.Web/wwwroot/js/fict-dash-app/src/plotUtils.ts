import Plotly from 'plotly.js-dist';

export const getPlotXYArrays = (measData: number[]): { timestamps: Date[], vals: number[] } => {
    let timestamps: Date[] = [];
    let vals: number[] = [];
    for (var i = 0; i < measData.length; i = i + 2) {
        timestamps.push(new Date(measData[i]));
        vals.push(measData[i + 1]);
    }
    return { timestamps: timestamps, vals: vals }
}

export const setPlot = (divId: string, measDataList: { data: number[], title: string, lineShape?: string, fillMode?: string }[], plotTitle: string) => {
    let traceData = [];
    const layout = {
        title: plotTitle,
        showlegend: true,
        legend: { "orientation": "h" }
    };

    for (var measIter = 0; measIter < measDataList.length; measIter++) {
        let measTraceData = getPlotXYArrays(measDataList[measIter].data);
        let measTraceName = measDataList[measIter].title;
        let traceObj = {
            x: measTraceData.timestamps,
            y: measTraceData.vals,
            fill: 'none',
            mode: 'lines',
            name: measTraceName,
            line: {}
        }
        if (measDataList[measIter].lineShape != undefined) {
            traceObj['line']['shape'] = measDataList[measIter].lineShape
        }
        if (measDataList[measIter].fillMode != undefined) {
            traceObj['fill'] = measDataList[measIter].fillMode
        }
        traceData.push(traceObj);
    }

    // https://stackoverflow.com/questions/39084438/how-to-import-plotly-js-into-typescript
    Plotly.newPlot(divId, traceData, layout);
};

export interface PlotTrace {
    timestamps: Date[], vals: number[], title: string, xaxis: string, yaxis: string, line?: { color?: string, width?: string }
}

export const setPlotTraces = (divId: string, traces: PlotTrace[], plotTitle: string, axTitles: { name: string, axisStr: string }[], nRows: number, nCols: number, height?: number) => {
    let traceData = [];
    const layout = {
        title: plotTitle,
        showlegend: false,
        legend: { "orientation": "h" },
        grid: { rows: nRows, columns: nCols, pattern: 'independent' },
        height: height,
        autosize: true,
    }

    for (var axTitleIter = 0; axTitleIter < axTitles.length; axTitleIter++) {
        layout[axTitles[axTitleIter].axisStr] = { title: axTitles[axTitleIter].name };
    }

    for (var traceIter = 0; traceIter < traces.length; traceIter++) {
        const trace = traces[traceIter];
        let traceObj = {
            x: trace.timestamps,
            y: trace.vals,
            mode: 'lines',
            name: trace.title,
            xaxis: trace.xaxis,
            yaxis: trace.yaxis
        };
        if (trace.line != null) {
            traceObj['line'] = trace.line
        }

        traceData.push(traceObj);
    }

    Plotly.newPlot(divId, traceData, layout);
};

export const getPlotData = (divId: string): string => {
    const plotData = (document.getElementById(divId) as any).data as { mode: string, name: string, x: Date[], y: number[] }[];
    var csvStr: string = "";
    if (plotData.length > 0) {
        const makeTwoDigits = (num: number): string => {
            if (num < 10) {
                return `0${num}`;
            } else {
                return `${num}`;
            }
        }
        csvStr += `Time,${plotData[0].x.map((ts) => `${ts.getFullYear()}-${makeTwoDigits(ts.getMonth() + 1)}-${makeTwoDigits(ts.getDate())} ${makeTwoDigits(ts.getHours())}:${makeTwoDigits(ts.getMinutes())}:${makeTwoDigits(ts.getSeconds())}`).join(',')}\n`
    }
    else {
        return csvStr;
    }
    for (var seriesIter = 0; seriesIter < plotData.length; seriesIter++) {
        csvStr += `${plotData[seriesIter].name},${plotData[seriesIter].y.map((val) => val.toString()).join(',')}\n`
    }
    return csvStr;
}

export const exportPlotData = (divId: string): void => {
    const csvStr: string = getPlotData(divId);
    var hiddenElement = document.createElement('a');
    hiddenElement.href = 'data:text/csv;charset=utf-8,' + encodeURI(csvStr);
    hiddenElement.target = '_blank';
    hiddenElement.download = 'plotData.csv';
    hiddenElement.click();
}