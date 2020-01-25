import { IMeterMeas } from "./IMeterMeas";
import { IScadaMeas } from "./IScadaMeas";
import { ISchType } from "./ISchType";
import { IPlotData } from "./IPlotData";

[]

export interface IDashboardPageState {
    ui: {
        // for meter picker
        meterMeasList: IMeterMeas[],
        // for scada picker
        scadaMeasList: IScadaMeas[],
        scadaMeasTypes: string[],
        // for sch arch picker
        schArchUtils: string[],
        schArchMeasTypes: ISchType[],
        // for selected meas list
        plotData: IPlotData,
        startTime: Date,
        endTime: Date
    },
    urls: {
        meterServiceBaseAddress: string,
        scadaServiceBaseAddress: string,
        schArchServiceBaseAddress: string
    }
}
