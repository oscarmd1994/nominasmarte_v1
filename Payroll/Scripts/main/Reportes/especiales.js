$(function () {

    const yearSearch    = document.getElementById('yearSearch');
    const btnYearSearch = document.getElementById('btnYearSearch');
    const typePeriodSearch = document.getElementById('typePeriodSearch');
    const periodSearch     = document.getElementById('periodSearch');
    const btnGenerateReportSearch = document.getElementById('btnGenerateReportSearch');
    const date = new Date();
    const accordionExample = document.getElementById('accordionExample');

    typePeriodSearch.disabled = true;
    periodSearch.disabled     = true;
    btnGenerateReportSearch.disabled = true;
    yearSearch.value = date.getFullYear();

    //$("#collapseOne").collapse('show');

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

    // Funcion que realiza la busqueda de los periodos en el año de busqueda
    fSearchPeriodsYear = () => {
        typePeriodSearch.innerHTML = `<option value="none">Selecciona</option>`;
        typePeriodSearch.disabled = true;
        accordionExample.innerHTML = "";
        try {
            if (yearSearch.value != "" && yearSearch.value.length == 4) {
                const dataSend = { yearSearch: parseInt(yearSearch.value) };
                $.ajax({
                    url: "../Reportes/SearchPeriodsYear",
                    type: "POST",
                    data: dataSend,
                    beforeSend: () => {

                    }, success: (request) => {
                        console.group("Periodos por año");
                        console.log(request);
                        console.groupEnd();
                        if (request.Bandera == true) {
                            typePeriodSearch.disabled = false;
                            for (let i = 0; i < request.Datos.length; i++) {
                                typePeriodSearch.innerHTML += `<option value="${request.Datos[i].iTipoPeriodo}">${request.Datos[i].sValor.toUpperCase()}</option>`;
                                let idDynamic = `#collapse${request.Datos[i].iTipoPeriodo}`;
                                accordionExample.innerHTML += `
                                    <div class="card">
                                        <div class="card-header" id="headingOne">
                                            <h2 class="mb-0">
                                                <button class="btn btn-link btn-block text-left" type="button" data-toggle="collapse" data-target="${idDynamic}" aria-expanded="true" aria-controls="collapseOne">
                                                    <i class="fas fa-building mr-2"></i> Empresas del Periodo ${request.Datos[i].sValor}
                                                </button>
                                            </h2>
                                        </div>
                                        <div id="collapse${request.Datos[i].iTipoPeriodo}" class="collapse" aria-labelledby="headingOne" data-parent="#accordionExample">
                                        </div>
                                    </div>
                                `;
                            }
                            setTimeout(() => { typePeriodSearch.focus(); }, 1500);
                            periodSearch.disabled = false;
                            btnGenerateReportSearch.disabled = false;
                        } else {
                            fShowTypeAlert("Atención!", "No se encontraron registros con el parametro indicado", "warning", yearSearch, 2);
                            typePeriodSearch.disabled = true;
                            periodSearch.disabled     = true;
                            btnGenerateReportSearch.disabled = true;
                        }
                    }, error: (jqXHR, exception) => {
                        console.error(jqXHR, exception);
                    }
                });
            } else {
                fShowTypeAlert("Atención!", "Ingresa un año valido", "warning", yearSearch, 2);
            }
        } catch (error) {
            if (error instanceof EvalError) {
                console.error('EvalError: ', error.message);
            } else if (error instanceof RangeError) {
                console.error('RangeError: ', error.message);
            } else if (error instanceof TypeError) {
                console.error('TypeError: ', error.message)
            } else {
                console.error('Error: ', error)
            }
        }
    }

    btnYearSearch.addEventListener('click', fSearchPeriodsYear);

    // Funcion que busca las empresas de acuerdo al periodo
    fSearchBusinessPeriod = () => {
        const paramId = "collapse";
        document.getElementById(paramId + typePeriodSearch.value).innerHTML = "";
        try {
            const dataSend = {
                yearSearch: parseInt(yearSearch.value),
                typePeriodSearch: parseInt(typePeriodSearch.value)
            };
            $.ajax({
                url: "../Reportes/SearchBusinessPeriod",
                type: "POST",
                data: dataSend,
                beforeSend: () => {

                }, success: (request) => {
                    console.group('Empresas de acuerdo al periodo');
                    console.log(request);
                    $("#" + paramId + typePeriodSearch.value).collapse('show');
                    if (request.Bandera == true) {
                        document.getElementById(paramId + typePeriodSearch.value).innerHTML = `<div class="p-2">`;
                        for (let i = 0; i < request.Datos.length; i++) {
                            document.getElementById(paramId + typePeriodSearch.value).innerHTML += `<span class="badge badge-primary p-2 m-1 shadow-lg">
                               [${request.Datos[i].iIdEmpresa}] - ${request.Datos[i].sNombreEmpresa}
                            </span>`;
                        }
                        document.getElementById(paramId + typePeriodSearch.value).innerHTML += `<br/><br/></div>`;
                        setTimeout(() => { periodSearch.focus(); }, 1000);
                    } else {
                        fShowTypeAlert("Atención!", "No se encontraron registros con el parametro indicado", "warning", typePeriodSearch, 2);
                    }
                    console.groupEnd();
                }, error: (jqXHR, exception) => {
                    console.error(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof EvalError) {
                console.error('EvalError: ', error.message);
            } else if (error instanceof RangeError) {
                console.error('RangeError: ', error.message);
            } else if (error instanceof TypeError) {
                console.error('TypeError: ', error.message)
            } else {
                console.error('Error: ', error)
            }
        }
    }

    typePeriodSearch.addEventListener('change', fSearchBusinessPeriod);

    // Funcion que genera el reporte de acuerdo al tipo de periodo seleccionado

    fGenerateReportParamsSearch = () => {
        try {
            if (yearSearch.value != "" && yearSearch.value.length == 4) {
                if (typePeriodSearch.value != "none" && periodSearch.value != "" && periodSearch.value >= 0) {
                    const dataSend = { yearSearch: parseInt(yearSearch.value), typePeriodSearch: parseInt(typePeriodSearch.value), periodSearch: parseInt(periodSearch.value) };
                    $.ajax({
                        url: "../Reportes/GenerateReportParamsSearch",
                        type: "POST",
                        data: dataSend,
                        beforeSend: () => {

                        }, success: (request) => {
                            console.group('Reporte generado por periodos');
                            console.log(request);
                            console.groupEnd();
                        }, error: (jqXHR, exception) => {
                            console.error(jqXHR, exception);
                        }
                    });
                } else {
                    fShowTypeAlert("Atención!", "Selecciona un tipo de periodo y captura un periodo valido", "warning", typePeriodSearch, 2);
                }
            } else {
                fShowTypeAlert("Atención!", "Completa de forma correcta el campo año", "warning", yearSearch, 2);
            }
        } catch (error) {
            if (error instanceof EvalError) {
                console.error('EvalError: ', error.message);
            } else if (error instanceof RangeError) {
                console.error('RangeError: ', error.message);
            } else if (error instanceof TypeError) {
                console.error('TypeError: ', error.message)
            } else {
                console.error('Error: ', error)
            }
        }
    }

    btnGenerateReportSearch.addEventListener('click', fGenerateReportParamsSearch);

});