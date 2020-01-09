export const loadSchTypes = async () => {
    try {
        const resp = await fetch(`../api/wbesArchive/getSchTypes`, {
            method: 'get'
        });
        const respJSON = await resp.json() as { t: string, v: string }[];
        // console.log(respJSON);
        return respJSON;
    } catch (e) {
        console.error(e);
        return [];
        //return { success: false, message: `Could not retrieve measurements data due to error ${JSON.stringify(e)}` };
    }
};

export const loadWbesUtils = async () => {
    try {
        const resp = await fetch(`../api/wbesArchive/getUtilities`, {
            method: 'get'
        });
        const respJSON = await resp.json() as string[];
        // console.log(respJSON);
        return respJSON;
    } catch (e) {
        console.error(e);
        return [];
        //return { success: false, message: `Could not retrieve measurements data due to error ${JSON.stringify(e)}` };
    }
};

export const getWbesArchMeasData = async (utilName: string, schType: string, startDate: string, endDate: string) => {

    try {
        const resp = await fetch(`../api/wbesArchive/${utilName}/${schType}/${startDate}/${endDate}`, {
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