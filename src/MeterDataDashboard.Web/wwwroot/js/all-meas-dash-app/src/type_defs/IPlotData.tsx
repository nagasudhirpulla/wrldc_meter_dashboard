import { IMeas } from "./IMeas";
import { ISeriesData } from "./ISeriesData";
export interface IPlotData extends Array<{
    meas: IMeas;
    data: ISeriesData;
}> { }