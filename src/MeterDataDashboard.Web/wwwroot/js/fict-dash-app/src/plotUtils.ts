import Plotly from 'plotly.js-dist';

const getPlotXYArrays = (measData: number[]): { timestamps: Date[], vals: number[] } => {
    let timestamps: Date[] = [];
    let vals: number[] = [];
    for (var i = 0; i < measData.length; i = i + 2) {
        timestamps.push(new Date(measData[i]));
        vals.push(measData[i + 1]);
    }
    return { timestamps: timestamps, vals: vals }
}

export const setPlot = (divId: string, measDataList: { data: number[], title: string }[], plotTitle: string) => {
    let traceData = [];
    const layout = {
        title: plotTitle,
        showlegend: true,
        legend: { "orientation": "h" }
    };

    for (var measIter = 0; measIter < measDataList.length; measIter++) {
        let measTraceData = getPlotXYArrays(measDataList[measIter].data);
        let measTraceName = measDataList[measIter].title;
        traceData.push({
            x: measTraceData.timestamps,
            y: measTraceData.vals,
            mode: 'lines',
            name: measTraceName,
        });
    }

    // https://stackoverflow.com/questions/39084438/how-to-import-plotly-js-into-typescript
    Plotly.newPlot(divId, traceData, layout);
};

export const getPlotData = (divId: string): string => {
    const plotData = (document.getElementById(divId) as any).data as { mode: string, name: string, x: Date[], y: number[] }[];
    var csvStr: string = "";
    if (plotData.length > 0) {
        for (var timeIter = 0; timeIter < plotData[0].x.length; timeIter++) {
            csvStr += `Time,${plotData[0].x.map((ts) => ts.toString()).join(',')}\n`
        }
    }
    else {
        return csvStr;
    }
    for (var seriesIter = 0; seriesIter < plotData.length; seriesIter++) {
        for (var timeIter = 0; timeIter < plotData[seriesIter].y.length; timeIter++) {
            csvStr += `Time,${plotData[seriesIter].y.map((val) => val.toString()).join(',')}\n`
        }
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