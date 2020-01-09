import { loadWbesUtils, getWbesArchMeasData, loadSchTypes } from "./wbesArchUtils";
import { setPlot, exportPlotData } from "../plotUtils";
import $ from 'jquery';

window.onload = async () => {
    const wbesUtilSelect = document.getElementById("wbesUtilSelect");
    // https://harvesthq.github.io/chosen/options.html
    $(wbesUtilSelect).chosen({
        placeholder_text_multiple: "Select Utilities",
        no_results_text: "Oops, nothing found!"
    });
    await populateSchTypes();
    await populateWbesUtils();
    //todo create container for util sch multiple selection
}

document.getElementById("plotBtn").onclick = async () => {
    const startDate = (document.getElementById("start_date") as HTMLInputElement).value;
    const endDate = (document.getElementById("end_date") as HTMLInputElement).value;
    const wbesUtilSelect = document.getElementById("wbesUtilSelect") as HTMLSelectElement;
    const schType = (document.getElementById("schTypeSelect") as HTMLSelectElement).value;

    let measDataList = [] as { title: string, data: number[] }[];

    // iterate over selected entities
    for (let optInd = 0; optInd < wbesUtilSelect.options.length; optInd++) {
        if (wbesUtilSelect.options[optInd].selected == true) {
            let wbesUtil = wbesUtilSelect.options[optInd].value;
            var measData = await getWbesArchMeasData(wbesUtil, schType, startDate, endDate);
            measDataList.push({ title: wbesUtil, data: measData })
        }
    }

    // render plot data
    setPlot("plotDiv", measDataList, `WBES Archive Data from ${startDate} to ${endDate}`);
}

document.getElementById("plotExportBtn").onclick = () => {
    exportPlotData("plotDiv");
}

const populateSchTypes = async () => {
    const schTypes = await loadSchTypes();
    const schTypeSelect = document.getElementById("schTypeSelect");
    //Create and append the options
    for (let i = 0; i < schTypes.length; i++) {
        var option = document.createElement("option");
        option.value = schTypes[i].v;
        option.text = schTypes[i].t;
        schTypeSelect.appendChild(option);
    }
}

const populateWbesUtils = async () => {
    const measList = await loadWbesUtils();
    // Update options
    const wbesUtilSelect = document.getElementById("wbesUtilSelect");
    wbesUtilSelect.innerHTML = "";
    // Create and append options
    for (let i = 0; i < measList.length; i++) {
        var option = document.createElement("option");
        option.value = measList[i];
        option.text = measList[i];
        wbesUtilSelect.appendChild(option);
    }
    // https://harvesthq.github.io/chosen/options.html
    $(wbesUtilSelect).trigger("chosen:updated");
}