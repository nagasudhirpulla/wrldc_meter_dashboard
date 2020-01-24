import toastr from 'toastr'

export const createToast = (msg: string, toastType: ToastrType, opts: ToastrOptions) => {
    let defOpts: ToastrOptions = {};
    defOpts.positionClass = 'toast-top-left';
    defOpts.extendedTimeOut = 1000;
    defOpts.timeOut = 1000;
    defOpts.progressBar = true;
    defOpts.closeButton = true;

    const toastOpts: ToastrOptions = { ...defOpts, ...opts }

    toastr[toastType](msg, null, toastOpts);
}