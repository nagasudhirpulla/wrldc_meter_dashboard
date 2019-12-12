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
    const fictMeasSelect = document.getElementById("fictMeasSelect") as HTMLSelectElement;
    const locationTag = fictMeasSelect.options[fictMeasSelect.selectedIndex].value;
    const startDate = (document.getElementById("start_date") as HTMLInputElement).value;
    const endDate = (document.getElementById("end_date") as HTMLInputElement).value;
    var measData = await getFictMeasData(locationTag, startDate, endDate);
    setPlot("plotDiv", locationTag, measData);
}