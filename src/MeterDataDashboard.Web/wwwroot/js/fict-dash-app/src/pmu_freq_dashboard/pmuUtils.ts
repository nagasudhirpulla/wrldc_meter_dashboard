export const getPmuMeasData = async (measId: string, startDate: string, endDate: string) => {
    try {
        const resp = await fetch(`../api/pmudata/getdata/${measId}/${startDate}/${endDate}`, {
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