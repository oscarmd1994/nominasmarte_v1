$(function () {
    var tabProcesos;
    tabProcesos = $('#TbProcesos').DataTable({
        ajax: {
            url: "../Nomina/LoadMonitorProcesos",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            dataSrc: "",
            dataType: "json"
        },
        "columns": [
            { "data": "iIdTarea" },
            { "data": "sNombreDefinicion" },
            { "data": "sUsuario" },
            { "data": "sFechaIni" },
            { "data": "sFechaFinal" },
            { "data": "sEstatusFinal" }
            //{ "data": "sMensaje" }
        ],
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json"
        },
        ordering: false,
    });
    ////////////////////////////
    var shutDown = true;
    /// COUNTDOWN
    var count = 60;
    var counter = setInterval(timer, 1000);
    function timer() {
        if (shutDown) {
            $("#btnActualizarMonitor").attr("disabled", "disabled").addClass("disabled");
        }
        count = count - 1;
        if (count <= 0) {
            clearInterval(counter);
            $("#btnActualizarMonitor").removeAttr("disabled").removeClass("disabled");
        }
        let varSec = count % 60
        //SECONDS
        if (varSec == 60) {
            countS = '00';
        } else if (varSec >= 10) {
            countS = varSec;
        } else {
            countS = '0' + varSec;
        }
        if (document.getElementById("timer") != null) {
            if (countS == null || countS == "" || countS == undefined) {
                document.getElementById("timer").innerHTML = "";
            } else {
                document.getElementById("timer").innerHTML = countS;
            }
        }
    }

    $("#btnActualizarMonitor").click(() => {
        isContainerVisible(false)
        shutDown = true;
        count = 60;
        counter = setInterval(timer, 1000);
        tabProcesos.ajax.reload();
        isContainerVisible(true);
    });

    isContainerVisible = (isVisible) => {
        if (isVisible) {
            setTimeout(() => {
                $("#spinnerDiv").removeClass("d-block").addClass("d-none");
                $("#tabContainer").removeClass("d-none").addClass("d-block");
            }, 1000);
        } else {
            $("#tabContainer").removeClass("d-block").addClass("d-none");
            $("#spinnerDiv").removeClass("d-none").addClass("d-block");
        }
    }

});