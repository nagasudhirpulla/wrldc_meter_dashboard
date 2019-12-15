import { loadScadaMeasurements, getScadaMeasData, loadScadaMeasTypes } from "./scadaMeasUtils";
import { setPlot } from "../plotUtils";
import $ from 'jquery';

window.onload = async () => {
    const scadaMeasSelect = document.getElementById("scadaMeasSelect");
    // https://harvesthq.github.io/chosen/options.html
    $(scadaMeasSelect).chosen({
        placeholder_text_multiple: "Select Measurements",
        no_results_text: "Oops, nothing found!"
    });
    await populateScadaMeasTypes();
    await updateMeasAsPerType();
}

document.getElementById("plotBtn").onclick = async () => {
    const startDate = (document.getElementById("start_date") as HTMLInputElement).value;
    const endDate = (document.getElementById("end_date") as HTMLInputElement).value;
    const scadaMeasSelect = document.getElementById("scadaMeasSelect") as HTMLSelectElement;

    let measDataList = [] as { title: string, data: number[] }[];

    // iterate over selected entities
    for (let optInd = 0; optInd < scadaMeasSelect.options.length; optInd++) {
        if (scadaMeasSelect.options[optInd].selected == true) {
            let measTag = scadaMeasSelect.options[optInd].value;
            let measName = scadaMeasSelect.options[optInd].text;
            var measData = await getScadaMeasData(measTag, startDate, endDate);
            measDataList.push({ title: measName, data: measData })
        }
    }

    // render plot data
    setPlot("plotDiv", measDataList, `Scada Archive Data from ${startDate} to ${endDate}`);
}

document.getElementById("measTypeSelect").onchange = async () => {
    await updateMeasAsPerType();
}

const updateMeasAsPerType = async () => {
    const measType: string = (document.getElementById("measTypeSelect") as HTMLSelectElement).value;
    await populateScadaMeasurements(measType);
}

const populateScadaMeasTypes = async () => {
    const measTypes = await loadScadaMeasTypes();
    const measTypeSelect = document.getElementById("measTypeSelect");
    //Create and append the options
    for (let i = 0; i < measTypes.length; i++) {
        var option = document.createElement("option");
        option.value = measTypes[i];
        option.text = measTypes[i];
        measTypeSelect.appendChild(option);
    }
}

const populateScadaMeasurements = async (measType: string) => {
    if (measType == null) {
        return;
    }
    const measList = await loadScadaMeasurements(measType);
    const scadaMeasSelect = document.getElementById("scadaMeasSelect");
    scadaMeasSelect.innerHTML = "";
    //Create and append the options
    for (let i = 0; i < measList.length; i++) {
        var option = document.createElement("option");
        option.value = measList[i].measTag;
        option.text = measList[i].description;
        scadaMeasSelect.appendChild(option);
    }
    // https://harvesthq.github.io/chosen/options.html
    $(scadaMeasSelect).chosen({
        placeholder_text_multiple: "Select Measurements",
        no_results_text: "Oops, nothing found!"
    });
}