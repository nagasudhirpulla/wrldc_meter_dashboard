import React, { useState } from 'react';
import Plot from 'react-plotly.js';
import { Data, Datum, Config, Layout, Color } from 'plotly.js';
import { ITimeSeriesLinePlotProps } from '../type_defs/ITimeSeriesLinePlotProps';

function TimeSeriesLinePlot(props: ITimeSeriesLinePlotProps) {
    const [selMeas, setSelMeas] = useState(null);


    return (
        <>
            {/*<Plot
                data={plot_data}
                layout={plot_layout}
                frames={plot_frames}
                config={plot_config}
                style={{ width: '100%', height: '100%' }}
            />*/}
        </>
    );
}

export default TimeSeriesLinePlot;