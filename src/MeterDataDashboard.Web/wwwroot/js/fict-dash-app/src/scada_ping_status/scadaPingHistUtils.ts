export const loadNodeNames = async () => {
    try {
        const resp = await fetch(`../api/pingStatus/getMeasurements`, {
            method: 'get'
        });
        const respJSON = await resp.json() as string[];
        // console.log(respJSON);
        return respJSON;
    } catch (e) {
        console.error(e);
        return [];
        //return { success: false, message: `Could not retrieve node names due to error ${JSON.stringify(e)}` };
    }
};

export const getNodeStatusHist = async (nodeName: string, startDate: string, endDate: string) => {

    try {
        const resp = await fetch(`../api/pingStatus/${nodeName}/${startDate}/${endDate}`, {
            method: 'get'
        });
        const respJSON = (await resp.json() as number[])
        //console.log(respJSON);
        return respJSON;
    } catch (e) {
        console.error(e);
        return [];
        //return { success: false, message: `Could not retrieve node status history due to error ${JSON.stringify(e)}` };
    }
};