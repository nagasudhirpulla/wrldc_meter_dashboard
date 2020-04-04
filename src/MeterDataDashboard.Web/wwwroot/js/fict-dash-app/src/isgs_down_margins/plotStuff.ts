export const stackedArea = (traces) => {
    var i, j;
    for (i = 0; i < traces.length; i++) {
        traces[i].text = [];
        traces[i].hoverinfo = 'text';
        for (j = 0; j < (traces[i]['y'].length); j++) {
            traces[i].text.push(traces[i]['name'] + " (" + traces[i]['y'][j].toFixed(0) + ")");
        }
    }
    for (i = 1; i < traces.length; i++) {
        for (j = 0; j < (Math.min(traces[i]['y'].length, traces[i - 1]['y'].length)); j++) {
            traces[i]['y'][j] += traces[i - 1]['y'][j];
        }
    }
    return traces;
}