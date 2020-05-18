export interface IsgsMarginsDTO {
    genNames: string[];
    timestamps: string[];
    margins: {
        [key: string]: number[];
    };
}
