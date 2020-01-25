import { IMeterMeas } from "../type_defs/IMeterMeas";
import { MeasDiscriminator } from "../type_defs/MeasDiscriminator";
// http://portal.wrldc.in/dashboard/api/fictdata/AC-91/2020-01-01/2020-01-24

export const getMeterMeasList = async (baseAddr: string): Promise<IMeterMeas[]> => {
    // http://portal.wrldc.in/dashboard/api/fictdata/getMeasurements
    try {
        const resp = await fetch(`${baseAddr}/getMeasurements`, {
            method: 'get'
        });
        const respJSON = await resp.json() as IMeterMeas[];
        // console.log(respJSON);
        for (var measIter = 0; measIter < respJSON.length; measIter++) {
            respJSON[measIter].discriminator = MeasDiscriminator.meter
        }
        return respJSON;
    } catch (e) {
        console.log(e);
        return null;
    }
}

export const getFictMeasData = async (baseAddr: string, locationTag: string, startDate: string, endDate: string): Promise<number[]> => {
    try {
        const resp = await fetch(`${baseAddr}/${locationTag}/${startDate}/${endDate}`, {
            method: 'get'
        });
        const respJSON = await resp.json() as number[];
        //console.log(respJSON);
        return respJSON;
    } catch (e) {
        console.error(e);
        return [];
        //return { success: false, message: `Could not retrieve measurements data due to error ${JSON.stringify(e)}` };
    }
};