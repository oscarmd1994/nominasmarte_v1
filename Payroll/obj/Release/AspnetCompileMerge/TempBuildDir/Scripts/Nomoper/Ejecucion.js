$(function () {

    var DatoEjeCerrada;
    var IdDropList = 0;
    //var IdDropList2;
    var AnioDropList;
    var TipodePeridoDroplip;
    var periodo;
    var empresa;
    var RowsGrid;
    var exitRow;
    var NombreEmpleado;
    var NoEmpleado;
    var CheckCalculoEmpresa = 0;
    var checkCalculoEmplado = 0;
    var checkedItemsIdEmpleados = "";

    /// Tab Ejecucion

    const btnFloGuardar = document.getElementById('btnFloGuardar');
    const btnFloEjecutar = document.getElementById('btnFloEjecutar');
    const CheckXEmpresa = document.getElementById('CheckXEmpresa');
    const inputAnio = document.getElementById('TbAño');
    const NombEmpre = document.getElementById('NombEmpre');
    const TxbTipoPeriodo = document.getElementById('TxbTipoPeriodo');
    const PeridoEje = document.getElementById('PeridoEje');
    const EjeEmpresa = document.getElementById('EjeEmpresa');
    const CheckXempleado = document.getElementById('CheckXempleado');
    const LaCheckXEmpleado = document.getElementById('LaCheckXEmpleado');

    const CheckPeridoEspc = document.getElementById('CheckPeridoEspc');
    const Empleadoseje = document.getElementById('Empleadoseje');
    const dropEmpledos = document.getElementById('DropLitEmple');

    var valorCheckXempleado = document.getElementById('CheckXempleado');
    var ChekEnFirme = document.getElementById('ChekEnFirme');
    var empresas = "";
    var seconds = 0;
    var cantidad;
    var valorCheckxempresa = document.getElementById('CheckXEmpresa');

    /// DESACTIVA BOTONES ✔
    $("#btnFloGuardar").attr("disabled", "disabled");
    //$("#btnFloActualiza").attr("disabled", "disabled");
    //$("#btnFloLimpiar").attr("disabled", "disabled");
    $("#btnFloEjecutar").attr("disabled", "disabled");
    ////

    /// quita logo de carga ✔
    //$('#jqxLoader').jqxLoader('close');

    /// llenad el grid de Definicion
    LoadDefinicionSelect = () => {
        var opDeNombre = "Selecciona";
        var opDeCancelados = 2;
        const dataSend = { sNombreDefinicion: opDeNombre, iCancelado: opDeCancelados };
        $.ajax({
            url: "../Nomina/TpDefinicionNominaQry",
            type: "POST",
            data: dataSend,
            success: (data) => {
                var select = document.getElementById("definicionSelect");
                select.innerHTML = "<option class='text-center' value=''>  --Selecciona--  </option>";
                for (var i = 0; i < data.length; i++) {
                    select.innerHTML += "<option value='" + data[i].iIdDefinicionhd + "'>" + data[i].iIdDefinicionhd + " - " + data[i].sNombreDefinicion + " / " + data[i].sDescripcion + "</option>";
                }
            }
        });
    }

    LoadDefinicionSelect();

    /// LLENA CAMPOS DESPUES DE ELEGIR LA DEFINICION ✔
    $("#definicionSelect").change(() => {
        if ($("#definicionSelect").val() > 0) {
            var definicion_id = $("#definicionSelect").val();
            LoadSelectedDefinicion(definicion_id);
            getStatusDefinicion(definicion_id);
        } else {
            FLimpiaCamp();
        }
    });

    /// INFO DEFINICION SELECCIONADA ✔
    LoadSelectedDefinicion = (idDefinicion) => {
        var opDeNombre = "Selecciona";
        var opDeCancelados = 2;
        const dataSend = { sNombreDefinicion: opDeNombre, iCancelado: opDeCancelados };
        $.ajax({
            url: "../Nomina/TpDefinicionNominaQry",
            type: "POST",
            data: dataSend,
            success: (data) => {
                for (var i = 0; i < data.length; i++) {
                    if (data[i].iIdDefinicionhd == idDefinicion) {
                        inputAnio.value = data[i].iAno;
                    }
                }
                VerifyCalculosHD();
                ListEmpresa($("#definicionSelect").val());
            },
        });
    }

    /// VERIFICA EN TBCALCULOS HD
    VerifyCalculosHD = () => {
        const dataSend = { iIdDefinicionHd: $("#definicionSelect").val(), iperiodo: 0, NomCerr: 0, Anio: inputAnio.value };
        $.ajax({
            url: "../Nomina/CompruRegistroExit",
            type: "POST",
            data: dataSend,
            success: (data) => {
                if (data.Result[0].iIdCalculosHd == 1) {
                    // tipo de periodo de la definicion
                    $("#TxbTipoPeriodo").val(data.LTP[0].iId);
                    // ingresa el periodo
                    if (data.LPe[0].sMensaje == "success") {
                        $("#PeridoEje").empty();
                        document.getElementById("PeridoEje").innerHTML += `<option value='${data.LPe[0].iPeriodo}'>${data.LPe[0].iPeriodo} Fecha del: ${data.LPe[0].sFechaInicio} al ${data.LPe[0].sFechaFinal}</option>`;
                        periodo = data.LPe[0].iPeriodo;
                    }
                    if (data.LPe[0].sMensaje == "error") {
                        Swal.fire({ icon: 'error', title: 'Ejecucion', text: 'Algunas empresas no tienen periodo disponible!' });
                    }
                }
                if (data.Result[0].iIdCalculosHd == 0) {
                    $("#btnFloGuardar").removeAttr("disabled");
                    $("#btnFloEjecutar").attr("disabled", "disabled");
                    $("#PeridoEje").html("<option class='text-center'>Sin periodos<option>");
                    Swal.fire({ icon: 'info', title: 'Validación', text: 'Pare ver los periodos disponibles, guarda la Definición!' });
                }
            }
        });
    }

    /// verifica que la definicion no este en 
    getStatusDefinicion = (Definicion_id) => {
        $.ajax({
            url: "../Nomina/getStatusDefinicion",
            type: "POST",
            data: JSON.stringify({
                Definicion_id: Definicion_id,
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: (data) => {
                if (data.iFlag != 1) {
                    $("#btnFloEjecutar").removeAttr("disabled");
                } else {
                    Swal.fire({ icon: 'warning', title: 'En Proceso', text: 'La definicion ' + Definicion_id + ' se encuentra en proceso!' });
                    FLimpiaCamp();
                }
            },
        });
    }

    /// llena el drop de empresa en la pantalla ejecucion ✔
    ListEmpresa = (CalculosHd_id) => {
        empresas = "";
        const dataSend2 = { iIdCalculosHd: CalculosHd_id, iTipoPeriodo: 0, iPeriodo: 0, idEmpresa: 0, anio: 0 };
        $("#EjeEmpresa").empty();
        $('#EjeEmpresa').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/EmpresaCal",
            type: "POST",
            data: dataSend2,
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("EjeEmpresa").innerHTML += `<option value='${data[i].iIdEmpresa}'>${data[i].iIdEmpresa}  ${data[i].sNombreEmpresa} </option>`;
                    empresa = empresa + "," + data[i].iIdEmpresa;
                }
            },
        });
    };

    /// Limpia campos de tab Ejecucion ✔

    FLimpiaCamp = () => {

        inputAnio.value = "";
        TxbTipoPeriodo.value = "";
        if (CheckXempleado.checked == true) {
            $("#CheckXempleado").click();
        }
        if (CheckXEmpresa.checked == true) {
            $("#CheckXEmpresa").click();
        }
        if (ChekEnFirme.checked == true) {
            ChekEnFirme.checked = false;
        }
        $("#btnFloEjecutar").attr("disabled", "disabled");
        $("#frmEjecucion")[0].reset();
        $("#PeridoEje").html("<option class='text-center'>Sin periodos<option>");
    };

    //btnFloLimpiar.addEventListener('click', FLimpiaCamp);

    /// Periodo especial ✔

    $('#CheckPeridoEspc').change(function () {
        //console.log('cambia check p esp');
        if ($("#definicionSelect").val() != "" || $("#definicionSelect").val() < 0) {
            var dataSend = { iIdDefinicionHd: $("#definicionSelect").val(), iperiodo: 0, NomCerr: 0, Anio: inputAnio.value };
            if (CheckPeridoEspc.checked == true) {
                LoadPeriodosEspeciales(dataSend);
            }
            if (CheckPeridoEspc.checked == false) {
                LoadPeriodos(dataSend);
            }
        } else {
            Swal.fire({ icon: 'warning', title: 'Ejecucion', text: 'Seleccione una definicion antes de continuar!' });
            CheckPeridoEspc.checked = false;
        }
    });

    /// CARGA PERIODOS ESPECIALES ✔
    LoadPeriodosEspeciales = (dataSend) => {
        $.ajax({
            url: "../Nomina/PeridoEsp",
            type: "POST",
            data: dataSend,
            success: function (data) {
                var periodosEspecialesCount = 0;
                var periodos;
                if (data[0].sMensaje == "success") {
                    for (i = 0; i < data.length; i++) {
                        if (data[i].sPeEspecial == "True") {
                            periodos += `<option value='${data[i].iPeriodo}'>${data[i].iPeriodo} Fecha del: ${data[i].sFechaInicio} al ${data[i].sFechaFinal}</option>`;
                        } else {
                            periodosEspecialesCount++;
                        }
                    }
                    if (periodosEspecialesCount > 0) {
                        Swal.fire({ icon: 'error', title: 'Ejecucion', text: 'No se tienen periodos especiales disponibles!' });
                        CheckPeridoEspc.checked = false;
                    } else {
                        $("#PeridoEje").html(periodos);
                    }
                }
                if (data[0].sMensaje == "error") {
                    Swal.fire({ icon: 'error', title: 'Ejecucion', text: 'Algunas empresas no tienen periodo disponible!' });
                    CheckPeridoEspc.checked = false;
                }
            },
        });
    }

    /// CARGA PERIODOS NORMALES ✔
    LoadPeriodos = (dataSend) => {
        $.ajax({
            url: "../Nomina/PeridoEsp",
            type: "POST",
            data: dataSend,
            success: function (data) {
                var periodosCount = 0;
                if (data[0].sMensaje == "success") {
                    $("#PeridoEje").empty();
                    //document.getElementById("PeridoEje").innerHTML += `<option value='${data[0].iPeriodo}'>${data[0].iPeriodo} Fecha del: ${data[0].sFechaInicio} al ${data[0].sFechaFinal}</option>`;

                    for (i = 0; i < data.length; i++) {
                        if (data[i].sPeEspecial == "False") {
                            document.getElementById("PeridoEje").innerHTML += `<option value='${data[i].iPeriodo}'>${data[i].iPeriodo} Fecha del: ${data[i].sFechaInicio} al ${data[i].sFechaFinal}</option>`;
                            periodosCount++;
                        }
                    }
                    if (periodosCount == 0) {
                        Swal.fire({ icon: 'error', title: 'Ejecucion', text: 'No se tienen periodos especiales disponibles!' });
                    }
                }
                if (data[0].sMensaje == "error") {
                    Swal.fire({ icon: 'error', title: 'Ejecucion', text: 'Algunas empresas no tienen periodo disponible!' });
                }
            },
        });
    }

    // Funcion de guardar ✔
    Fguardar = () => {
        const dataSend = { iIdDefinicionHd: $("#definicionSelect").val() };
        //console.log(dataSend);

        $.ajax({
            url: "../Nomina/ExitPerODedu",
            type: "POST",
            data: dataSend,
            success: (data) => {
                if (data[0] == 1) {
                    DatoEjeCerrada = 0;
                    if ($("#definicionSelect").val() > 0) {

                        $.ajax({
                            url: "../Nomina/CompruRegistroExitdef",
                            type: "POST",
                            data: dataSend,
                            success: (data) => {
                                if (data[0].iIdCalculosHd == 1) {
                                    exitRow = "1";
                                }
                                if (data[0].iIdCalculosHd == 0) {
                                    exitRow = "0";
                                }
                                if (exitRow == "1") {

                                }
                                if (exitRow == "0") {
                                    const dataSend2 = { iIdDefinicionHd: $("#definicionSelect").val(), iNominaCerrada: DatoEjeCerrada };
                                    $.ajax({
                                        url: "../Nomina/InsertDatTpCalculos",
                                        type: "POST",
                                        data: dataSend2,
                                        success: (data) => {
                                            if (data.sMensaje == "success") {
                                                FLimpiaCamp();
                                                fshowtypealert('Correcto!', 'Se agrego correctamente para primer calculo', 'success');
                                            }
                                            else {
                                                fshowtypealert('Error', 'Contacte a sistemas', 'error');
                                            }
                                        },
                                        error: function (jqXHR, exception) {
                                            fcaptureaerrorsajax(jqXHR, exception);
                                        }
                                    });
                                }
                            },
                        });
                    }
                    else {
                        fshowtypealert('Selecciona un nombre definición !', '', 'warning');
                    }
                }
                if (data[0] == 0) {

                    fshowtypealert('Ejecución', 'La definicon debe tener por lo menos una percepcion o una deduccion para ser registrada y ejecutada', 'warning')
                }
            },
        });
    };

    btnFloGuardar.addEventListener('click', Fguardar);

    ///Checa si termina de realizar la ejecucion

    Frefresh = () => {
        btnFloEjecutar.style.visibility = 'visible';
        btnFloEjecutar.value = 1;
        $('#jqxLoader').jqxLoader('close');
        TipodePeridoDroplip = TxbTipoPeriodo.value;
        periodo = PeridoEje.options[PeridoEje.selectedIndex].text;
        separador = " "
        limite = 2
        arreglosubcadena2 = periodo.split(separador, limite);
        //FllenaCalculos(arreglosubcadena2[0], 0, TipodePeridoDroplip);
    };

    //btnFloActualiza.addEventListener('click', Frefresh)

    /* Proceso para cerrar nomina  */

    FValorChec = () => {
        if ($("#definicionSelect").val() < 1) {
            ChekEnFirme.checked = false;
            Swal.fire({
                title: "Validación",
                text: "Se necesita seleccionar una definición antes",
                icon: "warning",
                timer: 3000
            });
        } else {
            if (ChekEnFirme.checked == true) {

                Swal.fire({
                    title: 'Seguro que deseas correr la Nomina en firme?',
                    text: "Si es asi da clic en aceptar!",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Aceptar!'
                }).then((result) => {
                    if (result.value) {
                        periodo = PeridoEje.options[PeridoEje.selectedIndex].text;
                        separador = " "
                        limite = 2
                        arreglosubcadena2 = periodo.split(separador, limite);
                        periodo = PeridoEje.options[PeridoEje.selectedIndex].text;
                        var tipPer = TxbTipoPeriodo.value
                        separador = " "
                        limite = 2
                        arreglosubcadena3 = tipPer.split(separador, limite);

                        const dataSend = { iIdCalculosHd: $("#definicionSelect").val(), iTipoPeriodo: TxbTipoPeriodo.value, iPeriodo: $("#PeridoEje").val(), idEmpresa: 0, Anio: inputAnio.value, cart: 0 }

                        var rows;
                        $.ajax({
                            url: "../Nomina/ListTpCalculoln",
                            type: "POST",
                            data: dataSend,
                            success: (Result) => {
                                console.log(Result);
                                if (Result.Result[0].sMensaje == "success") {
                                    rows = Result.Result.length;

                                    if (rows > 0) {

                                        Swal.fire('', 'Realizando los calculos en firme', 'success');

                                        btnFloEjecutar.value = 1;
                                        IdDropList;
                                        AnioDropList;
                                        TipodePeridoDroplip = TxbTipoPeriodo.value;
                                        separador = " "
                                        limite = 2
                                        arreglosubcadena3 = TipodePeridoDroplip.split(separador, limite);
                                        periodo = PeridoEje.options[PeridoEje.selectedIndex].text;
                                        separador = " "
                                        limite = 2
                                        arreglosubcadena = periodo.split(separador, limite);
                                        const dataSend3 = { iIdDefinicionHd: $("#definicionSelect").val(), iPerido: $("#PeridoEje").val(), iNominaCerrada: 1, Anio: inputAnio.value, IdTipoPeriodo: 0, IdEmpresa: 0 };
                                        var dataSend2 = { IdDefinicionHD: $("#definicionSelect").val(), anio: inputAnio.value, iTipoPeriodo: TxbTipoPeriodo.value, iperiodo: $("#PeridoEje").val(), iIdempresa: 0, iCalEmpleado: checkCalculoEmplado, iNominaCe: 1 };

                                        $.ajax({
                                            url: "../Nomina/UpdateCInicioFechasPeriodo",
                                            type: "POST",
                                            data: dataSend3,
                                            success: (data) => {
                                                console.log("Respuesta UpdateCInicioFechasPeriodo");
                                                console.log(data);
                                                if (data.sMensaje == "success") {
                                                    $.ajax({
                                                        timeout: 15000,
                                                        url: "../Nomina/ProcesosPots",
                                                        type: "POST",
                                                        data: dataSend2,
                                                        success: (data2) => {
                                                            tabProcesos.ajax.reload();
                                                            if (data2.sMessage == 'success') {


                                                                //FllenaCalculos(arreglosubcadena2[0], 0, TipodePeridoDroplip);
                                                            }
                                                            else {
                                                                fshowtypealert("Ejecucion", "La Nomina no se ejecuto, Intente de nuevo, \n" + data2.sRespuesta, "warning")
                                                            }
                                                            FLimpiaCamp();
                                                        }
                                                    });
                                                }
                                                else {
                                                    fshowtypealert('Error', 'Contacte a sistemas', 'error');
                                                    FLimpiaCamp();
                                                }
                                            },
                                            error: function (jqXHR, exception) {
                                                fcaptureaerrorsajax(jqXHR, exception);
                                            }
                                        });
                                    }
                                }
                                else {
                                    ChekEnFirme.checked = false;
                                    Swal.fire('La Nomina!', 'No contiene ningun calculo , no se puede cerrar', 'warning');
                                    FLimpiaCamp();
                                }
                            }
                        });
                    }
                    else {
                        ChekEnFirme.checked = false;
                    }
                });
            };
        }
    };

    ChekEnFirme.addEventListener('click', FValorChec);

    /*  Procesos de Ejecucion */
    // validaciones para ejecucion
    Fejecucion = () => {

        btnFloEjecutar.value = 1;
        AnioDropList;
        TipodePeridoDroplip = TxbTipoPeriodo.value;
        separador = " "
        limite = 2
        arreglosubcadena3 = TipodePeridoDroplip.split(separador, limite);
        periodo = PeridoEje.options[PeridoEje.selectedIndex].text;
        separador = " "
        limite = 2

        arreglosubcadena2 = periodo.split(separador, limite);
        const dataSend = { iIdDefinicionHd: $("#definicionSelect").val() };
        var dataSend2 = { IdDefinicionHD: $("#definicionSelect").val(), anio: inputAnio.value, iTipoPeriodo: TxbTipoPeriodo.value, iperiodo: $("#PeridoEje").val(), iIdempresa: 0, iCalEmpleado: checkCalculoEmplado, iNominaCe: 0 };
        var dataSend3 = { Idempresas: empresa, anio: inputAnio.value, Tipodeperido: TxbTipoPeriodo.value, Periodo: $("#PeridoEje").val(), IdDefinicionHD: $("#definicionSelect").val() };

        $.ajax({
            url: "../Nomina/ProcesEjecuEsta",
            type: "POST",
            data: dataSend2,
            success: (data) => {
                //console.log(data);
                if (data[0].sEstatusJobs == "0") {
                    if (CheckCalculoEmpresa == 0) {
                        if ($("#definicionSelect").val() != 0) {
                            dataSend2 = { IdDefinicionHD: $("#definicionSelect").val(), anio: inputAnio.value, iTipoPeriodo: TxbTipoPeriodo.value, iperiodo: $("#PeridoEje").val(), iIdempresa: 0, iCalEmpleado: checkCalculoEmplado, iNominaCe: 0 };
                            FejecutarProceso(dataSend, dataSend2, dataSend3);
                        }
                        else {
                            fshowtypealert("Ejecucion", " Favor de seleccionar una definicion", "warning")
                        }
                    }
                    if (CheckCalculoEmpresa == 1) {
                        if (checkCalculoEmplado == 0) {
                            if (EjeEmpresa.value != 0) {
                                dataSend2 = { IdDefinicionHD: $("#definicionSelect").val(), anio: inputAnio.value, iTipoPeriodo: TxbTipoPeriodo.value, iperiodo: $("#PeridoEje").val(), iIdempresa: EjeEmpresa.value, iCalEmpleado: checkCalculoEmplado, iNominaCe: 0 };
                                FejecutarProceso(dataSend, dataSend2, dataSend3);
                            }
                            else {
                                fshowtypealert("Ejecucion", " Favor de seleccionar una empresa", "warning")
                            }
                        }
                        if (checkCalculoEmplado == 1) {
                            var NumItemsEmpleado = checkedItemsIdEmpleados.length;

                            if (NumItemsEmpleado != 0) {
                                const dataSend4 = { IdEmpresa: EjeEmpresa.value, iAnio: inputAnio.value, TipoPeriodo: TxbTipoPeriodo.value, iPeriodo: $("#PeridoEje").val(), sIdEmpleados: checkedItemsIdEmpleados, iNominaCe: 0 };
                                $.ajax({
                                    url: "../Nomina/SaveEmpleados",
                                    type: "POST",
                                    data: dataSend4,
                                    success: function (data) {
                                        if (data.sMensaje == "success") {

                                            FejecutarProceso(dataSend, dataSend2, dataSend3);
                                        }
                                        if (data.sMensaje == "error") {
                                            fshowtypealert("Ejecucion", " Contacte a sistemas", "warning")

                                        }
                                    }
                                });
                            }
                            else {
                                fshowtypealert("Ejecucion", " Favor de seleccionar por lo menos un empleado", "warning")
                            }
                        }
                    }
                }

                if (data[0].sEstatusJobs == "1") {

                    fshowtypealert("Calculo de Nomina", "se está realizando los calculo de una definicion de nómina espera 10 min y vuelva intentarlo", "warning")

                }
                FLimpiaCamp();
                tabProcesos.ajax.reload();
            },
        });
    };

    FTimepo = () => {

        btnFloEjecutar.style.visibility = "hidden";
        Fejecucion();
        let identificadorIntervaloDeTiempo;
        function repetirCadaSegundo() {
            identificadorIntervaloDeTiempo = setInterval('', 25000);
            setTimeout(() => { clearInterval(identificadorIntervaloDeTiempo); btnFloEjecutar.style.visibility = "visible"; }, 27000);

        }

    }

    btnFloEjecutar.addEventListener('click', Fejecucion);

    /////////////////////////////////////////////////

    /* Funcion de ejecucion */

    FejecutarProceso = (dataSend, dataSend2, dataSend3) => {
        try {
            $.ajax({
                url: "../Nomina/CompruRegistroExitdef",
                type: "POST",
                data: dataSend,
                success: (data) => {
                    console.log(data);
                    if (data[0].iIdCalculosHd == 1) {
                        fshowtypealert2("Validacion", "Validando calculos de nomina", "success");

                        $.ajax({
                            url: "../Nomina/ProcesosPots",
                            type: "POST",
                            data: dataSend2,
                            success: (data2) => {
                                tabProcesos.ajax.reload();
                                if (data2.sMessage == 'success') {
                                    //FllenaCalculos(arreglosubcadena2[0], 0, TipodePeridoDroplip);
                                }
                                else {
                                    fshowtypealert("Ejecucion", "La Nomina no se ejecuto, Intente de nuevo, \n" + data2.sRespuesta, "warning");
                                }
                            }
                        });
                    }
                    if (data[0].iIdCalculosHd == 0) {
                        exitRow = "0";
                        fshowtypealert("Ejecucion", "La definicion de nomina seleccionada no esta guardada", "warning");
                    }
                },
            });
        } catch (e) {
            console.log(e);
            fshowtypealert("Ejecución", "La Nomina no se ejecuto, Intente de nuevo", "warning");
        }
    };

    ///// llena del dropList de empleados 

    $('#EjeEmpresa').change(function () {
        EmpleadoDEmp(EjeEmpresa.value);
    });

    EmpleadoDEmp = (IdEmpresa) => {
        var source = " ";
        const dataSend2 = { iIdEmpresa: IdEmpresa };
        $.ajax({
            url: "../Nomina/ListConIDEmplados",
            type: "POST",
            data: dataSend2,
            success: (data) => {
                source = {
                    localdata: data,
                    datatype: "array",
                    datafields: [
                        { name: 'iIdEmpleado', type: 'int' },
                        { name: 'sNombreCompleto', type: 'string' },
                    ],
                    datatype: "array",
                };
                var dataAdapter = new $.jqx.dataAdapter(source);
                $("#DropLitEmple").jqxDropDownList({
                    checkboxes: true,
                    filterable: true,
                    searchMode: "containsignorecase",
                    selectedIndex: 0,
                    source: dataAdapter,
                    displayMember: "sNombreCompleto",
                    valueMember: "iIdEmpleado",
                    height: 30
                });

            },
        });
    };

    ////// selecccion de los empleado de la empresa

    /* muestra calculos de nomina del empleado */

    // funcion del buscador del empleado 
    FListEmpleado = (iIdempresa, iIdTipoPerido, iIdPeriodo) => {
        const ListEmple = { iIdEmpresa: iIdempresa, TipoPeriodo: iIdTipoPerido, periodo: iIdPeriodo, Anio: inputAnio.value };
        $.ajax({
            url: "../Empleados/DataListEmpleado",
            type: "POST",
            data: ListEmple,
            success: (data) => {
                if (data.length > 0) {
                    var source = {
                        localdata: data,
                        datatype: "array",
                        datafields: [
                            { name: 'iIdEmpleado' },
                            { name: 'sNombreCompleto' }
                        ]
                    };
                    var dataAdapter = new $.jqx.dataAdapter(source);
                    $("#jqxInput").empty();
                    $("#jqxInput").jqxInput({ source: dataAdapter, placeHolder: " Nombre del Empleado", displayMember: "sNombreCompleto", valueMember: "iIdEmpleado", width: 250, height: 30, minLength: 1 });
                    $("#jqxInput").on('select', function (event) {
                        if (event.args) {
                            var item = event.args.item;
                            if (item) {
                                var valueelement = $("<div></div>");
                                valueelement.text("Value: " + item.value);
                                var labelelement = $("<div></div>");
                                labelelement.text("Label: " + item.label);
                                NoEmpleado = item.value;
                                NombreEmpleado = item.label;
                            }
                        }
                    });
                }
                else {
                    $("#jqxInput").empty();
                    $("#jqxInput").jqxInput({ source: null, placeHolder: " Nombre del Empleado", displayMember: "sNombreCompleto", valueMember: "iIdEmpleado", width: 250, height: 30, minLength: 1 });
                }

            }
        });


    };

    $('#EmpresaNom').change(function () {
        var tipoPeriodo = TxbTipoPeriodo.value
        separador = " "
        limite = 2
        arreglosubcadena = tipoPeriodo.split(separador, limite);
        const dataSend5 = { iIdEmpresa: EmpresaNom.value, TipoPeriodo: arreglosubcadena[0], periodo: PeriodoCal.value, Anio: inputAnio.value };
        FListEmpleado(EmpresaNom.value, arreglosubcadena[0], PeriodoCal.value);
    });

    FBusNom = () => {

        //FDelettable();

        var TotalPercep = 0;
        var TotalDedu = 0;
        var Total = 0;
        var IdEmpresa = EmpresaNom.value;
        NombreEmpleado;
        var periodo = PeridoEje.options[PeridoEje.selectedIndex].text;
        separador = " ",
            limite = 2,
            arreglosubcadena = periodo.split(separador, limite);
        NoEmpleado;
        TipodePeridoDroplip = TxbTipoPeriodo.value;
        separador = " ",
            limite = 2,
            arreglosubcadena3 = TipodePeridoDroplip.split(separador, limite);
        const dataSend2 = { iIdEmpresa: IdEmpresa, iIdEmpleado: NoEmpleado, ianio: AnioDropList, iTipodePerido: arreglosubcadena3[0], iPeriodo: arreglosubcadena[0], iespejo: 0 };
        FGridCalculos(dataSend2);
        btnFloGuardar.style.visibility = 'hidden';
        btnFloEjecutar.style.visibility = 'hidden';

    };

    FGridCalculos = (dataSend2) => {

        $.ajax({
            url: "../Empleados/ReciboNomina",
            type: "POST",
            data: dataSend2,
            success: (data) => {
                var source =
                {
                    localdata: data,
                    datatype: "array",
                    datafields:
                        [
                            { name: 'sConcepto', type: 'string' },
                            { name: 'dPercepciones', type: 'number' },
                            { name: 'dDeducciones', type: 'number' },
                            { name: 'dGravados', type: 'number' },
                            { name: 'dExcento', type: 'number' }
                        ]
                };

                var dataAdapter = new $.jqx.dataAdapter(source);
                $("#TbCalculosNom").jqxGrid(
                    {
                        theme: 'bootstrap',
                        width: 700,
                        source: dataAdapter,
                        showfilterrow: true,
                        filterable: true,
                        sortable: true,
                        pageable: true,
                        autoloadstate: false,
                        autosavestate: false,
                        columnsresize: true,
                        showgroupaggregates: true,
                        showstatusbar: true,
                        showaggregates: true,
                        statusbarheight: 25,
                        columns: [
                            { text: 'Concepto', datafield: 'sConcepto', width: 280 },
                            {
                                text: 'Percepciones', datafield: 'dPercepciones', aggregates: ["sum"], width: 100, cellsformat: 'c2', cellsrenderer: function (row, column, value, defaultRender, column, rowData) {
                                    if (value.toString().indexOf("Sum") >= 0) {
                                        return defaultRender.replace("Sum", "Total");
                                    }
                                },
                                aggregatesrenderer: function (aggregates, column, element) {
                                    var renderstring = '<div style=" margin-top: 4px; margin-right:5px; text-align: left; overflow: hidden;">' + "Total" + ': ' + aggregates.sum + '</div>';
                                    return renderstring;
                                }
                            },
                            {
                                text: 'Deducciones ', datafield: 'dDeducciones', aggregates: ["sum"], width: 100, cellsformat: 'c2', cellsrenderer: function (row, column, value, defaultRender, column, rowData) {
                                    if (value.toString().indexOf("Sum") >= 0) {
                                        return defaultRender.replace("Sum", "Total");
                                    }
                                },
                                aggregatesrenderer: function (aggregates, column, element) {

                                    var renderstring = '<div style=" margin-top: 4px; margin-right:5px; text-align:left; overflow: hidden;">' + "Total" + ': ' + aggregates.sum + '</div>';
                                    return renderstring;
                                }
                            },
                            {
                                text: 'Gravado', datafield: 'dGravados', /*aggregates: ["sum"],*/ width: 100, cellsformat: 'c2' /*cellsrenderer: function (row, column, value, defaultRender, column, rowData) {*/
                            },
                            {
                                text: 'Excento', datafield: 'dExcento', /*aggregates: ["sum"],*/ width: 100, cellsformat: 'c2'
                            }

                        ]
                    });
            }
        });
        $.ajax({
            url: "../Empleados/TotalesRecibo",
            type: "POST",
            data: dataSend2,
            success: (data) => {
                if (data.length > 0) {
                    for (i = 0; i < data.length; i++) {
                        if (data[i].iIdRenglon == 990) {
                            TotalPercep = data[i].dSaldo
                            //$('#LaTotalPer').html(TotalPercep);
                        }
                        if (data[i].iIdRenglon == 1990) {

                            TotalDedu = data[i].dSaldo
                            //$('#LaTotalDedu').html(TotalDedu);
                        }
                    }

                    Tbtotal.style.visibility = 'visible';
                    LaTotal.style.visibility = 'visible';
                    Total = TotalPercep - TotalDedu;
                    Tbtotal.value = formatter.format(Total)

                }
            }
        });
    };

    /// llena  grid de calculos recibo dos
    FRecibo2 = () => {
        if (valorCheckRec.checked == true) {
            const dataSend2 = { iIdEmpresa: EmpresaNom.value, iIdEmpleado: NoEmpleado, ianio: AnioDropList, iTipodePerido: arreglosubcadena3[0], iPeriodo: arreglosubcadena[0], iespejo: 1 };
            FGridCalculos(dataSend2);
        }
        if (valorCheckRec.checked == false) {
            const dataSend2 = { iIdEmpresa: EmpresaNom.value, iIdEmpleado: NoEmpleado, ianio: AnioDropList, iTipodePerido: arreglosubcadena3[0], iPeriodo: arreglosubcadena[0], iespejo: 0 };
            FGridCalculos(dataSend2);
        }
    };

    /* Tab Nom Cerradas */

    /* Funcion muestra Grid Con los datos de TPDefinicion en del droplist definicion */

    FLlenaGrid2 = () => {

        for (var i = 0; i <= RowsGrid; i++) {

            $("#TpDefinicion2").jqxGrid('deleterow', i);
        }

        var opDeNombre = "Selecciona"; /*EjeNombreDef.options[EjeNombreDef.selectedIndex].text*/
        var opDeCancelados = 2;
        $.ajax({
            url: "../Nomina/QryDifinicionPeriodoCerrado",
            type: "POST",
            data: JSON.stringify(),
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                if (data.length > 0) {
                    RowsGrid = data.length;
                }
                var source =
                {
                    localdata: data,
                    datatype: "array",
                    datafields:
                        [
                            { name: 'iIdDefinicionhd', type: 'int' },
                            { name: 'sNombreDefinicion', type: 'string' },
                            { name: 'sDescripcion', type: 'string' },
                            { name: 'iAno', type: 'int' },
                            { name: 'iCancelado', type: 'string' },
                        ],
                    datatype: "array",
                    updaterow: function (rowid, rowdata) {
                        // synchronize with the server - send update command   
                    }
                };

                var dataAdapter = new $.jqx.dataAdapter(source);
                //////////////////////////////////////
                $("#TpDefinicion2").jqxGrid({
                    width: 550,
                    height: 200,
                    source: dataAdapter,
                    columnsresize: true,
                    columns: [
                        { text: 'No. Registro', datafield: 'iIdDefinicionhd', width: 50 },
                        { text: 'Nombre de Definición', datafield: 'sNombreDefinicion', width: 100 },
                        { text: 'Descripción ', datafield: 'sDescripcion', whidth: 300 },
                        { text: 'Año', datafield: 'iAno', whidt: 50 },
                        { text: 'Cancelado', datafield: 'iCancelado', whidt: 50 },
                    ]
                });
            },
        });
    };

    //FLlenaGrid2();
    /* FUNCION QUE MUESTRA ALERTAS */

    fshowtypealert = (title, text, icon) => {
        Swal.fire({
            title: title, text: text, icon: icon,
            showClass: { popup: 'animated fadeInDown faster' },
            hideClass: { popup: 'animated fadeOutUp faster' },
            confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,

        }).then((acepta) => {
        });
    };

    fshowtypealert2 = (title, text, icon) => {
        Swal.fire({
            title: title,
            text: text,
            icon: icon,
            showConfirmButton: false,
            timer: 5000,
            timerProgressBar: true
        });
    };

    /// Selecciona calculo por empresa

    FCheckXempresa = () => {

        if (valorCheckxempresa.checked == true) {
            CheckXempleado.style.visibility = 'visible';
            LaCheckXEmpleado.style.visibility = 'visible';
            CheckCalculoEmpresa = 1;
            //EjeEmpresa
            $("#DropLitEmple").jqxDropDownList('uncheckAll');
            // LaEmplea.style.visibility = 'visible';
            $("#switchButtonEmple").toggle();
            NombEmpre.style.visibility = 'visible';
            EjeEmpresa.style.visibility = 'visible';
            EjeEmpresa.value = 0;
            EmpleadoDEmp(EjeEmpresa.value);
            Empleadoseje.style.visibility = 'hidden';
            dropEmpledos.style.visibility = 'hidden';
            CheckXempleado.checked = false;
        }
        if (valorCheckxempresa.checked == false) {
            CheckXempleado.style.visibility = 'hidden';
            LaCheckXEmpleado.style.visibility = 'hidden';
            CheckCalculoEmpresa = 0;
            $("#DropLitEmple").jqxDropDownList('uncheckAll');
            //LaEmplea.style.visibility = 'hidden';
            $("#switchButtonEmple").toggle();
            //switchButtonEmp.style.visibility = 'hidden';
            NombEmpre.style.visibility = 'hidden';
            EjeEmpresa.style.visibility = 'hidden';
            EmpleadoDEmp(EjeEmpresa.value);
            Empleadoseje.style.visibility = 'hidden';
            dropEmpledos.style.visibility = 'hidden';
            CheckXempleado.checked = false;
        }

    };

    CheckXEmpresa.addEventListener('click', FCheckXempresa);

    /// selecciona calculo por empleado

    Fcheckxempleado = () => {
        if (valorCheckXempleado.checked == true) {
            checkCalculoEmplado = 1;
            $("#DropLitEmple").jqxDropDownList('uncheckAll');
            Empleadoseje.style.visibility = 'visible';
            dropEmpledos.style.visibility = 'visible';
        };
        if (valorCheckXempleado.checked == false) {
            checkCalculoEmplado = 0;
            $("#DropLitEmple").jqxDropDownList('uncheckAll');
            Empleadoseje.style.visibility = 'hidden';
            dropEmpledos.style.visibility = 'hidden';
        };
    };

    CheckXempleado.addEventListener('click', Fcheckxempleado)

    $("#DropLitEmple").on('checkChange', function (event) {
        if (event.args) {
            var item = event.args.item;
            if (item) {
                var valueelement = $("<div></div>");
                valueelement.text("Value: " + item.value);
                var labelelement = $("<div></div>");
                labelelement.text("Label: " + item.label);
                var checkedelement = $("<div></div>");
                checkedelement.text("Checked: " + item.checked);
                $("#selectionlog").children().remove();

                var items = $("#DropLitEmple").jqxDropDownList('getCheckedItems');
                var checkedItems = "";
                checkedItemsIdEmpleados = "";
                $.each(items, function (index) {
                    checkedItems += this.label + ", ";
                    checkedItemsIdEmpleados += this.value + ",";
                });
                $("#checkedItemsLog").text(checkedItems);
            }
        }
    });

    //// seleciona el empleado
    $("#jqxInput").on('select', function (event) {
        if (event.args) {
            var item = event.args.item;
            if (item) {
                var valueelement = $("<div></div>");
                valueelement.text("Value: " + item.value);
                var labelelement = $("<div></div>");
                labelelement.text("Label: " + item.label);
                NoEmpleado = item.value;
                NombreEmpleado = item.label;
            }
        }

    });

    /// MONITOR DE PROCESOS

    var tabProcesos;
    tabProcesos = $('#TbProcesos').DataTable({
        ajax: {
            url: "../Nomina/LoadMonitorProcesos",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            dataSrc: "process",
            dataType: "json"
        },
        "columns": [
            { "data": "iIdTarea" },
            { "data": "sNombreDefinicion" },
            { "data": "sUsuario" },
            { "data": "sFechaIni" },
            { "data": "sFechaFinal" },
            { "data": "sEstatusJobs" },
            { "data": "sEstatusFinal" }
            //{ "data": "sMensaje" }
        ],
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json",
            "infoEmpty": "Sin procesos ejecutados hoy",
        },
        ordering: false,
    });
    ////////////////////////////
    var shutDown = true;
    /// COUNTDOWN
    var count = 60;
    //var counter = null;
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
        var time = $("#timer").html();
        if (time == "00") {
            isContainerVisible(false);
            shutDown = true;
            count = 60;
            counter = setInterval(timer, 1000);
            tabProcesos.ajax.reload();
            isContainerVisible(true);
        }
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