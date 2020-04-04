export interface IsgsDownMarginsDTO {
    genNames: string[];
    timestamps: string[];
    downMargins: {
        [key: string]: number[];
    };
}
