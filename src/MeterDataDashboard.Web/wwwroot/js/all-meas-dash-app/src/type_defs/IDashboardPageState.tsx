import { IMeas } from "./IMeas";
import { IMeterMeas } from "./IMeterMeas";
import { IScadaMeas } from "./IScadaMeas";
import { ISchType } from "./ISchType";

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
        selectedMeasList: IMeas[],
        startTime: Date,
        endTime: Date
    },
    urls: {
        meterServiceBaseAddress: string,
        scadaServiceBaseAddress: string,
        schArchServiceBaseAddress: string
    }
}
