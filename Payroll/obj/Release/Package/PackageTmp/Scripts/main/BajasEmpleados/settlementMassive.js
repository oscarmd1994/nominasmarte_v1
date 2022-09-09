$(function () {

    const fileUploadMasiveSettlement  = document.getElementById('file-upload-masive-settlement');
    const btnSaveFileMasiveSettlement = document.getElementById('btn-save-file-masive-settlement');
    const divShowLoadFileSettlement   = document.getElementById('div-show-load-file-settlement');

    const divContentLoadInfoSettlement1 = document.getElementById('div-content-load-info-settlement-1');
    const divContentLoadInfoSettlement2 = document.getElementById('div-content-load-info-settlement-2');


    document.getElementById('label-file-upload-settlement').style.cursor = "pointer";

    /*
     * FUNCIONES
     */

    // Funcion que captura los errores de ajax que se puedan generar
    fcaptureaerrorsajax = (jq, exc) => {
        let msg = "";
        if (jq.status === 0) {
            msg = "No conectado. \n Verifica tu conexión de red.";
        } else if (jq.status === 404) {
            msg = 'Página solicitada no encontrada. [404]';
        } else if (jq.status == 500) {
            msg = 'Error interno del servidor [500].';
        } else if (exc === 'parsererror') {
            msg = 'El análisis JSON solicitado falló.';
        } else if (exc === 'timeout') {
            msg = 'Error de tiempo de espera.';
        } else if (exc === 'abort') {
            msg = 'Solicitud de Ajax abortada.';
        } else {
            msg = 'Error no detectado.\n' + jq.responseText;
        }
        console.log(msg);
    }

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

    // Funcion que obtiene el valor del archivo seleccionado
    fSelectValueFile = () => {
        const nameFile = fileUploadMasiveSettlement.files[0].name;
        const arrNFile = nameFile.split(".");
        const extValid = "xlsx";
        if (extValid == arrNFile[1]) {
            document.getElementById('name-file-settlement-masive').innerHTML = `<i class="fas fa-file-excel mr-2"></i>` + nameFile;
            document.getElementById('name-file-settlement-masive').classList.add('fadeInDown');
            document.getElementById('name-file-settlement-masive').classList.remove('text-danger');
            document.getElementById('name-file-settlement-masive').classList.add('text-primary');
            $("html, body").animate({ scrollTop: $(`#${divShowLoadFile.id}`).offset().top - 50 }, 1000);
            btnSaveFileMasiveSettlement.disabled = false;
        } else {
            document.getElementById('name-file-settlement-masive').innerHTML = `El archivo con extension .${arrNFile[1]} no es valido.`;
            document.getElementById('name-file-settlement-masive').classList.remove('text-primary');
            document.getElementById('name-file-settlement-masive').classList.add('text-danger');
            fileUploadMasiveSettlement.value = "";
            btnSaveFileMasiveSettlement.disabled = true;
        }
    }

    fileUploadMasiveSettlement.addEventListener('change', fSelectValueFile);

    fUploadFileMasiveUpEmployees = () => {
        try {
            const valueInptFile = fileUploadMasiveSettlement.value;
            const allowedExtensions = /(.xlsx)$/i;
            if (valueInptFile != "") {
                if (!allowedExtensions.exec(valueInptFile)) {
                    fileUploadMasiveSettlement.value = "";
                    fShowTypeAlert("Atencion", "El archivo no es valido", "warning", fileUploadMasiveSettlement, 0);
                } else {
                    const selectFile = ($("#file-upload-masive-settlement"))[0].files[0];
                    let dataString = new FormData();
                    dataString.append("fileUpload", selectFile);
                    dataString.append("typeFile", "BAJA");
                    $.ajax({
                        url: "/MassiveUpsAndDowns/UploadFileSettlementMassive",
                        type: "POST",
                        data: dataString,
                        contentType: false,
                        processData: false,
                        async: false,
                        beforeSend: () => {
                            btnSaveFileMasiveSettlement.disabled = true;
                            fileUploadMasiveSettlement.disabled = true;
                            divShowLoadFileSettlement.classList.remove('fadeOut');
                            divShowLoadFileSettlement.classList.add('fadeIn');
                            divShowLoadFileSettlement.innerHTML = `
                                <div class="spinner-border text-primary" role="status">
                                  <span class="sr-only">Cargando...</span>
                                </div>
                                <h6 class="text-primary font-weight-bold mt-2">Cargando...</h6>
                            `;
                        }, success: (data) => {
                            console.log(data);
                            setTimeout(() => {
                                divShowLoadFileSettlement.classList.remove('fadeIn');
                                divShowLoadFileSettlement.classList.add('fadeOut');
                                setTimeout(() => { divShowLoadFileSettlement.innerHTML = ""; }, 500);
                                if (data.Bandera == true && data.MensajeError == "none" && data.Log == false) {
                                    $("html, body").animate({ scrollTop: $(`#${divContentLoadInfoSettlement1.id}`).offset().top - 50 }, 1000);
                                    setTimeout(() => {
                                        divContentLoadInfoSettlement1.classList.add('fadeInDown');
                                        divContentLoadInfoSettlement1.innerHTML = `
                                            <div class="card shadow p-2 border-left-primary">
                                                <b class="card-title text-center font-weight-bold text-success mt-3 mb-3"> <i class="fas fa-check-circle mr-2"></i> Carga correcta! </b>
                                                <hr class="mb-0 mt-0" />
                                                <p class="mt-0 text-center font-weight-bold text-info mt-3">Los datos estan siendo leídos. </p>
                                            </div>
                                        `;
                                        divContentLoadInfoSettlement2.classList.add('fadeInDown');
                                        divContentLoadInfoSettlement2.innerHTML = `
                                            <div class="card shadow p-2 border-right-primary">
                                                <b class="card-title text-center font-weight-bold text-success mt-3 mb-3"> <i class="fas fa-file-excel mr-2"></i> ${data.ArchivoCarga} </b>
                                                <hr class="mb-0 mt-0" />
                                                <p class="mt-0 text-center font-weight-bold text-info mt-3">Estado de la lectura: 
                                                    <span class="badge badge-info p-1" id="status-load">En proceso...</span>
                                                </p>
                                            </div>
                                        `;
                                        // Ejecutar funcion para la insercion
                                        setTimeout(() => {
                                            fReadDataFileMasiveDowns(String(data.ArchivoCarga), parseInt(data.Llave));
                                        }, 2000);
                                    }, 1000);
                                } else {
                                    setTimeout(() => {
                                        divContentLoadInfoSettlement1.classList.add('fadeInDown');
                                        divContentLoadInfoSettlement1.innerHTML = `
                                            <div class="card shadow p-2 border-left-primary">
                                                <b class="card-title text-center font-weight-bold text-danger mt-3 mb-3"> <i class="fas fa-times-circle mr-2"></i> Error de lectura </b>
                                                <hr class="mb-0 mt-0" />
                                                <p class="mt-0 text-center font-weight-bold text-info mt-3"> <i class="fas fa-laptop-code mr-2"></i> Contacte al área de TI </p>
                                            </div>
                                        `;
                                        divContentLoadInfoSettlement2.classList.add('fadeInDown');
                                        divContentLoadInfoSettlement2.innerHTML = `
                                            <div class="card shadow p-2 border-right-primary">
                                                <b class="card-title text-center font-weight-bold text-danger mt-3 mb-3"> <i class="fas fa-file-excel mr-2"></i> ${data.ArchivoCarga} </b>
                                                <hr class="mb-0 mt-0" />
                                                <p class="mt-0 text-center font-weight-bold text-info mt-3">Estado del registro: 
                                                    <span class="badge badge-danger p-1" id="status-load">Sin procesar...</span>
                                                </p>
                                            </div>
                                        `;
                                        btnSaveFileMasiveSettlement.disabled = false;
                                        fileUploadMasiveSettlement.disabled = false;
                                    }, 1000);
                                }
                            }, 2000);
                        }, error: (jqXHR, exception) => {
                            fcaptureaerrorsajax(jqXHR, exception);
                        }
                    });
                }
            } else {
                fShowTypeAlert("Atencion", "Selecciona un archivo", "warning", fileUploadMasiveSettlement, 0);
            }
        } catch (error) {
            if (error instanceof EvalError) {
                console.error('EvalError: ', error.message);
            } else if (error instanceof TypeError) {
                console.error('TypeError: ', error.message);
            } else if (error instanceof RangeError) {
                console.error('RangeError: ', error.message);
            } else {
                console.error('Error: ', error);
            }
        }
    }


});