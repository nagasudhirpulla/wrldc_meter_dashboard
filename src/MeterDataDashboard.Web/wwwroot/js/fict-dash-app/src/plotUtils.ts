import Plotly from 'plotly.js-dist';

export const setPlot = (divId: string, measName: string, measData: number[]) => {
    let timestamps: Date[] = [];
    let vals: number[] = [];
    for (var i = 0; i < measData.length; i = i + 2) {
        timestamps.push(new Date(measData[i]));
        vals.push(measData[i + 1]);
    }
    const plotData = [{
        x: timestamps,
        y: vals,
        mode: 'lines',
        name: measName,
    }];
    const layout = {
        title: 'Data'
    };
    // https://stackoverflow.com/questions/39084438/how-to-import-plotly-js-into-typescript
    Plotly.newPlot(divId, plotData, layout);
};