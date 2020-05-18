let marginReloadTimerId = null;
let marginReloadMills = 600000; // 10 mins

export const stopMarginTimer = () => {
    clearInterval(marginReloadTimerId);
}

export const updateMarginTimerPeriod = (periodMilli) => {
    marginReloadMills = periodMilli;
}

export const restartTimer = (timerFunc) => {
    stopMarginTimer();
    marginReloadTimerId = setInterval(timerFunc, marginReloadMills);
}

export const updateTimerPeriodFromUI = (marginFetchPeriodInputId) => {
    const marginFetchPeriodInput = document.getElementById(marginFetchPeriodInputId) as HTMLInputElement;
    if (marginFetchPeriodInput) {
        const marginFetchPeriodMins = marginFetchPeriodInput.value;
        updateMarginTimerPeriod(+marginFetchPeriodMins * 60 * 1000);
    }
}

