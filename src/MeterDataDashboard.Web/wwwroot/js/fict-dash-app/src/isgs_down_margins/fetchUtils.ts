import { IsgsDownMarginsDTO } from "./type_defs/IsgsDownMarginsDTO";

export const getIsgsDownMargins = async (startDate: string, endDate: string): Promise<IsgsDownMarginsDTO> => {
    try {
        const resp = await fetch(`../api/WbesLiveData/GetIsgsThermalDownMargins/${startDate}/${endDate}`, {
            method: 'get'
        });
        const respJSON = await resp.json() as IsgsDownMarginsDTO;
        //console.log(respJSON);
        return respJSON;
    } catch (e) {
        console.error(e);
        return null;
        //return { success: false, message: `Could not retrieve measurements data due to error ${JSON.stringify(e)}` };
    }
};