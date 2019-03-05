$(document).ready(function () {

    var browser = checkBrowser();

    if (browser === "Firefox" || browser === "Safari") {
        alert("Your browser doesn't support datetime picker library. Try another browser or use Microsoft Edge.");
    }

    $("#holidaysDataSource").DataTable();
    $("#departmentsDataSource").DataTable();
    $("#rolesDataSource").DataTable();
    $("#emptypesDataSource").DataTable();
    $("#employeesDataSource").DataTable();
    $("#reqDataSource").DataTable();
    $("#balancesDataSource").DataTable();
    $("#leaveRequestsDataSource").DataTable();
    $("#reportsDataSource").DataTable();
    $("#leaveTypeDataSource").DataTable();

});

function checkBrowser() {
    var browsers = ["Chrome", "Firefox", "Opera", "Safari", "MSIE"];
    var b, ua = navigator.userAgent;
    for (var i = 0; i < browsers.length; i++) {
        if (ua.indexOf(browsers[i]) > -1) {
            b = browsers[i];
            break;
        }
    }
    return b;
}