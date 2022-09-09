//$(function () {

//    /*
//     * Declaracion de variables de los botones que cargan datos consecutivos
//     */
//    //const btnSearchNumNomina        = document.getElementById('btn-search-num-nomina');
//    const btnSearchNumPosicion = document.getElementById('btn-search-num-posicion');
//    const btnSearchTablePosicion = document.getElementById('btn-search-table-num-posicion');
//    const btnSearchDepartament = document.getElementById('btn-search-departamento');

//    const btnModalSearchPuesto = document.getElementById('btn-modal-search-puesto');
//    const btnCloseRegisterPbtn = document.getElementById('btn-close-registerpuesto-btn');
//    const icoCloseRegisterPbtn = document.getElementById('ico-close-registerpuesto-btn');

//    localStorage.removeItem('modalbtnpuesto');
//    btnModalSearchPuesto.addEventListener('click', () => { localStorage.setItem('modalbtnpuesto', 1); });
//    btnCloseRegisterPbtn.addEventListener('click', () => { localStorage.removeItem('modalbtnpuesto'); });
//    icoCloseRegisterPbtn.addEventListener('click', () => { localStorage.removeItem('modalbtnpuesto'); });

//    const btnModalSearchDepartament = document.getElementById('btn-modal-search-departament');
//    const btnCloseRegisterDbtn = document.getElementById('btn-close-registerdepart-btn');
//    const icoCloseRegisterDbtn = document.getElementById('ico-close-registerdepart-btn');

//    localStorage.removeItem('modalbtndepartament');
//    btnModalSearchDepartament.addEventListener('click', () => { localStorage.setItem('modalbtndepartament', 1); });
//    btnCloseRegisterDbtn.addEventListener('click', () => { localStorage.removeItem('modalbtndepartament'); });
//    icoCloseRegisterDbtn.addEventListener('click', () => { localStorage.removeItem('modalbtndepartament'); });

//    /*
//     * Ejecución de función que carga el numero de nómina 
//     */
//    floadnumnomina = (idparam) => {
//        try {
//            $.ajax({
//                url: "../CatalogsTables/LoadNumNomina",
//                type: "POST",
//                data: { keyemp: idparam },
//                success: (data) => {
//                    if (data.sMensaje == "success") {
//                        document.getElementById('numnom').value = data.iNominaSiguiente;
//                    } else {
//                        document.getElementById('numnom').value = 0;
//                    }
//                }, error: (jqxHR, exception) => {
//                    fcaptureaerrorsajax(jqxHR, exception);
//                }
//            });
//        } catch (error) {
//            if (error instanceof TypeError) {
//                console.log('TypeError ', error);
//            } else if (error instanceof RangeError) {
//                console.log('RangeError ', error);
//            } else if (error instanceof EvalError) {
//                console.log('EvalError ', error);
//            } else {
//                console.log('Error ', error);
//            }
//        }
//    }

//    //btnSearchNumNomina.addEventListener('click', () => {
//    //    floadnumnomina(5);
//    //});

//    /*
//     * Ejecución de funcion que carga el numero de posicion
//     */
//    floadnumposicion = () => {
//        try {
//            $.ajax({
//                url: "../CatalogsTables/LoadNumPosicion",
//                type: "POST",
//                data: {},
//                success: (data) => {
//                    if (data.sMensaje == "success") {
//                        document.getElementById('numplareg').value = data.iPosicionSiguiente;
//                    } else {
//                        document.getElementById('numplareg').value = 0;
//                    }
//                }, error: (jqxHR, exception) => {
//                    fcaptureaerrorsajax(jqxHR, exception);
//                }
//            });
//        } catch (error) {
//            if (error instanceof TypeError) {
//                console.log('TypeError ', error);
//            } else if (error instanceof RangeError) {
//                console.log('RangeError ', error);
//            } else if (error instanceof EvalError) {
//                console.log('EvalError ', error);
//            } else {
//                console.log('Error ', error);
//            }
//        }
//    }

//    freloadnumpos = () => {
//        if (document.getElementById('numpla').value != "") {
//            floadnumposicion();
//        }
//    }

//    //let timernumpos = setInterval(freloadnumpos, 10000);

//    //btnSearchNumPosicion.addEventListener('click', () => {
//    //    floadnumposicion();
//    //});

//    /*
//     * Declaracion de variables de los botones que muestran las modales 
//     */
//    const btnRegisterPuesto = document.getElementById('btnregisterpuesto');
//    const btnRegisterPuestobtn = document.getElementById('btnregisterpuestobtn');
//    const btnSavePuesto = document.getElementById('btnsavepuesto');
//    const btnEdiPuesto = document.getElementById('btnedipuesto');
//    const btnRegisterPosicion = document.getElementById('btnregisterpositions');
//    const btnSearchPuesto = document.getElementById('btn-search-puesto');
//    // Btns limpia formularios
//    const btnClearFieldsJob = document.getElementById('btn-clear-fields-job');
//    const icoClearFieldsJob = document.getElementById('ico-clear-fields-job');
//    /*
//     * Declaracion de variables de las tablas a mostrar
//     */
//    const dataBodyPuestos = document.getElementById('data-body-puestos');

//    /*
//     * Declaracion de variables de botones a registrar datos
//     */


//    /*
//     * Ejecución de la carga de datos de las tablas
//     */

//    /*
//     * Declaracion de variables de registro de nuevo puesto
//    */
//    const regcodpuesto = document.getElementById('regcodpuesto');
//    const regpuesto = document.getElementById('regpuesto');
//    const regdescpuesto = document.getElementById('regdescpuesto');
//    const proffamily = document.getElementById('proffamily');
//    const clasifpuesto = document.getElementById('clasifpuesto');
//    const regcolect = document.getElementById('regcolect');
//    const nivjerarpuesto = document.getElementById('nivjerarpuesto');
//    const perfmanager = document.getElementById('perfmanager');
//    const tabpuesto = document.getElementById('tabpuesto');

//    btnRegisterPuesto.addEventListener('click', () => {
//        $("#searchpuesto").modal('hide');
//        setTimeout(() => {
//            regcodpuesto.focus();
//        }, 1200);
//    });

//    btnRegisterPuestobtn.addEventListener('click', () => {
//        $("#searchpuestobtn").modal('hide');
//        setTimeout(() => {
//            regcodpuesto.focus();
//        }, 1200);
//    });

//    $('[data-toggle="tooltip"]').tooltip();

//    /*
//     * Funcion que limpia los campos de registrar un nuevo puesto 
//     */

//    fclearfieldsnewjob = () => {
//        regcodpuesto.value = "";
//        regpuesto.value = "";
//        regdescpuesto.value = "";
//        proffamily.value = "0";
//        clasifpuesto.value = "0";
//        regcolect.value = "0";
//        nivjerarpuesto.vale = "0";
//        perfmanager.value = "0";
//        tabpuesto.value = "0";
//    }

//    /*
//     * Funcion que carga los datos del select profesion familia 
//     */

//    floadproffamily = (state, type, keyprof, elementid) => {
//        try {
//            $.ajax({
//                url: "../CatalogsTables/JobFamily",
//                type: "POST",
//                data: { state: state, type: type, keyprof: keyprof },
//                success: (data) => {
//                    const quantity = data.length;
//                    if (quantity > 0) {
//                        for (let i = 0; i < data.length; i++) {
//                            elementid.innerHTML += `<option value="${data[i].iIdProfesionFamilia}">${data[i].sNombreProfesion}</option>`;
//                        }
//                    }
//                }, error: (jqXHR, exception) => {
//                    fcaptureaerrorsajax(jqXHR, exception);
//                }
//            });
//        } catch (error) {
//            if (error instanceof TypeError) {
//                console.log('TypeError ', error);
//            } else if (error instanceof RangeError) {
//                console.log('RangeError ', error);
//            } else if (error instanceof EvalError) {
//                console.log('EvalError ', error);
//            } else {
//                console.log('Error ', error);
//            }
//        }
//    }

//    floadproffamily(1, 'Active/Desactive', 0, proffamily);

//    /*
//     * Funcion que carga los datos del select etiqetas contables 
//     */

//    floadtagscont = (state, type, keytag, elementid) => {
//        try {
//            $.ajax({
//                url: "../CatalogsTables/TagsCont",
//                type: "POST",
//                data: { state: state, type: type, keytag: keytag },
//                success: (data) => {
//                    const quantity = data.length;
//                    if (quantity > 0) {
//                        for (let i = 0; i < data.length; i++) {
//                            elementid.innerHTML += `<option value="${data[i].iIdEtiquetaContable}">${data[i].sNombreEtiquetaContable}</option>`;
//                        }
//                    }
//                }, error: (jqXHR, exception) => {
//                    fcaptureaerrorsajax(jqXHR, exception);
//                }
//            });
//        } catch (error) {
//            if (error instanceof TypeError) {
//                console.log('TypeError ', error);
//            } else if (error instanceof RangeError) {
//                console.log('RangeError ', error);
//            } else if (error instanceof EvalError) {
//                console.log('EvalError ', error);
//            } else {
//                console.log('Error ', error);
//            }
//        }
//    }


//    /*
//    * Funcion que carga los datos del select nivel jerarquico
//    */

//    floadnivjer = (state, type, keyniv, elementid) => {
//        try {
//            $.ajax({
//                url: "../CatalogsTables/NivJerar",
//                type: "POST",
//                data: { state: state, type: type, keyniv: keyniv },
//                success: (data) => {
//                    const quantity = data.length;
//                    if (quantity > 0) {
//                        for (let i = 0; i < data.length; i++) {
//                            elementid.innerHTML += `<option value="${data[i].iIdNivelJerarquico}">${data[i].sNombreNivelJerarquico}</option>`;
//                        }
//                    }
//                }, error: (jqXHR, exception) => {
//                    fcaptureaerrorsajax(jqXHR, exception);
//                }
//            });
//        } catch (error) {
//            if (error instanceof TypeError) {
//                console.log('TypeError ', error);
//            } else if (error instanceof RangeError) {
//                console.log('RangeError ', error);
//            } else if (error instanceof EvalError) {
//                console.log('EvalError ', error);
//            } else {
//                console.log('Error ', error);
//            }
//        }
//    }

//    floadnivjer(1, 'Active/Desactive', 0, nivjerarpuesto);

//    /*
//     * Funcion que carga los datos del select perfomance manager 
//     */

//    floadperfman = (state, type, keyper, elementid) => {
//        try {
//            $.ajax({
//                url: "../CatalogsTables/PerfManager",
//                type: "POST",
//                data: { state: state, type: type, keyper: keyper },
//                success: (data) => {
//                    const quantity = data.length;
//                    if (quantity > 0) {
//                        for (let i = 0; i < data.length; i++) {
//                            elementid.innerHTML += `<option value="${data[i].iIdPerfomanceManager}">${data[i].sPerfomanceManager}</option>`;
//                        }
//                    }
//                }, error: (jqXHR, exception) => {
//                    fcaptureaerrorsajax(jqXHR, exception);
//                }
//            });
//        } catch (error) {
//            if (error instanceof TypeError) {
//                console.log('TypeError ', error);
//            } else if (error instanceof RangeError) {
//                console.log('RangeError ', error);
//            } else if (error instanceof EvalError) {
//                console.log('EvalError ', error);
//            } else {
//                console.log('Error ', error);
//            }
//        }
//    }

//    floadperfman(1, 'Active/Desactive', 0, perfmanager);

//    /*
//     * Funcion que carga los datos del select nivel de tabulador 
//     */


//    floadniveltab = (elementid) => {
//        try {
//            $.ajax({
//                url: "../Empleados/LoadTabs",
//                type: "POST",
//                data: { state: 1, type: 'Active/Desactive', keytab: 0 },
//                success: (data) => {
//                    const quantity = data.length;
//                    if (quantity > 0) {
//                        for (i = 0; i < data.length; i++) {
//                            elementid.innerHTML += `<option value="${data[i].iIdTabulador}">${data[i].sTabulador}</option>`;
//                        }
//                    } else {
//                        console.error('Ocurrio un problema al cargar');
//                    }
//                }, error: (jqXHR, exception) => {
//                    fcaptureaerrorsajax(jqXHR, exception);
//                }
//            });
//        } catch (error) {
//            if (error instanceof TypeError) {
//                console.log('TypeError ', error);
//            } else if (error instanceof RangeError) {
//                console.log('RangeError ', error);
//            } else if (error instanceof EvalError) {
//                console.log('EvalError ', error);
//            } else {
//                console.log('Error ', error);
//            }
//        }
//    }

//    floadniveltab(tabpuesto);

//    floadcataloggeneral = (element, state, type, keycol, catalog) => {
//        try {
//            $.ajax({
//                url: "../CatalogsTables/ClasifPuest",
//                type: "POST",
//                data: { state: state, type: type, keycla: keycol, catalog: catalog },
//                success: (data) => {
//                    const quantity = data.length;
//                    if (quantity > 0) {
//                        for (let i = 0; i < data.length; i++) {
//                            element.innerHTML += `<option value="${data[i].iId}">${data[i].sValor}</option>`;
//                        }
//                    }
//                }, error: (jqXHR, exception) => {
//                    fcaptureaerrorsajax(jqXHR, exception);
//                }
//            });
//        } catch (error) {
//            if (error instanceof TypeError) {
//                console.log('TypeError ', error);
//            } else if (error instanceof RangeError) {
//                console.log('RangeError ', error);
//            } else if (error instanceof EvalError) {
//                console.log('EvalError ', error);
//            } else {
//                console.log('Error ', error);
//            }
//        }
//    }
//    /*
//     * Funcion que carga los datos del select clasificacion del puesto 
//     */
//    floadcataloggeneral(clasifpuesto, 0, 'Active/Desactive', 0, 15);

//    /*
//     * Funcion que carga los datos del select colectivo
//     */
//    floadcataloggeneral(regcolect, 0, 'Active/Desactive', 0, 16);

//    /*
//     * Funcion que carga las claves del sat
//    */
//    //floadcataloggeneral(clvsat, 0, 'Active/Desactive', 0, 17);

//    /*
//     * Eventos que limpian el formulario de registro de nuevo puesto 
//     */
//    btnClearFieldsJob.addEventListener('click', fclearfieldsnewjob);
//    icoClearFieldsJob.addEventListener('click', fclearfieldsnewjob);

//    const spanish = {
//        "decimal": "",
//        "emptyTable": "No hay información",
//        "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
//        "infoEmpty": "Mostrando 0 t 0 of 0 Entradas",
//        "infoFiltered": "(Filtrado de _MAX_ total entradas)",
//        "infoPostFix": "",
//        "thousands": ",",
//        "lengthMenu": "Mostrar _MENU_ Entradas",
//        "loadingRecords": "Cargando...",
//        "processing": "Procesando...",
//        "search": "Buscar:",
//        "zeroRecords": "Sin resultados encontrados",
//        "paginate": {
//            "first": "Primero",
//            "last": "Ultimo",
//            "next": "<button class='ml-2 btn btn-sm btn-primary'>Siguiente </button>",
//            "previous": "<button class='mr-2 btn btn-sm btn-primary'>Anterior</button>"
//        }
//    };

//    const tableJobs = $("#table-puestos").DataTable({
//        responsive: true,
//        destroy: true,
//        ajax: {
//            method: "POST",
//            url: "../CatalogsTables/Job",
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            dataSrc: "data"
//        },
//        columns: [
//            { "data": "sCodigoPuesto" },
//            { "data": "sNombrePuesto" },
//            { "data": "sDescripcionPuesto" },
//            { "defaultContent": "<button type='button' data-toggle='tooltip' data-placement='left' title='Seleccionar' class='btn text-center btn-outline-primary shadow rounded'> <i class='fas fa-check-circle'></i>" }
//        ],
//        language: spanish
//    });

//    $("#table-puestos tbody").on('click', 'button', function () {
//        var data = tableJobs.row($(this).parents('tr')).data();
//        document.getElementById('pueusureg').value = data.sNombrePuesto;
//        document.getElementById('puesid').value = data.iIdPuesto;
//        $("#searchpuesto").modal('hide');
//        $("#registerposition").modal('show');
//    });

//    const tableJobsbtn = $("#table-puestosbtn").DataTable({
//        responsive: true,
//        destroy: true,
//        ajax: {
//            method: "POST",
//            url: "../CatalogsTables/Job",
//            contentType: "application/json; charset=utf-8",
//            dataType: "json",
//            dataSrc: "data"
//        },
//        columns: [
//            { "data": "sCodigoPuesto" },
//            { "data": "sNombrePuesto" },
//            { "data": "sDescripcionPuesto" },
//            { "defaultContent": "<button data-toggle='modal' data-target='#editpuesto' title='Editar' class='btn text-center btn-outline-warning shadow rounded ml-2'><i class='fas fa-edit'></i></button>" }
//        ],
//        language: spanish
//    });

//    const clvpuesto = document.getElementById('clvpuesto');
//    const edicodpuesto = document.getElementById('edicodpuesto');
//    const edipuesto = document.getElementById('edipuesto');
//    const edidescpuesto = document.getElementById('edidescpuesto');
//    const ediproffamily = document.getElementById('ediproffamily');
//    const edicolect = document.getElementById('edicolect');
//    const ediclasifpuesto = document.getElementById('ediclasifpuesto');
//    const edinivjerarpuesto = document.getElementById('edinivjerarpuesto');
//    const ediperfmanager = document.getElementById('ediperfmanager');
//    const editabpuesto = document.getElementById('editabpuesto');

//    $("#table-puestosbtn").on('click', 'button', function () {
//        var data = tableJobsbtn.row($(this).parents('tr')).data();
//        console.log(data);
//        $("#searchpuestobtn").modal('hide');
//        setTimeout(() => { edicodpuesto.focus(); }, 1000);
//        document.getElementById('namepuestoedi').textContent = data.sNombrePuesto;
//        clvpuesto.value = data.iIdPuesto;
//        edicodpuesto.value = data.sCodigoPuesto;
//        edipuesto.value = data.sNombrePuesto;
//        edidescpuesto.value = data.sDescripcionPuesto;
//        ediproffamily.value = data.iIdProfesionFamilia;
//        edicolect.value = data.iIdColectivo;
//        ediclasifpuesto.value = data.iIdClasificacionPuesto;
//        edinivjerarpuesto.value = data.iIdNivelJerarquico;
//        ediperfmanager.value = data.iIdPerfomanceManager;
//        editabpuesto.value = data.iIdTabulador;
//    });

//    floadproffamily(1, 'Active/Desactive', 0, ediproffamily);

//    floadnivjer(1, 'Active/Desactive', 0, edinivjerarpuesto);
//    floadperfman(1, 'Active/Desactive', 0, ediperfmanager);
//    floadniveltab(editabpuesto);
//    /*
//     * Funcion que carga los datos del select clasificacion del puesto 
//     */
//    floadcataloggeneral(ediclasifpuesto, 0, 'Active/Desactive', 0, 15);

//    /*
//     * Funcion que carga los datos del select colectivo
//     */
//    floadcataloggeneral(edicolect, 0, 'Active/Desactive', 0, 16);

//    /*
//     * Funcion que carga las claves del sat
//    */
//    //floadcataloggeneral(ediclvsat, 0, 'Active/Desactive', 0, 17);


//    /*
//     * Funcion que genera alertas dinamicas
//     */
//    fshowtypealert = (title, text, icon, element, clear) => {
//        Swal.fire({
//            title: title, text: text, icon: icon,
//            showClass: { popup: 'animated fadeInDown faster' },
//            hideClass: { popup: 'animated fadeOutUp faster' },
//            confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
//        }).then((acepta) => {
//            $("html, body").animate({
//                scrollTop: $(`#${element.id}`).offset().top - 50
//            }, 1000);
//            if (clear == 1) {
//                setTimeout(() => {
//                    element.focus();
//                    setTimeout(() => { element.value = ""; }, 300);
//                }, 1200);
//            } else {
//                setTimeout(() => {
//                    element.focus();
//                }, 1200);
//            }
//        });
//    }

//    String.prototype.capitalize = function () {
//        return this.replace(/(^|\s)([a-z])/g, function (m, p1, p2) { return p1 + p2.toUpperCase(); });
//    };

//    fmayletter = (string) => {
//        return string.chartArt(0).toUpperCase() + string.slice(1);
//    }

//    /*
//     * Funcion que guarda los datos de un puesto 
//     */

//    btnSavePuesto.addEventListener('click', () => {
//        const arrInputs = [regcodpuesto, regpuesto, regdescpuesto, proffamily, clasifpuesto, regcolect, nivjerarpuesto, perfmanager, tabpuesto];
//        let validate = 0;
//        for (let i = 0; i < arrInputs.length; i++) {
//            if (arrInputs[i].hasAttribute('tp-select')) {
//                if (arrInputs[i].value == "0") {
//                    const attrselect = arrInputs[i].getAttribute('tp-select');
//                    fshowtypealert('Atención', 'Selecciona una opción para el campo ' + String(attrselect), 'warning', arrInputs[i], 0);
//                    validate = 1;
//                    break;
//                }
//            } else {
//                if (arrInputs[i].value == "") {
//                    fshowtypealert('Atención', 'Completa el campo ' + arrInputs[i].placeholder, 'warning', arrInputs[i], 0);
//                    validate = 1;
//                    break;
//                }
//            }
//        }
//        if (validate == 0) {
//            const dataEnv = {
//                regcodpuesto: regcodpuesto.value, regpuesto: regpuesto.value, regdescpuesto: regdescpuesto.value, proffamily: proffamily.value, clasifpuesto: clasifpuesto.value,
//                regcolect: regcolect.value, nivjerarpuesto: nivjerarpuesto.value, perfmanager: perfmanager.value, tabpuesto: tabpuesto.value
//            };
//            try {
//                $.ajax({
//                    url: "../SaveDataGeneral/SaveDataPuestos",
//                    type: "POST",
//                    data: dataEnv,
//                    success: (data) => {
//                        if (data.sMensaje === "success") {
//                            Swal.fire({
//                                title: 'Registro correcto', icon: 'success',
//                                showClass: { popup: 'animated fadeInDown faster' },
//                                hideClass: { popup: 'animated fadeOutUp faster' },
//                                confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
//                            }).then((acepta) => {
//                                fclearfieldsnewjob();
//                                $("#registerpuesto").modal('hide');
//                                tableJobs.ajax.reload();
//                                tableJobsbtn.ajax.reload();
//                                setTimeout(() => {
//                                    if (JSON.parse(localStorage.getItem('modalbtnpuesto')) != null) {
//                                        $("#searchpuestobtn").modal('show');
//                                    } else {
//                                        $("#searchpuesto").modal('show');
//                                    }
//                                }, 1000);
//                            });
//                        } else {
//                            Swal.fire({
//                                title: 'Error', text: 'Contacte a sistemas', icon: 'error',
//                                showClass: { popup: 'animated fadeInDown faster' },
//                                hideClass: { popup: 'animated fadeOutUp faster' },
//                                confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
//                            }).then((acepta) => {
//                                fclearfieldsnewjob();
//                                $("#registerpuesto").modal('hide');
//                                setTimeout(() => {
//                                    if (JSON.parse(localStorage.getItem('modalbtnpuesto')) != null) {
//                                        $("#searchpuestobtn").modal('show');
//                                    } else {
//                                        $("#searchpuesto").modal('show');
//                                    }
//                                }, 1000);
//                            });
//                        }
//                    }, error: (jqXHR, exception) => {
//                        fcaptureaerrorsajax(jqXHR, exception);
//                    }
//                });
//            } catch (error) {
//                if (error instanceof TypeError) {
//                    console.log('TypeError ', error);
//                } else if (error instanceof RangeError) {
//                    console.log('RangeError ', error);
//                } else if (error instanceof EvalError) {
//                    console.log('EvalError ', error);
//                } else {
//                    console.log('Error ', error);
//                }
//            }
//        }
//    });

//    btnEdiPuesto.addEventListener('click', () => {
//        const arrInputs = [edicodpuesto, edipuesto, edidescpuesto, ediproffamily, ediclasifpuesto, edicolect, edinivjerarpuesto, ediperfmanager, editabpuesto];
//        let validate = 0;
//        for (let i = 0; i < arrInputs.length; i++) {
//            if (arrInputs[i].hasAttribute('tp-select')) {
//                if (arrInputs[i].value == "0") {
//                    const attrselect = arrInputs[i].getAttribute('tp-select');
//                    fshowtypealert('Atención', 'Selecciona una opción para el campo ' + String(attrselect), 'warning', arrInputs[i], 0);
//                    validate = 1;
//                    break;
//                }
//            } else {
//                if (arrInputs[i].value == "") {
//                    fshowtypealert('Atención', 'Completa el campo ' + arrInputs[i].placeholder, 'warning', arrInputs[i], 0);
//                    validate = 1;
//                    break;
//                }
//            }
//        }
//        if (validate == 0) {
//            const dataSend = {
//                edicodpuesto: edicodpuesto.value, edipuesto: edipuesto.value, edidescpuesto: edidescpuesto.value, ediproffamily: ediproffamily.value,
//                ediclasifpuesto: ediclasifpuesto.value, edicolect: edicolect.value,
//                edinivjerarpuesto: edinivjerarpuesto.value, ediperfmanager: ediperfmanager.value, editabpuesto: editabpuesto.value, clvpuesto: clvpuesto.value
//            }
//            console.log(dataSend);
//            console.log('Listo para enviar');
//            try {
//                $.ajax({
//                    url: "../EditDataGeneral/EditPuesto",
//                    type: "POST",
//                    data: dataSend,
//                    success: (data) => {
//                        if (data.result === "success") {
//                            Swal.fire({
//                                title: 'Puesto actualizado', icon: 'success',
//                                showClass: { popup: 'animated fadeInDown faster' }, hideClass: { popup: 'animated fadeOutUp faster' },
//                                confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
//                            }).then((acepta) => {
//                                $("#editpuesto").modal('hide');
//                                tableJobs.ajax.reload();
//                                tableJobsbtn.ajax.reload();
//                                setTimeout(() => {
//                                    if (JSON.parse(localStorage.getItem('modalbtnpuesto')) != null) {
//                                        $("#searchpuestobtn").modal('show');
//                                    } else {
//                                        $("#searchpuesto").modal('show');
//                                    }
//                                }, 1000);
//                            });
//                        } else {
//                            Swal.fire({
//                                title: 'Error', text: 'Contacte a sistemas', icon: 'error',
//                                showClass: { popup: 'animated fadeInDown faster' }, hideClass: { popup: 'animated fadeOutUp faster' },
//                                confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
//                            }).then((acepta) => {
//                                fclearfieldsnewjob();
//                                $("#editpuesto").modal('hide');
//                                setTimeout(() => {
//                                    if (JSON.parse(localStorage.getItem('modalbtnpuesto')) != null) {
//                                        $("#searchpuestobtn").modal('show');
//                                    } else {
//                                        $("#searchpuesto").modal('show');
//                                    }
//                                }, 1000);
//                            });
//                        }
//                    }, error: (jqXHR, exception) => {
//                        fcaptureaerrorsajax(jqXHR, exception);
//                    }
//                });
//            } catch (error) {
//                if (error instanceof RangeError) {
//                    console.log('RangeError ', error);
//                } else if (error instanceof EvalError) {
//                    console.log('EvalError ', error);
//                } else if (error instanceof TypeError) {
//                    console.log('TypeError ', error);
//                } else {
//                    console.log('Error ', error);
//                }
//            }
//        }
//    });
    
//});

$(function () {

    const spanish = {
        "decimal": "",
        "emptyTable": "No hay información",
        "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
        "infoEmpty": "Mostrando 0 t 0 of 0 Entradas",
        "infoFiltered": "(Filtrado de _MAX_ total entradas)",
        "infoPostFix": "",
        "thousands": ",",
        "lengthMenu": "Mostrar _MENU_ Entradas",
        "loadingRecords": "Cargando...",
        "processing": "Procesando...",
        "search": "Buscar:",
        "zeroRecords": "Sin resultados encontrados",
        "paginate": {
            "first": "Primero",
            "last": "Ultimo",
            "next": "<button class='ml-2 btn btn-sm btn-primary'>Siguiente </button>",
            "previous": "<button class='mr-2 btn btn-sm btn-primary'>Anterior</button>"
        }
    };

    const btnModalSearchPosition = document.getElementById('btn-modal-search-positions');
    const btnClearFieldsPositions = document.getElementById('btn-clear-fields-positions');
    const btnsaveposition = document.getElementById('btnsaveposition');
    const btnregisterpositionbtn = document.getElementById('btnregisterpositionbtn');

    /*
     * Variables registro de posiciones 
     */
    const regpatcla = document.getElementById('regpatcla');
    const departreg = document.getElementById('departreg');
    const pueusureg = document.getElementById('pueusureg');
    const depaid = document.getElementById('depaid');
    const puesid = document.getElementById('puesid');
    const fechefectpos = document.getElementById('fechefectpos');
    const localityr = document.getElementById('localityr');
    const emprepreg = document.getElementById('emprepreg');
    const report = document.getElementById('report');

    fclearfieldsnewposition = () => {
        regpatcla.value = '0';
        departreg.value = '';
        pueusureg.value = '';
        depaid.value = '';
        puesid.value = '';
        fechefectpos.value = '';
        localityr.value = '0';
        emprepreg.value = '0';
    }

    const btnCloseRegisterPositionBtn = document.getElementById('btn-close-registerposition-btn');
    const icoCloseRegisterPositionBtn = document.getElementById('ico-close-registerposition-btn');

    //btnClearFieldsPositions.addEventListener('click', fclearfieldsnewposition);

    localStorage.removeItem('modalbtnpositions');
    btnModalSearchPosition.addEventListener('click', () => { localStorage.setItem('modalbtnpositions', 1) });
    btnCloseRegisterPositionBtn.addEventListener('click', () => { localStorage.removeItem('modalbtnpositions'); });
    icoCloseRegisterPositionBtn.addEventListener('click', () => { localStorage.removeItem('modalbtnpositions'); });

    btnregisterpositions.addEventListener('click', () => {
        $("#searchpositionstab").modal('hide');
        setTimeout(() => { departreg.focus(); }, 1000);
    });

    btnregisterpositionbtn.addEventListener('click', () => {
        $("#searchpositions").modal('hide');
        setTimeout(() => { departreg.focus(); }, 1000);
    });

    const tablePositions = $("#table-positions").DataTable({
        ajax: {
            method: "POST",
            url: "../CatalogsTables/Positions",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            dataSrc: "data"
        },
        deferRender: true,
        serverSide: true,
        processing: true,
        columns: [
            { "data": "iIdPosicion" },
            { "data": "sNombreDepartamento" },
            { "defaultContent": "<button type='button' data-toggle='tooltip' data-placement='left' title='Seleccionar' class='btn text-center btn-outline-primary shadow rounded'> <i class='fas fa-check-circle'></i></button>" }
        ],
        language: spanish
    });

    $("#table-departaments tbody").on('click', 'button', function () {
        var data = tableDeparts.row($(this).parents('tr')).data();
        document.getElementById('departreg').value = data.sDepartamento;
        document.getElementById('depaid').value = data.iIdDepartamento;
        $("#searchdepartament").modal('hide');
        $("#registerposition").modal('show');
    });

    $("#table-positions tbody").on('click', 'button', function () {
        var data = tablePositions.row($(this).parents('tr')).data();
        document.getElementById('depart').value = data.sNombreDepartamento;
        document.getElementById('pueusu').value = data.sNombrePuesto;
        document.getElementById('numpla').value = data.iIdPosicion;
        document.getElementById('clvstr').value = data.iIdPosicion;
        document.getElementById('emprep').value = data.sRegistroPat;
        document.getElementById('localty').value = data.sLocalidad;
        document.getElementById('report').value = data.iIdReportaAPosicion;
        document.getElementById('msjfech').classList.remove('d-none');
        $("#searchpositionstab").modal('hide');
        console.log(data);
    });

    const tablePositionsBtn = $("#table-positions-btn").DataTable({
        ajax: {
            method: "POST",
            url: "../CatalogsTables/Positions",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            dataSrc: "data"
        },
        deferRender: true,
        serverSide: true,
        processing: true,
        columns: [
            { "data": "iIdPosicion" },
            { "data": "sNombreDepartamento" },
            { "defaultContent": "<button title='Editar departamento' data-toggle='modal' data-target='#editposition' class='btn text-center btn-outline-warning shadow rounded ml-2'><i class='fas fa-edit'></i></button>" }
        ],
        language: spanish
    });

    const clvposition = document.getElementById('clvposition');
    const depaidedit = document.getElementById('depaidedit');
    const departedit = document.getElementById('departedit');
    const puesidedit = document.getElementById('puesidedit');
    const pueusuedit = document.getElementById('pueusuedit');
    const editatcla = document.getElementById('editatcla');
    const editlocalityr = document.getElementById('editlocalityr');
    const emprepedit = document.getElementById('emprepedit');

    $("#table-positions-btn tbody").on('click', 'button', function () {
        var data = tablePositionsBtn.row($(this).parents('tr')).data();
        clvposition.value = data.iIdPosicion;
        depaidedit.value = data.iDepartamento_id;
        departedit.value = data.sNombreDepartamento;
        $("#searchpositions").modal('hide');
        console.log(data);
    });

    fechefectpos.addEventListener('change', () => {
        if (fechefectpos.value != "") {
            $("#msjfech").hide('1000');
        } else {
            $("#msjfech").show('1000');
        }
    });

    floadlocalitys = () => {
        try {
            $.ajax({
                url: "../Empleados/LoadLocalitys",
                type: "POST",
                data: {},
                success: (data) => {
                    const quantity = data.length;
                    if (quantity > 0) {
                        for (let i = 0; i < data.length; i++) {
                            localityr.innerHTML += `<option value="${data[i].iIdLocalidad}">${data[i].sDescripcion}</option>`;
                        }
                    } else {
                        console.log('No hay registros');
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof TypeError) {
                console.log('TypeError ', error);
            } else if (error instanceof EvalError) {
                console.log('EvalError ', error);
            } else if (error instanceof RangeError) {
                console.log('RangeError ', error);
            } else {
                console.log('Error ', error);
            }
        }
    }

    // ** Ejecución de la carga de las localidades al registrar una nueva posicion M ** \\
    floadlocalitys();

    floadpositions = (element) => {
        try {
            $.ajax({
                url: "../Empleados/LoadPositiosRep",
                type: "POST",
                data: {},
                success: (data) => {
                    const quantity = data.length;
                    if (element.id == 'report') {
                        let clvrep;
                        if (JSON.parse(localStorage.getItem('objectDataTabEstructure')) != null) {
                            const getDataEstructure = JSON.parse(localStorage.getItem('objectDataTabEstructure'));
                            for (i in getDataEstructure) {
                                if (getDataEstructure[i].key === "estructure") {
                                    clvrep = getDataEstructure[i].data.report;
                                }
                            }
                        }
                        if (quantity > 0) {
                            for (let i = 0; i < data.length; i++) {
                                if (clvrep == data[i].iIdPosicion) {
                                    element.innerHTML += `<option selected value="${data[i].iIdPosicion}">${data[i].iIdPosicion}</option>`;
                                } else {
                                    element.innerHTML += `<option value="${data[i].iIdPosicion}">${data[i].iIdPosicion}</option>`;
                                }
                            }
                        }
                    } else {
                        if (quantity > 0) {
                            for (let i = 0; i < data.length; i++) {
                                element.innerHTML += `<option value="${data[i].iIdPosicion}">${data[i].iIdPosicion}</option>`;
                            }
                        }
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof TypeError) {
                console.log('TypeError ', error);
            } else if (error instanceof EvalError) {
                console.log('EvalError ', error);
            } else if (error instanceof RangeError) {
                console.log('RangeError ', error);
            } else {
                console.log('Error ', error);
            }
        }
    }

    //** Ejecución de la carga de las posiciones al registrar una nueva posicion ** \\
    floadpositions(emprepreg);
    //floadpositions(report);

    floadregpatclases = () => {
        try {
            $.ajax({
                url: "../Empleados/LoadRegPatCla",
                type: "POST",
                data: {},
                success: (data) => {
                    const quantity = data.length;
                    if (quantity > 0) {
                        for (let i = 0; i < data.length; i++) {
                            regpatcla.innerHTML += `<option value="${data[i].iIdRegPat}">${data[i].sAfiliacionIMSS}</option>`;
                        }
                    } else {
                        console.log('No hay registros');
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof TypeError) {
                console.log('TypeError ', error);
            } else if (error instanceof EvalError) {
                console.log('EvalError ', error);
            } else if (error instanceof RangeError) {
                console.log('RangeError ', error);
            } else {
                console.log('Error ', error);
            }
        }
    }

    //** Ejecución de la carga de los registros patronales clases al registrar una nueva posicion ** \\
    floadregpatclases();

    btnsaveposition.addEventListener('click', () => {
        try {
            const arrInput = [depaid, puesid, regpatcla, localityr, emprepreg];
            let validate = 0;
            for (let i = 0; i < arrInput.length; i++) {
                if (arrInput[i].hasAttribute('tp-select')) {
                    if (arrInput[i].value == "0") {
                        const attrselect = arrInput[i].getAttribute('tp-select');
                        fshowtypealert('Atención', 'Selecciona una opción para el campo ' + String(attrselect), 'warning', arrInput[i], 0);
                        validate = 1;
                        break;
                    }
                } else {
                    if (arrInput[i].value == "") {
                        fshowtypealert('Atención', 'Completa el campo ' + arrInput[i].placeholder, 'warning', arrInput[i], 0);
                        validate = 1;
                        break;
                    }
                }
            }
            if (validate == 0) {
                const dataSend = {
                    depaid: depaid.value, puesid: puesid.value, regpatcla: regpatcla.value, fechefectpos: fechefectpos.value,
                    localityr: localityr.value, emprepreg: emprepreg.value
                };
                $.ajax({
                    url: "../SaveDataGeneral/SavePositions",
                    type: "POST",
                    data: dataSend,
                    success: (data) => {
                        if (data.result === "success") {
                            Swal.fire({
                                title: 'Posicion registrada', icon: 'success',
                                showClass: { popup: 'animated fadeInDown faster' }, hideClass: { popup: 'animated fadeOutUp faster' },
                                confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
                            }).then((acepta) => {
                                $("#registerposition").modal('hide');
                                tablePositions.ajax.reload();
                                tablePositionsBtn.ajax.reload();
                                setTimeout(() => {
                                    if (JSON.parse(localStorage.getItem('modalbtnpositions')) != null) {
                                        $("#searchpositions").modal('show');
                                    } else {
                                        $("#searchpositionstab").modal('show');
                                    }
                                }, 1000);
                            });
                        } else {
                            Swal.fire({
                                title: 'Error', text: 'Contacte a sistemas', icon: 'error',
                                showClass: { popup: 'animated fadeInDown faster' }, hideClass: { popup: 'animated fadeOutUp faster' },
                                confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
                            }).then((acepta) => {
                                $("#registerposition").modal('hide');
                                setTimeout(() => {
                                    if (JSON.parse(localStorage.getItem('modalbtnpositions')) != null) {
                                        $("#searchpositions").modal('show');
                                    } else {
                                        $("#searchpositionstab").modal('show');
                                    }
                                }, 1000);
                            });
                        }
                    }, error: (jqxHR, exception) => {
                        fcaptureaerrorsajax(jqxHR, exception);
                    }
                });
            }
        } catch (error) {
            if (error instanceof TypeError) {
                console.log('TypeError ', error);
            } else if (error instanceof EvalError) {
                console.log('EvalError ', error);
            } else if (error instanceof RangeError) {
                console.log('RangeError ', error);
            } else {
                console.log('Error ', error);
            }
        }
    });

    freloadtables = () => {
        tablePositions.ajax.reload();
        tablePositionsBtn.ajax.reload();
    }

});
