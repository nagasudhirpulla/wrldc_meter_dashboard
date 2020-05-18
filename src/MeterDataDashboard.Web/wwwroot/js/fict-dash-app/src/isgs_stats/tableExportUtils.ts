import { IsgsMarginsDTO } from "./type_defs/IsgsMarginsDTO";

export const createTable = (tableData: IsgsMarginsDTO, tableEl: HTMLTableElement) => {
    // delete all table rows
    while (tableEl.rows.length > 0) {
        tableEl.deleteRow(0);
    }
    var row = document.createElement('tr');
    var cell = document.createElement('td');
    cell.appendChild(document.createTextNode('Time'));
    row.appendChild(cell);
    tableData.genNames.forEach(n => {
        var cell = document.createElement('td');
        cell.appendChild(document.createTextNode(n));
        row.appendChild(cell);
    });
    tableEl.appendChild(row);

    tableData.timestamps.forEach((ts, tsInd) => {
        var row = document.createElement('tr');
        var cell = document.createElement('td');
        cell.appendChild(document.createTextNode(ts));
        row.appendChild(cell);
        tableData.genNames.forEach(genName => {
            var cell = document.createElement('td');
            cell.appendChild(document.createTextNode(Math.round(tableData.margins[genName][tsInd] * 100) / 100 + ""));
            row.appendChild(cell);
        });
        tableEl.appendChild(row);
    });
}

export const exportTableToCSV = (filename: string) => {
    var csv = [];
    var rows = document.querySelectorAll("table tr");

    for (var i = 0; i < rows.length; i++) {
        var row = [], cols = rows[i].querySelectorAll("td, th") as NodeListOf<HTMLElement>;

        for (var j = 0; j < cols.length; j++)
            row.push(cols[j].innerText);

        csv.push(row.join(","));
    }

    // Download CSV file
    downloadCSV(csv.join("\n"), filename);
}

export const downloadCSV = (csv: string, filename: string) => {
    var csvFile;
    var downloadLink;

    // CSV file
    csvFile = new Blob([csv], { type: "text/csv" });

    // Download link
    downloadLink = document.createElement("a");

    // File name
    downloadLink.download = filename;

    // Create a link to the file
    downloadLink.href = window.URL.createObjectURL(csvFile);

    // Hide download link
    downloadLink.style.display = "none";

    // Add the link to DOM
    document.body.appendChild(downloadLink);

    // Click download link
    downloadLink.click();
}