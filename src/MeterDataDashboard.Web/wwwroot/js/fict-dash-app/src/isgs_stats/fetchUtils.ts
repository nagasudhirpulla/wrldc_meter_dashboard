import { IsgsMarginsDTO } from "./type_defs/IsgsMarginsDTO";

export const getIsgsMargins = async (startDate: string, endDate: string, margin_type: string): Promise<IsgsMarginsDTO> => {
    let fetchUrl = `../api/WbesLiveData/GetIsgsThermalDownMargins/${startDate}/${endDate}`
    if (margin_type == 'up') {
        fetchUrl = `../api/WbesLiveData/GetIsgsThermalUpMargins/${startDate}/${endDate}`
    }
    else if (margin_type == 'rras') {
        fetchUrl = `../api/WbesLiveData/GetIsgsRras/${startDate}/${endDate}`
    }
    else if (margin_type == 'sced') {
        fetchUrl = `../api/WbesLiveData/GetIsgsSced/${startDate}/${endDate}`
    }
    try {
        const resp = await fetch(fetchUrl, {
            method: 'get'
        });
        const respJSON = await resp.json() as IsgsMarginsDTO;
        //console.log(respJSON);
        return respJSON;
    } catch (e) {
        console.error(e);
        return null;
        //return { success: false, message: `Could not retrieve measurements data due to error ${JSON.stringify(e)}` };
    }
};