import { IMeterMeas } from "../type_defs/IMeterMeas";
import { MeasDiscriminator } from "../type_defs/MeasDiscriminator";
import { IMeas } from "../type_defs/IMeas";
import { getFictMeasData } from "./meterData";
import { IScadaMeas } from "../type_defs/IScadaMeas";
import { getScadaMeasData } from "./scadaData";
import { ISchArchMeas } from "../type_defs/ISchArchMeas";
import { getSchArchMeasData } from "./schArchData";
import { IDashboardPageState } from "../type_defs/IDashboardPageState";

export const getMeasData = async (baseAddrDict: IDashboardPageState['urls'], meas: IMeas, startDate: string, endDate: string): Promise<number[]> => {
    let plotData: number[] = [];
    if (meas.discriminator == MeasDiscriminator.meter) {
        plotData = await getFictMeasData(baseAddrDict.meterServiceBaseAddress, (meas as IMeterMeas).locationTag, startDate, endDate)
    } else if (meas.discriminator == MeasDiscriminator.scadaArch) {
        plotData = await getScadaMeasData(baseAddrDict.scadaServiceBaseAddress, (meas as IScadaMeas).measTag, startDate, endDate)
    } else if (meas.discriminator == MeasDiscriminator.schArch) {
        plotData = await getSchArchMeasData(baseAddrDict.schArchServiceBaseAddress, (meas as ISchArchMeas).utilName, (meas as ISchArchMeas).schType, startDate, endDate)
    }
    return plotData;
};