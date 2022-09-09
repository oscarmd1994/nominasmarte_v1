﻿$(function () {

    // CONSTANTES

    const typeReportselectCatalogs = document.getElementById('typeReportselectCatalogs');
    const btnGenerateReportCatalogs = document.getElementById('btnGenerateReportCatalogs');

    const contentGenerateRepCatalogs = document.getElementById('contentGenerateRepCatalogs');

    // Funcion que muestra alertas de forma dinamica
    fShowTypeAlert = (title, text, icon, element, clear) => {
        Swal.fire({
            title: title, text: text, icon: icon,
            showClass: { popup: 'animated fadeInDown faster' }, hideClass: { popup: 'animated fadeOutUp faster' },
            confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
        }).then((acepta) => {
            $("html, body").animate({ scrollTop: $(`#${element.id}`).offset().top - 50 }, 1000);
            if (clear == 1) {
                setTimeout(() => {
                    element.focus();
                    setTimeout(() => { element.value = ""; }, 300);
                }, 1200);
            } else if (clear == 2) {
                setTimeout(() => { element.focus(); }, 1200);
            }
        });
    }

    // Funcion que muestra el apartado de descarga del archivo
    fShowContentDownloadFile = (element, folder, file) => {
        element.innerHTML += `
                <div class="card border-left-success shadow h-100 py-2 animated fadeIn">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-success text-uppercase mb-1">Completado: ${file}</div>
                                <div class="row no-gutters align-items-center">
                                    <div class="col-auto">
                                        <div class="h5 mb-0 mr-3 font-weight-bold text-gray-800">100%</div>
                                    </div>
                                    <div class="col">
                                        <div class="progress progress-sm mr-2">
                                            <div class="progress-bar bg-success" role="progressbar" style="width: 100%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto">
                                <a title="Descargar" href="/Content/Reportes/${folder}/${file}" download="${file}"><i class="fas fa-download fa-2x text-gray-300"></i></a>
                            </div>
                        </div>
                    </div>
                </div>`;
    }

    fShowContentNoDataReport = (element) => {
        element.innerHTML += `
            <div class="card border-left-warning shadow h-100 py-2 animated fadeIn">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-warning text-uppercase mb-1">Atención!</div>
                                <div class="row no-gutters align-items-center">
                                    <div class="col">
                                        <p>No se encontraron registros</p>
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto">
                                <i class="fas fa-exclamation-triangle fa-2x text-warning"></i>
                            </div>
                        </div>
                    </div>
                </div>`;
    }

    // FUNCION QUE GENERA EL REPORTE DE CATALOGOS
    fGenerateReportCatalogs = () => {
        contentGenerateRepCatalogs.innerHTML = "";
        document.getElementById('img-reports').style.animation = "";
        document.getElementById('title-gr').textContent = "";
        try {
            if (typeReportselectCatalogs.value != "none" && typeReportselectCatalogs != "") {
                const dataSend = { report: String(typeReportselectCatalogs.value) };
                $.ajax({
                    url: "../Reportes/CatalogsGeneral",
                    type: "POST",
                    data: dataSend,
                    beforeSend: () => {
                        btnGenerateReportCatalogs.disabled = true;
                        typeReportselectCatalogs.disabled  = true;
                        document.getElementById('img-reports').style.animation = "float 3s ease-out infinite";
                        document.getElementById('title-gr').textContent = "Generando reporte, espere...";
                    }, success: (data) => {
                        console.log(data);
                        setTimeout(() => {
                            if (data.Bandera === true && data.MensajeError === "none") {
                                if (data.Rows > 0) {
                                    fShowContentDownloadFile(contentGenerateRepCatalogs, data.Folder, data.Archivo);
                                    document.getElementById('title-gr').textContent = "Reporte generado!";
                                    btnGenerateReportCatalogs.disabled = false;
                                    typeReportselectCatalogs.disabled  = false;
                                    typeReportselectCatalogs.value = "none";
                                } else {
                                    fShowContentNoDataReport(contentGenerateRepCatalogs);
                                }
                            } else {
                                alert('Algo fallo al realizar el reporte');
                                location.reload();
                            }
                        }, 4000);
                    }, error: (jqXHR, exception) => {
                        fcaptureaerrorsajax(jqXHR, exception);
                    }
                });
            } else {
                fShowTypeAlert('Atención', 'Selecciona una opcion de catalogo', 'warning', typeReportselectCatalogs, 2);
            }
        } catch (error) {
            if (error instanceof RangeError) {
                console.error('RangeError: ', error.message);
            } else if (error instanceof TypeError) {
                console.error('TypeError: ', error.message);
            } else if (error instanceof EvalError) {
                console.error('EvalError: ', error.message);
            } else {
                console.error('Error: ', error);
            }
        }
    }

    btnGenerateReportCatalogs.addEventListener('click', fGenerateReportCatalogs);

});