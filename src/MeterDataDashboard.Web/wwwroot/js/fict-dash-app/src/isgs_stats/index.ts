import { restartTimer, updateTimerPeriodFromUI } from "./timerStuff";
import Plotly from 'plotly.js-dist'
import { IsgsMarginsDTO } from "./type_defs/IsgsMarginsDTO";
import { stackedArea } from "./plotStuff";
import { getIsgsMargins } from "./fetchUtils";
import { createTable, exportTableToCSV } from "./tableExportUtils";

let isCheckBoxesListCreated = false;
let initialDesiredGenerators = [];
let global_g: { dcSchObj: IsgsMarginsDTO, plot_title: string } = { dcSchObj: null, 'plot_title': 'WR Down Margins Plot' };

export const doOnLoadStuff = (): void => {
    const todayDateStr: string = (new Date()).toISOString().substring(0, 10);
    (document.getElementById('start_date_input') as HTMLInputElement).value = todayDateStr;
    (document.getElementById('end_date_input') as HTMLInputElement).value = todayDateStr;
    //getMargins();
    updateTimerBtnCallback();
}

export const updateTimerBtnCallback = (): void => {
    updateTimerPeriodFromUI("marginFetchPeriodInput");
    restartTimer(timerFunc);
    getMargins();
}

export const timerFunc = (): void => {
    // timer execution
    updateDateString();
    getMargins();
}

export const getMargins = async (): Promise<void> => {
    document.getElementById('fetchStatusLabel').innerHTML = (new Date()).toLocaleString() + ': fetching margins started...';

    var start_date_str = (document.getElementById('start_date_input') as HTMLInputElement).value;
    var end_date_str = (document.getElementById('end_date_input') as HTMLInputElement).value;
    var margin_type = (document.getElementById('margin_type_combo') as HTMLInputElement).value;

    const dcSchObj = await getIsgsMargins(start_date_str, end_date_str, margin_type);
    global_g.dcSchObj = dcSchObj;

    // Now create the checkbox list
    if (isCheckBoxesListCreated == false) {
        var genSelectionDiv = document.getElementById('genSelectionDiv') as HTMLDivElement;
        genSelectionDiv.innerHTML = '';
        for (var k = 0; k < dcSchObj.genNames.length; k++) {
            var genName = dcSchObj.genNames[k];
            //check if the genName is a desired one
            var checkedString = '';
            if (initialDesiredGenerators.indexOf(genName) > -1) {
                checkedString = ' checked';
            }
            genSelectionDiv.innerHTML += '<label style="margin-right:20px"><input type="checkbox" class="gen_select" onclick="updatePlot()"' + checkedString + ' value="' + genName + '"> ' + genName + '</label>';
        }
        isCheckBoxesListCreated = true;
    }

    var marginTitleDict = { 'up': 'Up Reserves', 'down': 'Down Reserves', 'rras': 'RRAS', 'sced': 'SCED' }

    global_g['plot_title'] = `${marginTitleDict[margin_type]} for ${start_date_str} ${start_date_str != end_date_str ? `to ${end_date_str}` : ""}`;

    updatePlot();

    createTable(dcSchObj, document.getElementById('dcTable') as HTMLTableElement);

    document.getElementById('fetchStatusLabel').innerHTML = (new Date()).toLocaleString() + ': fetching data done!';
}

export const updateDateString = (): void => {
    var updateDateCheckbox = document.getElementById("updateDateCheckbox") as HTMLInputElement;
    if (updateDateCheckbox == null || updateDateCheckbox.checked != true) {
        // auto date update feature is disabled
        return;
    }
    // check date and change to today
    const today = new Date();
    let dd = "" + today.getDate();
    let mm = "" + today.getMonth() + 1; //January is 0!

    const yyyy = today.getFullYear();
    if (+dd < 10) {
        dd = '0' + dd;
    }
    if (+mm < 10) {
        mm = '0' + mm;
    }
    var todayStr = dd + '-' + mm + '-' + yyyy;
    if ((document.getElementById('start_date_input') as HTMLInputElement).value != todayStr) {
        (document.getElementById('start_date_input') as HTMLInputElement).value = todayStr;
        (document.getElementById('end_date_input') as HTMLInputElement).value = todayStr;
    }
}

export const selectAllGen = (ev: MouseEvent): void => {
    var genSelectionElements = document.getElementsByClassName("gen_select") as HTMLCollectionOf<HTMLInputElement>;
    for (var i = 0; i < genSelectionElements.length; i++) {
        genSelectionElements[i].checked = true;
    }
    updatePlot();
}

export const deselectAllGen = (ev: MouseEvent): void => {
    var genSelectionElements = document.getElementsByClassName("gen_select") as HTMLCollectionOf<HTMLInputElement>;
    for (var i = 0; i < genSelectionElements.length; i++) {
        genSelectionElements[i].checked = false;
    }
    updatePlot();
}

function updatePlot() {
    let dcSchObj = global_g.dcSchObj;
    // find the required generators from checkboxes
    var genSelectionElements = document.getElementsByClassName("gen_select") as HTMLCollectionOf<HTMLInputElement>;
    let activeGenerators = [];
    for (var i = 0; i < genSelectionElements.length; i++) {
        var isCheckedStatus = genSelectionElements[i].checked;
        if (typeof isCheckedStatus != "undefined" && isCheckedStatus == true) {
            activeGenerators.push(genSelectionElements[i].value);
        }
    }
    // Plot the margin values
    var dcPlotsDiv = document.getElementById("dcPlotsDiv") as HTMLDivElement;
    dcPlotsDiv.innerHTML = '';

    let xLabels: string[] = dcSchObj.timestamps;
    let traces = [];
    const lineStyle = (document.getElementById('line_style') as HTMLInputElement).value;
    let div = document.createElement('div') as any;
    div.className = 'sch_plot_div';
    div.id = 'plotDiv_0';
    div.style.border = '1px gray dashed';
    dcPlotsDiv.appendChild(div);
    for (var k = 0; k < dcSchObj.genNames.length; k++) {
        // dynamically create divs - https://stackoverflow.com/questions/14094697/javascript-how-to-create-new-div-dynamically-change-it-move-it-modify-it-in
        let genName = dcSchObj.genNames[k];
        if (activeGenerators.length != 0 && activeGenerators.indexOf(genName) == -1) { continue; }
        traces.push({
            x: xLabels,
            y: dcSchObj.margins[genName].map(x => x),
            fill: 'tonexty',
            name: genName,
            line: { shape: lineStyle }
        });
    }
    traces[0].fill = 'tozeroy';
    var layout = {
        title: global_g['plot_title'],
        xaxis: {
            title: 'Time'
        },
        yaxis: {
            title: 'MW'
        },
        legend: {
            font: {
                "size": "10"
            },
            orientation: "h"
        },
        margin: { 't': 35 },
        height: 800
    };
    Plotly.newPlot(div, stackedArea(traces), layout);
    div
        .on('plotly_hover', function (data) {
            if (data.points.length > 0) {
                var pointIndex = data.points[0]['pointNumber'];
                var textDataArray = (document.getElementById("plotDiv_0") as any).data;
                var infoStrings = [];
                for (var i = textDataArray.length - 1; i >= 0; i--) {
                    infoStrings.push(textDataArray[i]['text'][pointIndex]);
                }
                document.getElementById("reserveInfoDiv").innerHTML = "BLOCK (" + data.points[0]['x'] + ')<div style="height: 5px"></div>' + infoStrings.join('<div style="height: 5px"></div>');
            }
        })
        .on('plotly_unhover', function (data) {
            //document.getElementById("reserveInfoDiv").innerHTML = '';
        });
    document.getElementById('fetchStatusLabel').innerHTML = (new Date()).toLocaleString() + ': fetching, table, plot update done!';
}

window.onload = () => { doOnLoadStuff() }
// wire up selectAllGenBtn
(document.getElementById('selectAllGenBtn') as HTMLButtonElement).onclick = selectAllGen;
// wire up deselectAllGenBtn
(document.getElementById('deselectAllGenBtn') as HTMLButtonElement).onclick = deselectAllGen;
// wire up updateDateCheckbox
(document.getElementById('updateDateCheckbox') as HTMLInputElement).onchange = getMargins;
// wire up updateMarginsBtn
(document.getElementById('updateTimerPeriodBtn') as HTMLButtonElement).onclick = updateTimerBtnCallback;
// wire up selectAllGenBtn
(document.getElementById('updateMarginsBtn') as HTMLButtonElement).onclick = getMargins;
// wire up margin_type_combo
(document.getElementById('margin_type_combo') as HTMLSelectElement).onchange = getMargins;
// wire up exportBtn
(document.getElementById('exportBtn') as HTMLButtonElement).onclick = () => { exportTableToCSV('margin_export.csv') };