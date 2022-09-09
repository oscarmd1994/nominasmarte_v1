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

    /*
     * Funciones departamentos
     */

    /*
     * Variables departamentos 
     */

    const btnregisterdepartament = document.getElementById('btnregisterdepartament');
    const btnregisterdepartamentbtn = document.getElementById('btnregisterdepartamentbtn');
    const btnsavedepartament = document.getElementById('btnsavedepartament');
    const btnedidepartament = document.getElementById('btnedidepartament');
    const icoClearFieldsDepart = document.getElementById('ico-clear-fields-departament');
    const btnClearFieldsDepart = document.getElementById('btn-clear-fields-departament');
    const regdepart = document.getElementById('regdepart');
    const descdepart = document.getElementById('descdepart');
    const reportaa = document.getElementById('reportaa');
    const centrcost = document.getElementById('centrcost');
    const edific = document.getElementById('edific');
    const piso = document.getElementById('piso');
    const nivestuc = document.getElementById('nivestuc');
    const ubicac = document.getElementById('ubicac');
    const dgatxt = document.getElementById('dgatxt');
    const nivsuptxt = document.getElementById('nivsuptxt');
    const dirgentxt = document.getElementById('dirgentxt');
    const direjetxt = document.getElementById('direjetxt');
    const diraretxt = document.getElementById('diraretxt');
    const dirgen = document.getElementById('dirgen');
    const direje = document.getElementById('direje');
    const dirare = document.getElementById('dirare');

    fclearfieldsnewdepart = () => {
        regdepart.value = ""; descdepart.value = "";
        nivestuc.value = "0"; nivsuptxt.value = "";
        edific.value = "0"; piso.value = "";
        ubicac.value = ""; centrcost.value = "0";
        reportaa.value = "0"; dgatxt.value = "";
        dirgentxt.value = ""; direjetxt.value = "";
        diraretxt.value = ""; dirgen.value = "0";
        direje.value = "0"; dirare.value = "0";
    }

    icoClearFieldsDepart.addEventListener('click', () => { fclearfieldsnewdepart(); });
    btnClearFieldsDepart.addEventListener('click', () => { fclearfieldsnewdepart(); });

    // Funcion que carga las empresas del sistema \\
    floadbusiness = (state, type, keyemp, elementid) => {
        try {
            $.ajax({
                url: "../CatalogsTables/Business",
                type: "POST",
                data: { state: state, type: type, keyemp: keyemp },
                success: (data) => {
                    const quantity = data.length;
                    if (quantity > 0) {
                        for (let i = 0; i < data.length; i++) {
                            elementid.innerHTML += `<option value="${data[i].iIdEmpresa}">${data[i].sNombreEmpresa}</option>`;
                        }
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof EvalError) {
                console.log('EvalError ', error);
            } else if (error instanceof RangeError) {
                console.log('RangeError ', error);
            } else if (error instanceof TypeError) {
                console.log('TypeError ', error);
            } else {
                console.log('Error ', error);
            }
        }
    }

    floadbusiness(0, 'Active/Desactive', 0, reportaa);
    floadbusiness(0, 'Active/Desactive', 0, dirgen);
    floadbusiness(0, 'Active/Desactive', 0, direje);
    floadbusiness(0, 'Active/Desactive', 0, dirare);

    // Funcion que carga los centros de costo \\
    floadcentrcost = (state, type, keycos, elementid) => {
        try {
            $.ajax({
                url: "../CatalogsTables/CentrCost",
                type: "POST",
                data: { state: state, type: type, keycos: keycos },
                success: (data) => {
                    const quantity = data.length;
                    if (quantity > 0) {
                        for (let i = 0; i < data.length; i++) {
                            elementid.innerHTML += `<option value="${data[i].iIdCentroCosto}">${data[i].sCentroCosto}</option>`;
                        }
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof EvalError) {
                console.log('EvalError ', error);
            } else if (error instanceof RangeError) {
                console.log('RangeError ', error);
            } else if (error instanceof TypeError) {
                console.log('TypeError ', error);
            } else {
                console.log('Error ', error);
            }
        }
    }

    floadcentrcost(0, 'Active/Desactive', 0, centrcost);

    // Funcion que carga los datos de edificio \\

    floadbuilding = (type, keyedi, elementid, val) => {
        try {
            $.ajax({
                url: "../CatalogsTables/Buildings",
                type: "POST",
                data: { type: type, keyedi: keyedi },
                success: (data) => {
                    const quantity = data.length;
                    if (quantity) {
                        for (let i = 0; i < data.length; i++) {
                            if (val == data[i].iIdEdificio) {
                                elementid.innerHTML += `<option selected value="${data[i].iIdEdificio}">${data[i].sNombreEdificio}</option>`;
                            } else {
                                elementid.innerHTML += `<option value="${data[i].iIdEdificio}">${data[i].sNombreEdificio}</option>`;
                            }
                        }
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof RangeError) {
                console.log('RangeError ', error);
            } else if (error instanceof EvalError) {
                console.log('EvalError ', error);
            } else if (error instanceof TypeError) {
                console.log('TypeError ', error);
            } else {
                console.log('Error ', error);
            }
        }
    }

    floadbuilding('Active/Desactive', 0, edific, 0);

    // Funcion que carga los niveles de estructura \\
    floadnivestuc = (state, type, keyniv, elementid, val) => {
        try {
            $.ajax({
                url: "../CatalogsTables/NivEstruct",
                type: "POST",
                data: { state: state, type: type, keyniv: keyniv },
                success: (data) => {
                    const quantity = data.length;
                    if (quantity > 0) {
                        for (let i = 0; i < data.length; i++) {
                            if (val == data[i].sNivelEstructura) {
                                elementid.innerHTML += `<option selected value="${data[i].sNivelEstructura}">${data[i].sNivelEstructura}</option>`;
                            } else {
                                elementid.innerHTML += `<option value="${data[i].sNivelEstructura}">${data[i].sNivelEstructura}</option>`;
                            }
                        }
                    }
                }, error: (jqxHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof RangeError) {
                console.log('RangeError ', error);
            } else if (error instanceof EvalError) {
                console.log('EvalError ', error);
            } else if (error instanceof TypeError) {
                console.log('TypeError ', error);
            } else {
                console.log('Error ', error);
            }
        }
    }

    floadnivestuc(0, 'Active/Desactive', 0, nivestuc, 0);

    btnregisterdepartament.addEventListener('click', () => {
        $("#searchdepartament").modal('hide');
        setTimeout(() => { regdepart.focus(); }, 1200);
    });

    btnregisterdepartamentbtn.addEventListener('click', () => {
        $("#searchdepartamentbtn").modal('hide');
        setTimeout(() => { regdepart.focus(); }, 1200);
    });

    /*
     * Inicializacion datatable departamentos 
     */

    const tableDeparts = $("#table-departaments").DataTable({
        destroy: true,
        responsive: true,
        ajax: {
            method: "POST",
            url: "../CatalogsTables/Departaments",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            dataSrc: "data",
        },
        columns: [
            { "data": "sDeptoCodigo" },
            { "data": "sDescripcionDepartamento" },
            { "defaultContent": "<button type='button' data-toggle='tooltip' data-placement='left' title='Seleccionar' class='btn text-center btn-outline-primary shadow rounded'> <i class='fas fa-check-circle'></i> </button>" }
        ],
        language: spanish
    });

    $("#table-departaments tbody").on('click', 'button', function () {
        var data = tableDeparts.row($(this).parents('tr')).data();
        //document.getElementById('departreg').value = data.sDepartamento;
        //document.getElementById('depaid').value = data.iIdDepartamento;
        $("#searchdepartament").modal('hide');
        $("#registerposition").modal('show');
    });

    const tableDepartsBtn = $("#table-departamentsbtn").DataTable({
        destroy: true,
        responsive: true,
        ajax: {
            method: "POST",
            url: "../CatalogsTables/Departaments",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            dataSrc: "data",
        },
        columns: [
            { "data": "sDeptoCodigo" },
            { "data": "sDescripcionDepartamento" },
            { "defaultContent": "<button title='Editar departamento' data-toggle='modal' data-target='#editdepartament' class='btn text-center btn-outline-warning shadow rounded ml-2'><i class='fas fa-edit'></i></button>" }
        ],
        language: spanish
    });

    //const tableTestDepart = $("#table-departamentssearch").DataTable({

    //});

    const tableSearchDepartament = $("#table-departamentssearch").DataTable({
        destroy: true,
        responsive: true,
        ajax: {
            method: "POST",
            url: "../CatalogsTables/DepartamentsSearch",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            dataSrc: "data",
        },
        columns: [
            { "data": "sDeptoCodigo" },
            { "data": "sDescripcionDepartamento" },
            { "defaultContent": "<button type='button' data-toggle='tooltip' data-placement='left' title='Seleccionar' class='btn text-center btn-outline-primary shadow rounded'> <i class='fas fa-check-circle'></i> </button>" }
        ],
        language: spanish
    });

    const btnSearchTableDepartament0 = document.getElementById('btn-search-table-departaments0');
    const btnSearchTableDepartament1 = document.getElementById('btn-search-table-departaments1');
    const btnSearchTableDepartament2 = document.getElementById('btn-search-table-departaments2');
    const btnSearchTableDepartament3 = document.getElementById('btn-search-table-departaments3');
    const btnSearchTableDepartament4 = document.getElementById('btn-search-table-departaments4');

    btnSearchTableDepartament0.addEventListener('click', () => { $("#registerdepartament").modal('hide'); localStorage.setItem('departsearch0', 1); });

    btnSearchTableDepartament1.addEventListener('click', () => { $("#registerdepartament").modal('hide'); localStorage.setItem('departsearch1', 1); });

    btnSearchTableDepartament2.addEventListener('click', () => { $("#registerdepartament").modal('hide'); localStorage.setItem('departsearch2', 1); });

    btnSearchTableDepartament3.addEventListener('click', () => { $("#registerdepartament").modal('hide'); localStorage.setItem('departsearch3', 1); });

    btnSearchTableDepartament4.addEventListener('click', () => { $("#registerdepartament").modal('hide'); localStorage.setItem('departsearch4', 1); });

    // Variables edita departamento
    const clvdepart = document.getElementById('clvdepart');
    const edidepart = document.getElementById('edidepart');
    const edidescdepart = document.getElementById('edidescdepart');
    const nivsuptxtedit = document.getElementById('nivsuptxtedit');
    const edireportaa = document.getElementById('edireportaa');
    const edicentrcost = document.getElementById('edicentrcost');
    const ediedific = document.getElementById('ediedific');
    const edinivestuc = document.getElementById('edinivestuc');
    const edipiso = document.getElementById('edipiso');
    const ediubicac = document.getElementById('ediubicac');
    const edidgatxt = document.getElementById('edidgatxt');
    const edidirgentxt = document.getElementById('edidirgentxt');
    const edidirejetxt = document.getElementById('edidirejetxt');
    const edidiraretxt = document.getElementById('edidiraretxt');
    const edidirgen = document.getElementById('edidirgen');
    const edidireje = document.getElementById('edidireje');
    const edidirare = document.getElementById('edidirare');
    const btnSearchTableDepartament0Edit = document.getElementById('btn-search-table-departaments0edit');
    const btnSearchTableDepartament1Edit = document.getElementById('btn-search-table-departaments1edit');
    const btnSearchTableDepartament2Edit = document.getElementById('btn-search-table-departaments2edit');
    const btnSearchTableDepartament3Edit = document.getElementById('btn-search-table-departaments3edit');
    const btnSearchTableDepartament4Edit = document.getElementById('btn-search-table-departaments4edit');

    btnSearchTableDepartament0Edit.addEventListener('click', () => { $("#editdepartament").modal('hide'); localStorage.setItem('departsearchedit0', 1); });
    btnSearchTableDepartament1Edit.addEventListener('click', () => { $("#editdepartament").modal('hide'); localStorage.setItem('departsearchedit1', 1); });
    btnSearchTableDepartament2Edit.addEventListener('click', () => { $("#editdepartament").modal('hide'); localStorage.setItem('departsearchedit2', 1); });
    btnSearchTableDepartament3Edit.addEventListener('click', () => { $("#editdepartament").modal('hide'); localStorage.setItem('departsearchedit3', 1); });
    btnSearchTableDepartament4Edit.addEventListener('click', () => { $("#editdepartament").modal('hide'); localStorage.setItem('departsearchedit4', 1); });

    floadbusiness(0, 'Active/Desactive', 0, edidirgen);
    floadbusiness(0, 'Active/Desactive', 0, edidireje);
    floadbusiness(0, 'Active/Desactive', 0, edidirare);


    $("#table-departamentssearch tbody").on('click', 'button', function () {
        var data = tableSearchDepartament.row($(this).parents('tr')).data();
        if (localStorage.getItem('departsearch0') != null) {
            nivsuptxt.value = data.sDeptoCodigo;
            localStorage.removeItem('departsearch0');
            $("#registerdepartament").modal('show');
        }
        if (localStorage.getItem('departsearch1') != null) {
            dgatxt.value = data.sDeptoCodigo;
            localStorage.removeItem('departsearch1');
            $("#registerdepartament").modal('show');
        }
        if (localStorage.getItem('departsearch2') != null) {
            dirgentxt.value = data.sDeptoCodigo;
            localStorage.removeItem('departsearch2');
            $("#registerdepartament").modal('show');
        }
        if (localStorage.getItem('departsearch3') != null) {
            direjetxt.value = data.sDeptoCodigo;
            localStorage.removeItem('departsearch3');
            $("#registerdepartament").modal('show');
        }
        if (localStorage.getItem('departsearch4') != null) {
            diraretxt.value = data.sDeptoCodigo;
            localStorage.removeItem('departsearch4');
            $("#registerdepartament").modal('show');
        }
        if (localStorage.getItem('departsearchedit0') != null) {
            nivsuptxtedit.value = data.sDeptoCodigo;
            localStorage.removeItem('departsearchedit0');
            $("#editdepartament").modal('show');
        }
        if (localStorage.getItem('departsearchedit1') != null) {
            edidgatxt.value = data.sDeptoCodigo;
            localStorage.removeItem('departsearchedit1');
            $("#editdepartament").modal('show');
        }
        if (localStorage.getItem('departsearchedit2') != null) {
            edidirgentxt.value = data.sDeptoCodigo;
            localStorage.removeItem('departsearchedit2');
            $("#editdepartament").modal('show');
        }
        if (localStorage.getItem('departsearchedit3') != null) {
            edidirejetxt.value = data.sDeptoCodigo;
            localStorage.removeItem('departsearchedit3');
            $("#editdepartament").modal('show');
        }
        if (localStorage.getItem('departsearchedit4') != null) {
            edidiraretxt.value = data.sDeptoCodigo;
            localStorage.removeItem('departsearchedit4');
            $("#editdepartament").modal('show');
        }
        $("#searchdepartamentsdirec").modal('hide');
        console.log(data);
    });



    floadcentrcost(0, 'Active/Desactive', 0, edicentrcost);
    floadbusiness(0, 'Active/Desactive', 0, edireportaa);
    floadbuilding('Active/Desactive', 0, ediedific, 0);
    floadnivestuc(0, 'Active/Desactive', 0, edinivestuc, 0);

    $("#table-departamentsbtn tbody").on('click', 'button', function () {
        $("#searchdepartamentbtn").modal('hide');
        var data = tableDepartsBtn.row($(this).parents('tr')).data();
        setTimeout(() => { edidepart.focus(); }, 1000);
        document.getElementById('namedepartamentedi').textContent = data.sDescripcionDepartamento;
        clvdepart.value = data.iIdDepartamento;
        edidepart.value = data.sDeptoCodigo;
        edidescdepart.value = data.sDescripcionDepartamento;
        nivsuptxtedit.value = data.sNivelSuperior;
        edinivestuc.value = data.sNivelEstructura;
        ediedific.value = data.iEdificioId;
        edipiso.value = data.sPiso;
        ediubicac.value = data.sUbicacion;
        edicentrcost.value = data.iCentroCostoId;
        edireportaa.value = data.iEmpresaReportaId;
        edidgatxt.value = data.sDGA;
        edidirgentxt.value = data.sDirecGen;
        edidirejetxt.value = data.sDirecEje;
        edidiraretxt.value = data.sDirecAre;
        edidirgen.value = data.iEmpreDirGen;
        edidireje.value = data.iEmpreDirEje;
        edidirare.value = data.iEmpreDirAre;
        console.log(data);
    });

    /*
     * Guarda datos de nuevo departamento
     */


    btnsavedepartament.addEventListener('click', () => {
        const arrInputs = [regdepart, descdepart, nivestuc, edific, piso, centrcost, reportaa, dirgen, direje, dirare];
        let validate = 0;
        for (let i = 0; i < arrInputs.length; i++) {
            if (arrInputs[i].hasAttribute('tp-select')) {
                if (arrInputs[i].value == "0") {
                    const attrselect = arrInputs[i].getAttribute('tp-select');
                    fshowtypealert('Atención', 'Selecciona una opción para el campo ' + String(attrselect), 'warning', arrInputs[i], 0);
                    validate = 1;
                    break;
                }
            } else {
                if (arrInputs[i].value == "") {
                    fshowtypealert('Atención', 'Completa el campo ' + arrInputs[i].placeholder, 'warning', arrInputs[i], 0);
                    validate = 1;
                    break;
                }
            }
        }
        if (validate == 0) {
            const dataEnv = {
                regdepart: regdepart.value, descdepart: descdepart.value,
                nivestuc: nivestuc.value, nivsuptxt: nivsuptxt.value,
                edific: edific.value, piso: piso.value,
                ubicac: ubicac.value, centrcost: centrcost.value,
                reportaa: reportaa.value, dgatxt: dgatxt.value,
                dirgentxt: dirgentxt.value, direjetxt: direjetxt.value,
                diraretxt: diraretxt.value, dirgen: dirgen.value,
                direje: direje.value, dirare: dirare.value,
            };
            try {
                $.ajax({
                    url: "../SaveDataGeneral/SaveDepartament",
                    type: "POST",
                    data: dataEnv,
                    success: (data) => {
                        if (data.result === "success") {
                            Swal.fire({
                                title: 'Registro correcto', icon: 'success',
                                showClass: { popup: 'animated fadeInDown faster' },
                                hideClass: { popup: 'animated fadeOutUp faster' },
                                confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
                            }).then((acepta) => {
                                fclearfieldsnewdepart();
                                $("#registerdepartament").modal('hide');
                                tableDeparts.ajax.reload();
                                tableDepartsBtn.ajax.reload();
                                setTimeout(() => {
                                    if (JSON.parse(localStorage.getItem('modalbtndepartament')) != null) {
                                        $("#searchdepartamentbtn").modal('show');
                                    } else {
                                        $("#searchdepartament").modal('show');
                                    }
                                }, 1000);
                            });
                        } else {
                            Swal.fire({
                                title: 'Error', text: 'Contacte a sistemas', icon: 'error',
                                showClass: { popup: 'animated fadeInDown faster' },
                                hideClass: { popup: 'animated fadeOutUp faster' },
                                confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
                            }).then((acepta) => {
                                fclearfieldsnewdepart();
                                $("#registerdepartament").modal('hide');
                                setTimeout(() => {
                                    if (JSON.parse(localStorage.getItem('modalbtndepartament')) != null) {
                                        $("#searchdepartamentbtn").modal('show');
                                    } else {
                                        $("#searchdepartament").modal('show');
                                    }
                                }, 1000);
                            });
                        }
                    }, error: (jqXHR, exception) => {
                        fcaptureaerrorsajax(jqXHR, exception);
                    }
                });
            } catch (error) {
                if (error instanceof RangeError) {
                    console.log('RangeError ', error);
                } else if (error instanceof EvalError) {
                    console.log('EvalError ', error);
                } else if (error instanceof TypeError) {
                    console.log('TypeError ', error);
                } else {
                    console.log('Error ', error);
                }
            }
        }
    });

    btnedidepartament.addEventListener('click', () => {
        const arrInputs = [edidepart, edidescdepart, edinivestuc, ediedific, edipiso, edicentrcost, edireportaa, edidirgen, edidireje, edidirare];
        let validate = 0;
        for (let i = 0; i < arrInputs.length; i++) {
            if (arrInputs[i].hasAttribute('tp-select')) {
                if (arrInputs[i].value == "0") {
                    const attrselect = arrInputs[i].getAttribute('tp-select');
                    fshowtypealert('Atención', 'Selecciona una opción para el campo ' + String(attrselect), 'warning', arrInputs[i], 0);
                    validate = 1;
                    break;
                }
            } else {
                if (arrInputs[i].value == "") {
                    fshowtypealert('Atención', 'Completa el campo ' + arrInputs[i].placeholder, 'warning', arrInputs[i], 0);
                    validate = 1;
                    break;
                }
            }
        }
        if (validate == 0) {
            const dataSend = {
                edidepart: edidepart.value, edidescdepart: edidescdepart.value, edinivestuc: edinivestuc.value, nivsuptxtedit: nivsuptxtedit.value,
                ediedific: ediedific.value, edipiso: edipiso.value, ediubicac: ediubicac.value, edicentrcost: edicentrcost.value,
                edireportaa: edireportaa.value, edidgatxt: edidgatxt.value, edidirgentxt: edidirgentxt.value, edidirejetxt: edidirejetxt.value,
                edidiraretxt: edidiraretxt.value, edidirgen: edidirgen.value, edidireje: edidireje.value, edidirare: edidirare.value,
                clvdepart: clvdepart.value
            };
            try {
                $.ajax({
                    url: "../EditDataGeneral/EditDepartament",
                    type: "POST",
                    data: dataSend,
                    success: (data) => {
                        if (data.result === "success") {
                            Swal.fire({
                                title: 'Departamento actualizado', icon: 'success',
                                showClass: { popup: 'animated fadeInDown faster' }, hideClass: { popup: 'animated fadeOutUp faster' },
                                confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
                            }).then((acepta) => {
                                $("#editdepartament").modal('hide');
                                tableDeparts.ajax.reload();
                                tableDepartsBtn.ajax.reload();
                                setTimeout(() => {
                                    if (JSON.parse(localStorage.getItem('modalbtndepartament')) != null) {
                                        $("#searchdepartamentbtn").modal('show');
                                    } else {
                                        $("#searchdepartament").modal('show');
                                    }
                                }, 1000);
                            });
                        } else {
                            Swal.fire({
                                title: 'Error', text: 'Contacte a sistemas', icon: 'error',
                                showClass: { popup: 'animated fadeInDown faster' }, hideClass: { popup: 'animated fadeOutUp faster' },
                                confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
                            }).then((acepta) => {
                                $("#editdepartament").modal('hide');
                                setTimeout(() => {
                                    if (JSON.parse(localStorage.getItem('modalbtndepartament')) != null) {
                                        $("#searchdepartamentbtn").modal('show');
                                    } else {
                                        $("#searchdepartament").modal('show');
                                    }
                                }, 1000);
                            });
                        }
                    }, error: (jqXHR, exception) => {
                        fcaptureaerrorsajax(jqXHR, exception);
                    }
                });
            } catch (error) {
                if (error instanceof RangeError) {
                    console.log('RangeError ', error);
                } else if (error instanceof TypeError) {
                    console.log('TypeError ', error);
                } else if (error instanceof EvalError) {
                    console.log('EvalError ', error);
                } else {
                    console.log('Error ', error);
                }
            }
        }
    });

});