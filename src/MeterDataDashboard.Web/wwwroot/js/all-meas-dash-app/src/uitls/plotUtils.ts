﻿import { IDashboardPageState } from '../type_defs/IDashboardPageState';
import { IPlotData } from '../type_defs/IPlotData';
import { IMeas } from '../type_defs/IMeas';
import { MeasDiscriminator } from '../type_defs/MeasDiscriminator';
import { IMeterMeas } from '../type_defs/IMeterMeas';
import { IScadaMeas } from '../type_defs/IScadaMeas';
import { ISchArchMeas } from '../type_defs/ISchArchMeas';

export const getPlotXYArrays = (measData: number[]): { timestamps: Date[], vals: number[] } => {
    let timestamps: Date[] = [];
    let vals: number[] = [];
    for (var i = 0; i < measData.length; i = i + 2) {
        timestamps.push(new Date(measData[i]));
        vals.push(measData[i + 1]);
    }
    return { timestamps: timestamps, vals: vals }
}

export const getPlotDataCsvString = (plotData: IPlotData): string => {
    var csvStr: string = "";
    if (plotData.length > 0) {
        const makeTwoDigits = (num: number): string => {
            if (num < 10) {
                return `0${num}`;
            } else {
                return `${num}`;
            }
        }
        csvStr += `Time,${plotData[0].data.timestamps.map((ts) => `${ts.getFullYear()}-${makeTwoDigits(ts.getMonth() + 1)}-${makeTwoDigits(ts.getDate())} ${makeTwoDigits(ts.getHours())}:${makeTwoDigits(ts.getMinutes())}:${makeTwoDigits(ts.getSeconds())}`).join(',')}\n`
    }
    else {
        return csvStr;
    }
    for (var seriesIter = 0; seriesIter < plotData.length; seriesIter++) {
        csvStr += `${plotData[seriesIter].data.title},${plotData[seriesIter].data.vals.map((val) => val.toString()).join(',')}\n`
    }
    return csvStr;
}

export const exportPlotData = (plotData: IDashboardPageState['ui']['plotData']): void => {
    const csvStr: string = getPlotDataCsvString(plotData);
    var hiddenElement = document.createElement('a');
    hiddenElement.href = 'data:text/csv;charset=utf-8,' + encodeURI(csvStr);
    hiddenElement.target = '_blank';
    hiddenElement.download = 'plotData.csv';
    hiddenElement.click();
}

export const getPlotTitle = (meas: IMeas): string => {
    let plotTitle: string = "";
    if (meas.discriminator == MeasDiscriminator.meter) {
        plotTitle = (meas as IMeterMeas).description
    } else if (meas.discriminator == MeasDiscriminator.scadaArch) {
        plotTitle = (meas as IScadaMeas).description
    } else if (meas.discriminator == MeasDiscriminator.schArch) {
        const schMeas = meas as ISchArchMeas
        plotTitle = `${schMeas.utilName}|${schMeas.schType}`
    }
    return plotTitle;
}