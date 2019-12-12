import { loadFictMeasurements, getFictMeasData } from "./fictMeasUtils";
import { setPlot } from "./plotUtils";
import $ from 'jquery';

window.onload = async () => {
    const measList = await loadFictMeasurements();
    const fictMeasSelect = document.getElementById("fictMeasSelect");
    //Create and append the options
    for (let i = 0; i < measList.length; i++) {
        var option = document.createElement("option");
        option.value = measList[i].locationTag;
        option.text = measList[i].description;
        fictMeasSelect.appendChild(option);
    }
    // https://harvesthq.github.io/chosen/options.html
    $(fictMeasSelect).chosen({
        placeholder_text_multiple: "Select Entities",
        no_results_text: "Oops, nothing found!"
    });
}

document.getElementById("addSeriesBtn").onclick = async () => {
    const startDate = (document.getElementById("start_date") as HTMLInputElement).value;
    const endDate = (document.getElementById("end_date") as HTMLInputElement).value;
    const fictMeasSelect = document.getElementById("fictMeasSelect") as HTMLSelectElement;

    let measDataList = [] as { title: string, data: number[] }[];

    // iterate over selected entities
    for (let optInd = 0; optInd < fictMeasSelect.options.length; optInd++) {
        if (fictMeasSelect.options[optInd].selected == true) {
            let locTag = fictMeasSelect.options[optInd].value;
            let locName = fictMeasSelect.options[optInd].text;
            var measData = await getFictMeasData(locTag, startDate, endDate);
            measDataList.push({ title: locName, data: measData })
        }
    }

    // render plot data
    setPlot("plotDiv", measDataList, `Meter Data from ${startDate} to ${endDate}`);
}