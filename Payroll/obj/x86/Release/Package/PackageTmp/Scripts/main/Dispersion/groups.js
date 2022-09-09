$(function () {

    localStorage.removeItem("bankGroup");

    const year = document.getElementById('yeardis-banksgroup');
    const typePeriod = document.getElementById('typeperiod-banksgroup');
    const period = document.getElementById('periodis-banksgroup');
    const buttonClearSettings = document.getElementById('button-clearsettings');

    fLoadCards = () => {
        try {
            $.ajax({
                url: "../ConfigDataBank/LoadDataTableBanks",
                type: "POST",
                data: {},
                success: (data) => {
                    if (data.Bandera == true && data.MensajeError == "none") {
                        const dataB = data.DatosBancos;
                        if (dataB.length > 0) {
                            let cards = '';
                            for (var i = 0; i < dataB.length; i++) {
                                if (dataB[i].iIdBanco == 12 || dataB[i].iIdBanco == 14 || dataB[i].iIdBanco == 44 || dataB[i].iIdBanco == 72) {
                                    let status = (dataB[i].sCancelado == 'False') ? "<span class='badge badge-success ml-2'>Activo</span>" : "<span class='badge badge-danger ml-2'>Cancelado</span>";
                                    let action = (dataB[i].sCancelado == 'False') ? `onclick='fConfigureBank(${dataB[i].iIdBancoEmpresa}, "${dataB[i].sNombreBanco}");'` : "";
                                    let details = (dataB[i].sCancelado == 'False') ? `onclick='fViewDetailsBankGroups(${dataB[i].iIdBancoEmpresa}, "${dataB[i].sNombreBanco}");'` : "";
                                    let deploy = (dataB[i].sCancelado == 'False') ? `onclick='fDeployDepositsGroup(${dataB[i].iIdBancoEmpresa}, ${dataB[i].iCg_tipo_dispersion}, "${dataB[i].sNombreBanco}");'` : "";
                                    cards += `
                                        <div class='col-md-6 p-1'>
                                            <div class='card shadow rounded animated fadeInUp'>
                                                <h6 class='text-center text-primary mt-3 font-weight-bold'><i class='fas fa-university mr-2'></i> ${dataB[i].sNombreBanco}</h6>
                                                <hr class='mt-2'/>
                                                <div class='card-body pt-0'>
                                                    Estado: ${status}
                                                </div>
                                                <div class='card-footer text-right'>
                                                    <button class='btn btn-sm btn-primary' type='button' ${action} title='Configurar'><i class='fas fa-cogs'></i> Configurar</button>
                                                    <button class='btn btn-sm btn-primary' type='button' ${details} title='Ver detalles'><i class='fas fa-eye'></i> Detalles</button>
                                                    <button class='btn btn-sm btn-primary' type='button' ${deploy} title='Desplegar'><i class='fas fa-play-circle'></i> Generar archivos</button>
                                                </div>
                                            </div>
                                        </div>
                                    `;
                                }
                            }
                            document.getElementById('content-banks-groups').innerHTML = cards;
                        }
                    } else {
                        document.getElementById('content-banks-groups').innerHTML = `
                            <div class="col-md-8 offset-2">
                                <div class="alert alert-info" role="alert">
                                  <h4 class="alert-heading text-center">Atención!</h4>
                                  <hr>
                                  <p class="text-center">No existen bancos disponibles para este tipo de configuración</p>
                                  <hr>
                                  <p class="mb-0 text-center font-weight-bold">Asigne un banco interbancario valido para usar este apartado.</p>
                                </div>
                            </div>
                        `;
                        year.disabled = true;
                        period.disabled = true;
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            fCaptureErrorTryCatch(error);
        }
    }

    fLoadCards();

    fConfigureBank = (keyBankBusiness, paramname) => {
        try {
            if (keyBankBusiness > 0) {
                const periodD = period.value;
                const typePeriodD = typePeriod.value;
                const yearD = year.value;
                if (periodD != "" && typePeriodD != "" && yearD != "") {
                    $("#modal-banksgroups").modal("show");
                    $.ajax({
                        url: '../DispersionGroups/LoadBanksAvailable',
                        type: 'POST',
                        data: { year: parseInt(yearD), period: parseInt(periodD), typePeriod: parseInt(typePeriodD) },
                        beforeSend: () => {

                        }, success: (request) => {
                            let cards = "";
                            document.getElementById('name-bank-group').textContent = paramname;
                            localStorage.setItem("bankGroup", paramname);
                            if (request.Bandera == true && request.Error == "none") {
                                for (let i = 0; i < request.Data.length; i++) {
                                    cards += `
                                        <div class="col-md-4">
                                            <div class="card p-2 ">
                                                <div class="text-right">
                                                    <i class="fas fa-plus fa-lg text-success" title="Asignar banco a ${paramname}" style="cursor:pointer;" onclick="fAddRemoveBankGroup(${keyBankBusiness},${request.Data[i].iIdBanco}, 1);"></i>
                                                </div>
                                                <h6 class="text-center card-header rounded shadow"> ${request.Data[i].sBanco} </h6>
                                                <span class="badge badge-primary p-1 mt-2">
                                                    Total: $ ${request.Data[i].sImporte}
                                                </span>
                                                <span class="badge badge-primary p-1 mt-2">
                                                    Depositos: ${request.Data[i].iDepositos}
                                                </span>
                                            </div>
                                        </div>
                                    `;
                                }
                                document.getElementById('content-banks-available').innerHTML = cards;
                            } else {
                                document.getElementById('content-banks-available').innerHTML = `
                                    <div class="col-md-8 offset-2">
                                        <div class="alert alert-warning text-center" role="alert">
                                          <b>No Existen bancos disponibles, todos han sido asignados.</b>
                                        </div>
                                    </div>
                                `;
                            }
                        }, error: (jqXHR, exception) => {
                            fcaptureaerrorsajax(jqXHR, exception);
                        }
                    });
                } else {
                    alert('Los parametros del periodo no pueden ir vacíos');
                }
            } else {
                alert('Accion invalida');
                location.reload();
            }
        } catch (error) {
            fCaptureErrorTryCatch(error);
        }
    }

    fViewDetailsBankGroups = (keyBankBusiness, paramname) => {
        try {
            if (keyBankBusiness > 0) {
                const periodD = period.value;
                const typePeriodD = typePeriod.value;
                const yearD = year.value;
                if (periodD != "" && typePeriodD != "" && yearD != "") {
                    $("#modal-detailsgroups").modal("show");
                    $.ajax({
                        url: '../DispersionGroups/ViewDetails',
                        type: 'POST',
                        data: { keyBankBusiness: parseInt(keyBankBusiness), year: parseInt(yearD), period: parseInt(periodD), typePeriod: parseInt(typePeriodD) },
                        beforeSend: () => {

                        }, success: (request) => {
                            let cards = "";
                            document.getElementById('name-bank-detailsgroups').textContent = paramname;
                            localStorage.setItem("bankGroup", paramname);
                            if (request.Bandera == true && request.Error == "none") {
                                for (let i = 0; i < request.Data.length; i++) {
                                    cards += `
                                    <div class="col-md-4">
                                        <div class="card p-2 ">
                                            <div class="text-right">
                                                <i class="fas fa-minus-circle fa-lg text-danger" title="Remover banco a ${paramname}" style="cursor:pointer;" onclick="fAddRemoveBankGroup(${keyBankBusiness},${request.Data[i].iIdBanco}, 0);"></i>
                                            </div>
                                            <h6 class="text-center card-header rounded shadow"> ${request.Data[i].sBanco} </h6>
                                            <span class="badge badge-primary p-1 mt-2">
                                                Total: $ ${request.Data[i].sImporte}
                                            </span>
                                            <span class="badge badge-primary p-1 mt-2">
                                                Depositos: ${request.Data[i].iDepositos}
                                            </span>
                                        </div>
                                    </div>
                                `;
                                }
                                document.getElementById('content-banks-detailsgroups').innerHTML = cards;
                                document.getElementById('content-banks-detailsinfo').innerHTML = `
                                    <div class="col-md-12">
                                        <div class="card shadow">
                                            <h5 class="card-header text-center"> Detalles grupo </h5>
                                            <div class="card-body">
                                                <ul class="list-group border-0">
                                                    <li class="list-group-item d-flex justify-content-between align-items-center">
                                                        <span><i class="fas fa-calculator mr-2 text-success"></i>Depositos:</span>
                                                        <span class="badge badge-success badge-pill">${request.Depositos}</span>
                                                    </li>
                                                    <li class="list-group-item d-flex justify-content-between align-items-center">
                                                        <span><i class="fas fa-money-bill-wave mr-2 text-success"></i>Total a pagar:</span>
                                                        <span class="badge badge-success badge-pill">$ ${request.Total}</span>
                                                    </li>
                                                    <li class="list-group-item justify-content-between align-items-center text-center">
                                                        <span class="badge badge-light text-success badge-pill">${request.TotalLetra}</span>
                                                    </li>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                `;
                            } else {
                                document.getElementById('content-banks-detailsgroups').innerHTML = "";
                                document.getElementById('content-banks-detailsinfo').innerHTML = "";
                                document.getElementById('content-banks-detailsgroups').innerHTML = `
                                    <div class="col-md-8 offset-2">
                                        <div class="alert alert-warning text-center" role="alert">
                                            <b>No Existen bancos asignados, dirigase a configuración.</b>
                                        </div>
                                    </div>
                                `;
                            }
                        }, error: (jqXHR, exception) => {
                            fcaptureaerrorsajax(jqXHR, exception);
                        }
                    });
                } else {
                    alert('Los parametros del periodo no pueden ir vacíos');
                }
            } else {
                alert('Accion invalida');
                location.reload();
            }
        } catch (error) {
            fCaptureErrorTryCatch(error);
        }
    }

    fAddRemoveBankGroup = (keyBankBusiness, keyBank, type) => {
        try {
            if (keyBankBusiness > 0 && keyBank > 0) {
                const dataSend = { keyBankBusiness: parseInt(keyBankBusiness), keyBank: parseInt(keyBank), status: type };
                $.ajax({
                    url: '../DispersionGroups/AddRemoveBankGroup',
                    type: 'POST',
                    data: dataSend,
                    beforeSend: () => { },
                    success: (request) => {
                        const nameBank = localStorage.getItem("bankGroup");
                        if (request.Bandera == true && request.Error == "none") {
                            const msg = (type == 1) ? "Banco asignado" : "Banco removido";
                            Swal.fire({
                                position: 'top-end',
                                icon: 'success',
                                title: msg,
                                showConfirmButton: false,
                                timer: 1500
                            })
                            if (type == 1) {
                                fConfigureBank(keyBankBusiness, nameBank);
                            } else {
                                fViewDetailsBankGroups(keyBankBusiness, nameBank);
                            }
                        } else {
                            alert('Ocurrio un problema al asignar el banco al grupo');
                        }
                    }, error: (jqXHR, exception) => {
                        fcaptureaerrorsajax(jqXHR, exception);
                    }
                });
            } else {
                alert('Acción invalida');
                location.reload();
            }
        } catch (error) {
            fCaptureErrorTryCatch(error);
        }
    }

    fDeployDepositsGroup = (keyBankBusiness, keyTypeDispersion, paramname) => {
        try {
            $("html, body").animate({ scrollTop: $('#content-loading-dispersiongroups').offset().top - 130 }, 1000);
            if (keyBankBusiness > 0) {
                const periodD = period.value;
                const typePeriodD = typePeriod.value;
                const yearD = year.value;
                if (periodD != "" && typePeriodD != "" && yearD != "") {
                    const dataSend = { keyBankBusiness: parseInt(keyBankBusiness), typeDispersion: parseInt(keyTypeDispersion), keyType: 1, period: parseInt(periodD), typePeriod: parseInt(typePeriodD), year: parseInt(yearD) };
                    $.ajax({
                        url: '../DispersionGroups/DeployDepositsGroup',
                        type: 'POST',
                        data: dataSend,
                        beforeSend: () => {
                            document.getElementById('content-loading-dispersiongroups').innerHTML = `
                                <div class="col-md-8 offset-2 text-center">
                                    <div class="spinner-border text-primary" style="width: 2.5rem; height: 2.5rem;" role="status">
                                        <span class="sr-only">Loading...</span>
                                    </div>
                                    <br />
                                    <b>Generando, espere por favor...</b>
                                </div>
                            `;
                        },
                        success: (request) => {
                            if (request.Cantidad > 0) {
                                if (request.Bandera == true && request.MensajeError == "none") {
                                    let listFiles = "<h5 class='text-center text-success mb-3'>Archivos dentro del .ZIP</h5><ul class='list-group'>";
                                    for (var i = 0; i < request.Archivos.length; i++) {
                                        listFiles += `
                                            <li class="list-group-item d-flex justify-content-between align-items-center">
                                                <span><i class='fas fa-file mr-3 text-success'></i>${request.Archivos[i]}</span>
                                                <span class="badge badge-primary p-2" title="Descargar"><a class='text-white' href='/DispersionTXT/${request.FolderAnio}/${request.FolderTxt}/${request.Zip}/${request.Archivos[i]}' download='${request.Archivos[i]}'><i class='fas fa-file-download'></i></a></span>
                                            </li>
                                        `;
                                    }
                                    listFiles += "</ul>";
                                    setTimeout(() => {
                                        document.getElementById('content-loading-dispersiongroups').innerHTML = `
                                        <div class="col-md-10 offset-1">
                                            <div class="card border-left-success shadow h-100 py-2 animated fadeIn">
                                                <div class="card-header">
                                                    <h5 class="card-title text-center">Archivos INTERBANCARIOS de ${paramname}</h5>
                                                </div>
                                                <div class="card-body">
                                                    <div class="row no-gutters align-items-center">
                                                        <div class="col mr-2">
                                                            <div class="text-xs font-weight-bold text-success text-uppercase mb-1">${request.Zip}.zip</div>
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
                                                            <a title="Descargar archivo ${request.Zip}.zip" id="btn-down-txt" download="${request.Zip}.zip" href="/DispersionZIP/${request.Anio}/${request.FolderGuardado}/${request.Zip}.zip" ><i class="fas fa-download fa-2x text-primary"></i></a>
                                                        </div>
                                                        <div class="col-md-10 offset-1 mt-4">
                                                            ${listFiles}
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>`;
                                    }, 2000);
                                } else {
                                    document.getElementById('content-loading-dispersiongroups').innerHTML = '';
                                    alert('Ocurrio un problema al generar los archivos de dispersion para el grupo: ' + paramname);
                                }
                            } else {
                                document.getElementById('content-loading-dispersiongroups').innerHTML = '';
                                alert('No existen bancos asignados a: ' + paramname);
                            }
                        }, error: (jqXHR, exception) => {
                            fcaptureaerrorsajax(jqXHR, exception);
                        }
                    });
                } else {
                    alert('Los parametros del periodo no pueden ir vacíos');
                }
            } else {
                alert('Acción invalida');
                location.reload();
            }
        } catch (error) {
            fCaptureErrorTryCatch(error);

        }
    }

    fClearSettings = () => {
        try {
            $.ajax({
                url: '../DispersionGroups/ClearSettings',
                type: 'POST',
                data: {},
                beforeSend: () => {
                    buttonClearSettings.disabled = true;
                }, success: (request) => {
                    if (request.Bandera && request.Error == 'none') {
                        Swal.fire({
                            position: 'top-end',
                            icon: 'success',
                            title: 'Configuración existente removida',
                            showConfirmButton: false,
                            timer: 1500
                        })
                    } else {
                        alert('Ocurrio un problema interno en la aplicación');
                    }
                    buttonClearSettings.disabled = false;
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            fCaptureErrorTryCatch(error);
        }
    }

    buttonClearSettings.addEventListener('click', fClearSettings);

});