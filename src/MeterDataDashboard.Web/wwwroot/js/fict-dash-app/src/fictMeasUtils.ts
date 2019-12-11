﻿export const loadFictMeasurements = async () => {
    try {
        const resp = await fetch(`./api/fictdata/getMeasurements`, {
            method: 'get'
        });
        const respJSON = await resp.json() as { locationTag: string, description: string, [key: string]: any }[];
        // console.log(respJSON);
        return respJSON;
    } catch (e) {
        console.error(e);
        return [];
        //return { success: false, message: `Could not retrieve measurements data due to error ${JSON.stringify(e)}` };
    }
};

export const getFictMeasData = async (locationTag: string, startDate: string, endDate: string) => {

    try {
        const resp = await fetch(`./api/fictdata/${locationTag}/${startDate}/${endDate}`, {
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