import { loadNodeNames, getNodeStatusHist } from "./scadaPingHistUtils";
import { setPlot, exportPlotData } from "../plotUtils";
import $ from 'jquery';

window.onload = async () => {
    const measList = await loadNodeNames();
    const nodeNameSelect = document.getElementById("nodeNameSelect");
    //Create and append the options
    for (let i = 0; i < measList.length; i++) {
        var option = document.createElement("option");
        option.value = measList[i];
        option.text = measList[i];
        nodeNameSelect.appendChild(option);
    }
    // https://harvesthq.github.io/chosen/options.html
    $(nodeNameSelect).chosen({
        placeholder_text_multiple: "Select Nodes",
        no_results_text: "Oops, nothing found!"
    });
}

document.getElementById("plotBtn").onclick = async () => {
    const startDate = (document.getElementById("start_date") as HTMLInputElement).value + '-00-00-00';
    const endDate = (document.getElementById("end_date") as HTMLInputElement).value + '-23-59-59';
    const nodeNameSelect = document.getElementById("nodeNameSelect") as HTMLSelectElement;

    let pingStatusList = [] as { data: number[], title: string, lineShape?: string, fillMode?: string }[];

    // iterate over selected entities
    for (let optInd = 0; optInd < nodeNameSelect.options.length; optInd++) {
        if (nodeNameSelect.options[optInd].selected == true) {
            let locTag = nodeNameSelect.options[optInd].value;
            let locName = nodeNameSelect.options[optInd].text;
            var measData = await getNodeStatusHist(locTag, startDate, endDate);
            pingStatusList.push({ title: locName, data: measData, lineShape: 'hv', fillMode: 'tozeroy' })
        }
    }

    // render plot data
    setPlot("plotDiv", pingStatusList, `Ping status from ${startDate} to ${endDate}`);
}

document.getElementById("plotExportBtn").onclick = () => {
    exportPlotData("plotDiv");
}