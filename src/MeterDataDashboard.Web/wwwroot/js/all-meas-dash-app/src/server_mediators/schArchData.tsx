import { ISchType } from "../type_defs/ISchType";

export const getSchUtils = async (baseAddr: string): Promise<string[]> => {
    // http://portal.wrldc.in/dashboard/api/wbesArchive/getUtilities
    try {
        const resp = await fetch(`${baseAddr}/getUtilities`, {
            method: 'get'
        });
        const respJSON = await resp.json() as string[];
        // console.log(respJSON);
        return respJSON;
    } catch (e) {
        console.log(e);
        return null;
    }
}

export const getSchTypes = async (baseAddr: string): Promise<ISchType[]> => {
    // http://portal.wrldc.in/dashboard/api/wbesArchive/getSchTypes
    try {
        const resp = await fetch(`${baseAddr}/getSchTypes`, {
            method: 'get'
        });
        const respJSON = await resp.json() as {};
        // console.log(respJSON);
        return respJSON as ISchType[];
    } catch (e) {
        console.log(e);
        return null;
    }
}

export const getSchArchMeasData = async (baseAddr: string, utilName: string, schType: string, startDate: string, endDate: string): Promise<number[]> => {
    try {
        const resp = await fetch(`${baseAddr}/${utilName}/${schType}/${startDate}/${endDate}`, {
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