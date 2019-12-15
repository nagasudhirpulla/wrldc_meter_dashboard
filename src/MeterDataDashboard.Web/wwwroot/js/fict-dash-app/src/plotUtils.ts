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

export const setPlot = (divId: string, measDataList: { data: number[], title: string }[], plotTitle:string) => {
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