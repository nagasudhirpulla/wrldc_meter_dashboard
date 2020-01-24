import { IScadaMeas } from "../type_defs/IScadaMeas";

export const getScadaMeasList = async (baseAddr: string, measType: string): Promise<IScadaMeas[]> => {
    // http://portal.wrldc.in/dashboard/api/scadadata/getMeasurements/sch_act_ui
    try {
        const resp = await fetch(`${baseAddr}/getMeasurements/${measType}`, {
            method: 'get'
        });
        const respJSON = await resp.json() as IScadaMeas[];
        // console.log(respJSON);
        for (var measIter = 0; measIter < respJSON.length; measIter++) {
            respJSON[measIter].discriminator = "scada"
        }
        return respJSON
    } catch (e) {
        console.log(e);
        return null;
    }
}

export const getScadaMeasTypes = async (baseAddr: string): Promise<string[]> => {
    // http://portal.wrldc.in/dashboard/api/scadadata/getMeasTypes
    try {
        const resp = await fetch(`${baseAddr}/getMeasTypes`, {
            method: 'get'
        });
        const respJSON = await resp.json() as {};
        // console.log(respJSON);
        return respJSON as string[];
    } catch (e) {
        console.log(e);
        return null;
    }
}