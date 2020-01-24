import { IMeas } from "./IMeas";

//http://portal.wrldc.in/dashboard/api/scadadata/WRLDCMP.SCADA3.A0103034/2020-01-22/2020-01-24
export interface IScadaMeas extends IMeas {
    measTag: string,
    description: string,
    measType: string
}