import React from 'react';
import createPlotlyComponent from 'react-plotly.js';
import { Data, Datum, Config, Layout } from 'plotly.js';
// https://www.npmjs.com/package/react-plotlyjs
import Plotly from 'plotly.js-cartesian-dist';
import { ITimeSeriesLinePlotProps } from '../type_defs/ITimeSeriesLinePlotProps';

function TimeSeriesLinePlot(props: ITimeSeriesLinePlotProps) {
    const PlotlyComponent = createPlotlyComponent(Plotly);
    const generateSeriesData = (seriesIter: number): Data => {
        let series_data_template: Data = {
            name: props.seriesList[seriesIter].data.title,
            x: [], y: [], mode: 'lines', line: { width: 2 }
        }
        let seriesData: Data = { ...series_data_template };
        // get points from series data
        for (let pntIter = 0; pntIter < props.seriesList[seriesIter].data.timestamps.length; pntIter++) {
            let xVal: Datum = props.seriesList[seriesIter].data.timestamps[pntIter];
            let yVal: Datum = props.seriesList[seriesIter].data.vals[pntIter];
            (seriesData.x as Datum[]).push(xVal);
            (seriesData.y as Datum[]).push(yVal);
        }
        return seriesData;
    }

    const generatePlotData = () => {
        let plot_data = []
        for (let seriesIter = 0; seriesIter < props.seriesList.length; seriesIter++) {
            plot_data.push(generateSeriesData(seriesIter));
        }
        return plot_data;
    }

    let plot_data: Data[] = generatePlotData();
    let plot_layout: Partial<Layout> = {
        autosize: true,
        legend: { orientation: "h" },
        title: { text: "Data" }
    };
    let plot_frames = [];
    let plot_config: Partial<Config> = {};

    return (
        <>
            <PlotlyComponent
                data={plot_data}
                layout={plot_layout}
                frames={plot_frames}
                config={plot_config}
                style={{ width: '100%', height: '100%' }}
            />
        </>
    );
}

export default TimeSeriesLinePlot;