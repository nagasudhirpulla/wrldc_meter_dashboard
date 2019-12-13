export const loadScadaMeasurements = async () => {
    try {
        const resp = await fetch(`${window.location.origin}/api/scadadata/getMeasurements`, {
            method: 'get'
        });
        const respJSON = await resp.json() as { measTag: string, description: string, [key: string]: any }[];
        // console.log(respJSON);
        return respJSON;
    } catch (e) {
        console.error(e);
        return [];
        //return { success: false, message: `Could not retrieve measurements data due to error ${JSON.stringify(e)}` };
    }
};

export const getScadaMeasData = async (measTag: string, startDate: string, endDate: string) => {

    try {
        const resp = await fetch(`${window.location.origin}/api/scadadata/${measTag}/${startDate}/${endDate}`, {
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