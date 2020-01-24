import { IDashboardPageState } from "../type_defs/IDashboardPageState";

const initState: IDashboardPageState = {
    ui: {
        meterMeasList: [],
        scadaMeasList: [],
        scadaMeasTypes: [],
        schArchUtils: [],
        schArchMeasTypes: [],
        selectedMeasList: [],
        startTime: new Date(),
        endTime: new Date()
    },
    urls: {
        meterServiceBaseAddress: '../api/fictdata',
        scadaServiceBaseAddress: '../api/scadadata',
        schArchServiceBaseAddress: '../api/wbesArchive'
    }
};

export default initState;
