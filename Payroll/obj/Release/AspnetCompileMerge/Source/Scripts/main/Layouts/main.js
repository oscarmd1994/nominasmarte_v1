$(function () {

    /*
     * Constantes
     */


    const btnCheckFileLayout = document.getElementById('btn-chek-file-layout');
    const fileUploadLayout   = document.getElementById('file-upload-layout');
    const typeLayout         = document.getElementById('type-layout');
    const contentDownloadExample = document.getElementById('content-download-example');
    const contentMessageLayout   = document.getElementById('content-message-layout');
    const contentDetailsValidation = document.getElementById('content-details-validation');

    btnCheckFileLayout.disabled = true;
    fileUploadLayout.disabled  = true;

    /*
     * Funciones
     */

    fCaptureErrorTryCatch = (error) => {
        if (error instanceof EvalError) {
            console.error('EvalError: ', error.message);
        } else if (error instanceof RangeError) {
            console.error('RangeError: ', error.message);
        } else if (error instanceof TypeError) {
            console.error('TypeError: ', error.message);
        } else {
            console.error('Error: ', error);
        }
    }

    fLoadButtonDownloadExample = (paramroute, paramname) => {
        fileUploadLayout.disabled = false;
        return `
            <a class="btn animated fadeInDown btn-sm btn-block btn-primary text-white" 
                href="${paramroute}" download="${paramname}">
                <i class="fas fa-download mr-2"></i> Descargar ejemplo
            </a>
        `;
    }

    fLoadExampleLayout = () => {
        if (typeLayout.value == "posts") { 
            contentDownloadExample.innerHTML = fLoadButtonDownloadExample('/Content/Layouts/EJEMPLO_LAYOUT_PUESTOS.xlsx', 'EJEMPLO_LAYOUT_PUESTOS.xlsx');
        } else if (typeLayout.value == "accountBank") {
            contentDownloadExample.innerHTML = fLoadButtonDownloadExample('/Content/Layouts/EJEMPLO_LAYOUT_CAMBIOS_CUENTAS.xlsx', 'EJEMPLO_LAYOUT_CAMBIOS_CUENTAS.xlsx');
        } else if (typeLayout.value == "dataPayroll") {
            contentDownloadExample.innerHTML = fLoadButtonDownloadExample('/Content/Layouts/EJEMPLO_LAYOUT_DATOS_NOMINA.xlsx', 'EJEMPLO_LAYOUT_DATOS_NOMINA.xlsx');
        } else {
            fileUploadLayout.disabled   = true;
            btnCheckFileLayout.disabled = true;
            fileUploadLayout.value      = "";
            contentDownloadExample.innerHTML = "";
        }
    }

    typeLayout.addEventListener('change', fLoadExampleLayout);

    fValidateFileSelect = () => {
        const nameFile  = fileUploadLayout.files[0].name;
        const arrayFile = nameFile.split(".");
        const extValid = "xlsx";
        if (arrayFile[1] == extValid) {
            btnCheckFileLayout.disabled     = false;
            contentMessageLayout.innerHTML  = "";
        } else {
            btnCheckFileLayout.disabled     = true;
            contentMessageLayout.innerHTML  = `
                <div class="alert alert-danger animated fadeInDown" role="alert">
                    <small class="">El archivo ${arrayFile[0]} con extensión .${arrayFile[1]} no es valido</small>
                </div>
            `;
        }
    }

    fileUploadLayout.addEventListener('change', fValidateFileSelect);

    fCardValidationsSpreadSheet = (paramflag, parammessage, paramanimated) => {
        let color = 'success';
        if (!paramflag) {
            color = 'danger';
        }
        return `
            <div class="col-md-4 animated ${paramanimated}">
                <div class="card shadow rounded mb-3">
                    <div class="card-header text-${color}"> 
                        <i class="fas fa-info mr-2"></i> Nombre de hoja</div>
                    <div class="card-body text-${color}">
                        <p class="card-text">${parammessage}</p>
                    </div>
                </div>
            </div>
        `;
    }

    fCheckFileLayout = () => {
        try {
            contentDetailsValidation.innerHTML = "";
            const nameFile = fileUploadLayout.files[0].name;
            const arrayFile = nameFile.split(".");
            const extValid = "xlsx";
            if (extValid == arrayFile[1]) {
                const selectFile = ($("#file-upload-layout"))[0].files[0];
                let dataString = new FormData();
                dataString.append("fileUpload", selectFile);
                dataString.append("typeFile", typeLayout.value);
                dataString.append("continueLoad", 0);
                let url = ""
                if (typeLayout.value == "posts") {
                    url = "CheckFileLayoutPosts";
                } else if (typeLayout.value == "accountBank") {
                    url = "CheckFileLayoutAccountBank";
                } else if (typeLayout.value == "dataPayroll") {
                    url = "CheckFileLayoutDataPayroll";
                }
                if (typeLayout.value == "posts" || typeLayout.value == "accountBank" || typeLayout.value == "dataPayroll") {
                    $.ajax({
                        url: "/Layouts/" + url,
                        type: "POST",
                        data: dataString,
                        contentType: false,
                        processData: false,
                        async: false,
                        beforeSend: () => {

                        }, success: (request) => {
                            console.group('Validacion de archivo');
                            console.log(request);
                            console.groupEnd();
                            $("html, body").animate({ scrollTop: $(`#${contentDetailsValidation.id}`).offset().top - 50 }, 1000);
                            if (request.GuardaArchivo) {
                                if (request.ValidacionHoja) {
                                    if (request.ValidacionDatos) {
                                        if (!request.BanderaError) {
                                            contentDetailsValidation.innerHTML = `
                                                <div class="alert alert-light col-md-8 offset-2" role="alert">
                                                    <h4 class="alert-heading text-center text-success"> 
                                                        <i class="fas fa-check-circle mr-2"></i> Importación exitosa!
                                                    </h4>
                                                    <hr/>
                                                    <div class="container-fluid">
                                                        <div class="card shadow p-2">
                                                            <div class="card-body">
                                                                <ul class="list-group">
                                                                    <li class="list-group-item d-flex justify-content-between 
                                                                        align-items-center">
                                                                        Registros indicados
                                                                        <span class="badge badge-primary badge-pill">${request.Cantidad}</span>
                                                                    </li>
                                                                    <li class="list-group-item d-flex justify-content-between 
                                                                        align-items-center">
                                                                        Registros correctos
                                                                        <span class="badge badge-success badge-pill">${request.Correctos}</span>
                                                                    </li>
                                                                </ul>
                                                            </div>
                                                            <div class="card-footer text-right">
                                                                <button class="btn btn-secondary btn-sm" onclick="fRestartOptions();">
                                                                    <i class="fas fa-undo mr-2"></i> Reiniciar opciones
                                                                </button>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            `;
                                        } else {
                                            contentDetailsValidation.innerHTML = `
                                                <div class="alert alert-light col-md-8 offset-2" role="alert">
                                                    <h4 class="alert-heading text-center text-warning"> 
                                                        <i class="fas fa-check-circle mr-2"></i> Resultado de la importación
                                                    </h4>
                                                    <hr/>
                                                    <div class="container-fluid">
                                                        <div class="card shadow p-2">
                                                            <div class="card-body">
                                                                <ul class="list-group">
                                                                    <li class="list-group-item d-flex justify-content-between 
                                                                            align-items-center">
                                                                        Registros indicados
                                                                        <span class="badge badge-primary badge-pill">${request.Cantidad}</span>
                                                                    </li>
                                                                    <li class="list-group-item d-flex justify-content-between 
                                                                            align-items-center">
                                                                        Registros correctos
                                                                        <span class="badge badge-success badge-pill">${request.Correctos}</span>
                                                                    </li>
                                                                    <li class="list-group-item d-flex justify-content-between 
                                                                            align-items-center">
                                                                        Registros con error
                                                                        <span class="badge badge-danger badge-pill">${request.Errores}</span>
                                                                    </li>
                                                                </ul>
                                                            </div>
                                                            <div class="card-footer text-right">
                                                                <a href="/Content/LayoutsLog/${request.Archivo}" 
                                                                    download="${request.Archivo}" class="btn btn-secondary btn-sm">
                                                                    <i class="fas fa-download mr-2"></i> Descargar LOG
                                                                </a>
                                                                <button class="btn btn-secondary btn-sm" onclick="fRestartOptions();">
                                                                    <i class="fas fa-undo mr-2"></i> Reiniciar opciones
                                                                </button>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            `;
                                        }
                                    } else {
                                        let html = "";
                                        for (var i = 0; i < request.Validaciones.lLogs.length; i++) {
                                            let tdMessage = "";
                                            let logs = request.Validaciones.lLogs[i].sMensaje.split('[*]');
                                            for (var t = 0; t < logs.length; t++) {
                                                if (logs[t].trim() != "") {
                                                    tdMessage += `<li class="list-group-item"> <i class="fas fa-circle text-danger mr-2"></i> ${logs[t]}</li>`;
                                                }
                                            }
                                            html += `<tr> <td class="text-center">${request.Validaciones.lLogs[i].iFilaError}</td>
                                                        <td class="p-0">
                                                            <ul class="list-group list-group-flush">${tdMessage}</ul>
                                                        </td> 
                                                    </tr>`;
                                        }
                                        contentDetailsValidation.innerHTML = `
                                            <table class="table">
                                                <thead>
                                                    <tr>
                                                        <th>Fila</th>
                                                        <th>Mensaje validación</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    ${html}
                                                </tbody>
                                            </table>
                                        `;
                                        fileUploadLayout.value = "";
                                    }
                                } else {
                                    const data = request.ValidacionesHoja;
                                    contentDetailsValidation.innerHTML += fCardValidationsSpreadSheet(data.VNombreHoja.bBandera, data.VNombreHoja.sMensaje, 'fadeIn');
                                    contentDetailsValidation.innerHTML += fCardValidationsSpreadSheet(data.VNombreCodigo.bBandera, data.VNombreCodigo.sMensaje, 'fadeInLeft delay-1s');
                                    contentDetailsValidation.innerHTML += fCardValidationsSpreadSheet(data.VCantidadRegistros.bBandera, data.VCantidadRegistros.sMensaje, 'fadeInLeft delay-2s');
                                    if (typeLayout.value == "dataPayroll") {
                                        contentDetailsValidation.innerHTML += fCardValidationsSpreadSheet(data.vTipoLayout.bBandera, data.vTipoLayout.sMensaje, 'fadeInDown delay-3s');
                                    }
                                    fileUploadLayout.value = "";
                                }
                            } else {
                                console.log('Error al guardar el archivo');
                            }
                        }, error: (jqXHR, exception) => {
                            console.error(jqXHR, exception);
                        }
                    });
                } else {
                    alert('Accion invalida');
                }
            } else {
                btnCheckFileLayout.disabled    = true;
                contentMessageLayout.innerHTML = `
                    <div class="alert alert-danger animated fadeInDown" role="alert">
                        <small class="">El archivo ${arrayFile[0]} con extensión .${arrayFile[1]} no es valido</small>
                    </div>
                `;
            }
        } catch (error) {
            fCaptureErrorTryCatch(error);
        }
    }

    btnCheckFileLayout.addEventListener('click', () => {
        if (fileUploadLayout.value != "") {
            fCheckFileLayout();
        } else {
            alert('Selecciona un archivo');
        }
    });

    fRestartOptions = () => {
        $("html, body").animate({ scrollTop: $(`#${fileUploadLayout.id}`).offset().top - 50 }, 1000);
        contentDetailsValidation.innerHTML = "";
        fileUploadLayout.value = "";
        btnCheckFileLayout.disabled = true;
        typeLayout.value = "none";
    }

});