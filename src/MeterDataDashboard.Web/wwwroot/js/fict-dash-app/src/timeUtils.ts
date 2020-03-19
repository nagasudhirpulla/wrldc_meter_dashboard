export const convertDateTimeToUrlDate = (inp: Date): string => {
    return `${inp.getFullYear()}-${ensureTwoDigits(inp.getMonth() + 1)}-${ensureTwoDigits(inp.getDate())}`;
}

const ensureTwoDigits = (num: number): string => {
    if (num < 10) {
        return "0" + num;
    }
    return "" + num;
}