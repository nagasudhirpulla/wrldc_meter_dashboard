﻿import { IMeterMeas } from "../type_defs/IMeterMeas";
// http://portal.wrldc.in/dashboard/api/fictdata/AC-91/2020-01-01/2020-01-24

export const getMeterMeasList = async (baseAddr): Promise<IMeterMeas[]> => {
    // http://portal.wrldc.in/dashboard/api/fictdata/getMeasurements
    try {
        const resp = await fetch(`${baseAddr}/getMeasurements`, {
            method: 'get'
        });
        const respJSON = await resp.json() as IMeterMeas[];
        // console.log(respJSON);
        for (var measIter = 0; measIter < respJSON.length; measIter++) {
            respJSON[measIter].discriminator = "meter"
        }
        return respJSON;
    } catch (e) {
        console.log(e);
        return null;
    }
}