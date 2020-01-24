import { IScadaMeas } from "./IScadaMeas";
export interface IScadaArchMeasPickerProps {
    measList: IScadaMeas[],
    measTypes: string[],
    onMeasSelected: any,
    onMeasTypeChanged: any
}