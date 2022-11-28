$(function () {
    $("#tabDef").fadeOut();


    /// declaracion de variables

    let tabbody = document.getElementById('tabbody');

    btnAgreDef = document.getElementById('btnAgreDef');


    //// declaracion de variables modal Agre/Actualiza


    const NombreDe = document.getElementById('NombreDe');
    const DescripcionDe = document.getElementById('DescripcionDe');
    const AnoDe = document.getElementById('AnoDe');
    const DropCancelado = document.getElementById('DropCancelado');

    var IdDh;
    var DatoNombrede;
    var DatoDescripcion;
    var Datoano;
    var DatoCancel;

    /// fuciones


    /// llena tabla de definiciones

    Fllenagrip = () => {
        $("#tabbody").empty();
        $.ajax({
            url: "../Nomina/TpDefinicionNomina",
            type: "POST",
            data: JSON.stringify(),
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    TR = data[i].TR;
                    TD = data[i].TD;
                    TD = TD.replace(/ AAA /g, '"');
                    document.getElementById('tabbody').innerHTML += TR;
                    document.getElementById(data[i].iIdDefinicionhd + 'TbId').innerHTML += TD;

                }


            }
        });

        setTimeout(function () {
            tabCargaMasiva = $('#tabDef').DataTable({
                "language": {
                    "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json"
                },
                "lengthMenu": [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Todos"]]
            });
            $("#tabDef").fadeIn();
        }, 2000);



    };
    Fllenagrip();



    FPrueba = (e) => {

        $("#BtnModal").remove();
        document.getElementById('Btn_modal').innerHTML += '<div id="BtnModal"> </div>';

        document.getElementById('BtnModal').innerHTML += '<button data - toggle="modal" data-target="#ActualizarDefinicion" type = "button" onclick="FActualiza()" class="btn btn-outline-success btn-sm" id = "btnActualizarDefinicion"  > <i class="fas fa-plus mr-2"></i> Actualizar Definición</button >  <button type="button" class="btn btn-sm btn-outline-secondary" data-dismiss="modal"  onclick="CerrarPantDef()" id="btnCierraDefinicion"> <i class="fas fa-times mr-2"></i> Cerrar</button> ';
        let DatFila = document.getElementById(e + "TbId");
        let ElemFila = DatFila.getElementsByTagName("td");

        IdDh = ElemFila[0].innerHTML;
        DatoNombrede = ElemFila[1].innerHTML;
        DatoDescripcion = ElemFila[2].innerHTML;
        Datoano = ElemFila[3].innerHTML;



        NombreDe.value = ElemFila[1].innerHTML;
        DescripcionDe.value = ElemFila[2].innerHTML;
        AnoDe.value = ElemFila[3].innerHTML;

        if (ElemFila[3].innerHTML == "No") {
            DropCancelado.value = 0;
            DatoCancel = 0;
        }
        if (ElemFila[3].innerHTML == "Si") {
            DropCancelado.value = 1;
            DatoCancel = 1;
        }

    };

    FActualiza = (e) => {
        var opcion = 0;

        var opselesc = DropCancelado.options[DropCancelado.selectedIndex].text;
        if (DatoNombrede != NombreDe.value || DatoDescripcion != DescripcionDe.value || Datoano != AnoDe.value || DatoCancel != opselesc) {
            if (Datoano != AnoDe.value) {
                opcion = 1;
            }
            const dataSend = {
                sNombreDefinicion: NombreDe.value, sDescripcion: DescripcionDe.value,
                iAno: AnoDe.value, iCancelado: DropCancelado.value, iIdDefinicionhd: IdDh, OptAnio: opcion,
            };

            $.ajax({
                url: "../Nomina/UpdatePtDefinicion",
                type: "POST",
                data: dataSend,
                success: function (data) {
                    if (data.sMensaje == "success") {
                        fshowtypealert('Actualizacion correcta!', 'Definicion plantilla actualizada', 'success');
                        Fllenagrip();
                        FRecargaGrip();


                    } else {
                        fshowtypealert('Error', 'Contacte a sistemas', 'error');
                    }
                },

            });
        }
    };

    // Fcerrar Cierra la pantallad Modal  definicion

    CerrarPantDef = () => {
        NombreDe.value = '';
        DescripcionDe.value = '';
        AnoDe.value = '';
        DropCancelado.value = '';
    };

    FAgregaDef = () => {
        $("#BtnModal").remove();
        document.getElementById('Btn_modal').innerHTML += '<div id="BtnModal"> </div>';
        document.getElementById('BtnModal').innerHTML += '  <button data-toggle="modal" data-target="#AgregarDefinicion" type="button" class="btn btn-outline-success btn-sm" onclick="FAgregaDef()" id="btnAgregarDefinicion" style="visibility:visible"> <i class="fas fa-plus mr-2"></i> Agregar Definición</button> <button type="button" class="btn btn-sm btn-outline-secondary" data-dismiss="modal"  onclick="CerrarPantDef()" id="btnCierraDefinicion"> <i class="fas fa-times mr-2"></i> Cerrar</button> ';

    }

    btnAgreDef.addEventListener('click', FAgregaDef)



    // FAgrega datos de definicion en BD

    FAgregaDef = () => {
        if (NombreDe.value != "" && NombreDe.value != " " && DescripcionDe.value != "" && DescripcionDe.value != " " && AnoDe.value != "" && AnoDe.value != " ") {
            const dataSend = {
                sNombreDefinicion: NombreDe.value, sDescripcion: DescripcionDe.value,
                iAno: AnoDe.value, iCancelado: DropCancelado.value
            };

            $.ajax({
                url: "../Nomina/DefiNomina",
                type: "POST",
                data: dataSend,
                success: function (data) {
                    if (data.sMensaje == "success") {

                        fshowtypealert('Registro correcto!', 'Definicion plantilla guardada', 'success');
                        NombreDe.value = '';
                        DescripcionDe.value = '';
                        AnoDe.value = '';
                        Fllenagrip();
                        FRecargaGrip();

                    } else {
                        fshowtypealert('Error', 'Contacte a sistemas', 'error');
                    }
                },

            });

        }
        else {
            console.log('mesaje de error');
            fshowtypealert('warning', 'Introduce todos los campos', 'warning');

        }
    };


    //// selecciona definicion
    FSelectDefinicion = (e) => {
        document.getElementById('content-blockDedu/per').classList.remove("d-none");
        let DatFila = document.getElementById(e + "TbId");
        let ElemFila = DatFila.getElementsByTagName("td");

        IdDh = ElemFila[0].innerHTML;
        DatoNombrede = ElemFila[1].innerHTML;
        DatoDescripcion = ElemFila[2].innerHTML;
        Datoano = ElemFila[3].innerHTML;
        dato = ElemFila[1].innerHTML;
        if (ElemFila[3].innerHTML == "No") {

            DatoCancel = 0;
        }
        if (ElemFila[3].innerHTML == "Si") {
            DatoCancel = 1;
        }

        FcargaPercepciones();
        FcargaDeducionesGrip();



    };

    ////////// Parte de Persepciones

    var RegEmpresa = document.getElementById('RegEmpresa');
    var RegRenglon = document.getElementById('RegRenglon');

    var Tpoperiodo1 = document.getElementById('RegTipoperiodo1');
    var iRegEspejo = document.getElementById('RegEspejo');

    var AnioPre;
    var cancelado;
    var IdMaxDefNom;
    var DatoiId;
    var DatoEmpresa;
    var DatoRenglon;
    var datotipoperiodo;
    var datoespejo;




    const BActuPer = document.getElementById('BActuPer');
    const btnActualizarPercep = document.getElementById('btnActualizarPercep');
    const BEliminarPer = document.getElementById('BEliminarPer');
    const btnCierrapercepcion = document.getElementById('btnCierrapercepcion');
    const BAgregarPer = document.getElementById('BAgregarPer');


    ///// llega tabla percepciones
    const navPercepcionestab = document.getElementById('nav-Percepciones-tab');
    var RosCountPer;
    FcargaPercepciones = () => {

        $("#tabbodyPer").remove();
        document.getElementById('TablePerp').innerHTML += '<tbody id="tabbodyPer"> </tbody>';
        navPercepcionestab.style.visibility = "visible";

        const dataSend = { iIdDefinicionln: IdDh };
        $.ajax({
            url: "../Nomina/listdatosPercesiones",
            type: "POST",
            data: dataSend,
            success: function (data) {
                if (data[0].sMensaje == "success") {

                    for (i = 0; i < data.length; i++) {
                        TR = data[i].TR;
                        TD = data[i].TD;
                        TD = TD.replace(/ AAA /g, '"');
                        document.getElementById('tabbodyPer').innerHTML += TR;
                        document.getElementById(i + 'TbPerId').innerHTML += TD;

                    }

                    setTimeout(function () {
                        tabCargaMasiva = $('#TablePerp').DataTable({
                            "language": {
                                "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json"
                            },
                            "lengthMenu": [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Todos"]]
                        });
                        $("#TablePerp").fadeIn();
                    }, 2000);

                }
                if (data[0].sMensaje == "NotDat") {
                }
            }
        });
    };



    // fincion de llenado el droplist de empresa de persepciones
    LisEmpresa = () => {
        $.ajax({
            url: "../Nomina/LisEmpresas",
            type: "POST",
            data: JSON.stringify(),
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                BAgregarPer.value = data[0].iIdEmpresaSess;
                for (i = 0; i < data.length; i++) {
                    document.getElementById("RegEmpresa").innerHTML += `<option value='${data[i].iIdEmpresa}'>${data[i].sNombreEmpresa}</option>`;
                }
            }
        });
    };

    LisEmpresa();

    // llenado de lista droplist de tipo de periodo
    RecargaLisperiodo = () => {

        var op = RegEmpresa.options[RegEmpresa.selectedIndex].text;
        var opval = RegEmpresa.value;
        const dataSend = { IdEmpresa: opval };
        $("#RegTipoperiodo1").empty();
        $('#RegTipoperiodo1').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisTipPeriodo",
            type: "POST",
            data: dataSend,
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("RegTipoperiodo1").innerHTML += `<option value='${data[i].iId} '>${data[i].sValor}</option>`;
                }
            },
            error: function (jqXHR, exception) {
                fshowtypealert(jqXHR, exception);
            }
        });
    };

    // llenado del droplist de renglon

    RecargaLisRenglon = () => {

        var OpRenglon = RegEmpresa.options[RegEmpresa.selectedIndex].text;
        var opval = RegEmpresa.value;
        const dataSend = { IdEmpresa: opval, iElemntoNOm: 1 };
        $("#RegRenglon").empty();
        $('#RegRenglon').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisRenglon",
            type: "POST",
            data: dataSend,
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("RegRenglon").innerHTML += `<option value='${data[i].iIdRenglon} - ${data[i].iEspejo}'>${data[i].sNombreRenglon}</option>`;
                }
            },
         
        });

    };

    RecargaLisRenglon2 = (IdEmpresa) => {


        const dataSend = { IdEmpresa: IdEmpresa, iElemntoNOm: 1 };
        $("#RegRenglon").empty();
        $('#RegRenglon').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisRenglon",
            type: "POST",
            data: dataSend,
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("RegRenglon").innerHTML += `<option value='${data[i].iIdRenglon} - ${data[i].iEspejo}'>${data[i].sNombreRenglon}</option>`;
                }
            },
           
        });

    };

    $('#RegEmpresa').change(function () {

        RecargaLisperiodo();
        RecargaLisRenglon();
    });

    // Funcion que llenado del droplist Acumulado


    $('#RegRenglon').change(function () {

        RecargaLisAcumulado();

    });



    // FAgrega datos de percepcion en BD

    FBtnAgregaModal = () => {

        console.log('entro modal de boton perp');
        $("#BtnModal_Per").remove();
        document.getElementById('BtnModalPer').innerHTML += '<div id="BtnModal_Per"> </div>';
        document.getElementById('BtnModal_Per').innerHTML += '<button data-toggle="modal" data-target="#AgregarPersepcion" type="button"  onclick="FAgregaper()" class="btn btn-outline-success btn-sm" id="btnAgregarPercep" style="visibility:visible"> <i class="fas fa-plus mr-2"></i> Agregar Percepción</button>  <button type="button" class="btn btn-sm btn-outline-secondary" data-dismiss="modal"  onclick="CerrarPantDef()" id="btnCierrapercepcion"> <i class="fas fa-times mr-2"></i> Cerrar</button> ';


    };
    BAgregarPer.addEventListener('click', FBtnAgregaModal)


    FAgregaper = () => {

        var op = RegTipoperiodo1.options[RegTipoperiodo1.selectedIndex].text;
        if (RegEmpresa.value != "0" && RegRenglon.value != "0" && iRegEspejo.value != "0" && op != "Selecciona") {
            var ispejo;
            if (iRegEspejo.value == "1") {
                ispejo = "1";
            }
            else if (iRegEspejo.value == "2") {
                ispejo = "0";
            }
            const dataSend = { iIdFinicion: IdDh };

            $.ajax({
                url: "../Nomina/DefCancelado",
                type: "POST",
                data: dataSend,
                success: (data) => {
                    for (i = 0; i < data.length; i++) {
                        cancelado = data[i].iCancelado;
                    }
                    var opcanel1;
                    if (cancelado == "True" || cancelado == "true") {
                        opcanel1 = 1;
                    }
                    else if (cancelado == "False" || cancelado == "false") {
                        opcanel1 = 0;
                    }


                    const idempresa = RegEmpresa.value;
                    const idTipoPeriodo = RegTipoperiodo1.value;
                    //const idPeriodo = op2;
                    separador = "-",
                        limite = 2,
                        arreglosubcadena = RegRenglon.value.split(separador, limite);
                    const idRenglon = arreglosubcadena[0];
                    const icance = opcanel1;
                    const iEleNom = '39';
                    const dataSend2 = {

                        iIdDefinicionHd: IdDh, iIdEmpresa: idempresa,
                        iTipodeperiodo: idTipoPeriodo, /*iIdperiodo: idPeriodo,*/
                        iRenglon: idRenglon, iCancelado: icance, iElementonomina: iEleNom,
                        iEsespejo: ispejo, iIdAcumulado: ''
                    };
                    const dataSend3 = {
                        iIdDefinicionHd: IdDh, iIdEmpresa: idempresa,
                        iRenglon: idRenglon, iElementonomina: iEleNom
                    };
                    $.ajax({
                        url: "../Nomina/ExiteRenglon",
                        type: "POST",
                        data: dataSend3,
                        success: function (data) {
                            console.log(data[0].iIdDefinicionHd);
                            if (data[0].iIdDefinicionHd == 0) {
                                $.ajax({
                                    url: "../Nomina/insertDefinicioNl",
                                    type: "POST",
                                    data: dataSend2,
                                    success: function (data) {
                                        if (data.sMensaje == "success") {
                                            $("#TpDefinicion").click();
                                            FcargaPercepciones();
                                            fshowtypealert('Registro correcto!', ' Percepción Guardada', 'success');
                                            RegEmpresa.value = "0";
                                            RegRenglon.value = "0";
                                            RegTipoperiodo1.value = "0";
                                            RegEspejo.value = "0";

                                        } else {
                                            fshowtypealert('Error', 'Contacte a sistemas', 'error');
                                        }
                                    },
                                
                                });

                            }
                            if (data[0].iIdDefinicionHd > 0) {
                                fshowtypealert("Nomina/Percepción", "La percepcion ya esta ingresada favor de ingresar otra percepción diferente", "warning")
                            }


                        },
                      
                    });

                },
            });
        }

        else {
            fshowtypealert('warning', 'Los campos: Empresa, Renglo, Tipo de periodo y Es espejo son obligatorios', 'warning');

        }

    };


    // abre ventana y carga datos 
    botonActuPer = (e) => {
        $("#BtnModal_Per").remove();
        document.getElementById('BtnModalPer').innerHTML += '<div id="BtnModal_Per"> </div>';
        document.getElementById('BtnModal_Per').innerHTML += ' <button data-toggle="modal" data-target="#ActualizarDefinicion" type="button" class="btn btn-outline-success btn-sm" onclick="FactulizaPer()" id="btnActualizarPercep" style="visibility:visible"> <i class="fas fa-plus mr-2"></i> Actualizar Percepción</button>  <button type="button" class="btn btn-sm btn-outline-secondary" data-dismiss="modal"  onclick="FlimpicamposPEr()" id="btnCierrapercepcion"> <i class="fas fa-times mr-2"></i> Cerrar</button> ';


        FSelectPer(e);

        for (var i = 0; i < RegEmpresa.length; i++) {

            if (RegEmpresa.options[i].text == DatoEmpresa) {
                // seleccionamos el valor que coincide
                RegEmpresa.selectedIndex = i;
            }

        }
        var opvalEmpresa = RegEmpresa.value;
        const dataSend = { IdEmpresa: opvalEmpresa, iElemntoNOm: 1 };


        $("#RegRenglon").empty();
        $('#RegRenglon').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisRenglon",
            type: "POST",
            data: dataSend,
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    console.log('imprimerenglon');
                    document.getElementById("RegRenglon").innerHTML += `<option value='${data[i].iIdRenglon}'>${data[i].sNombreRenglon}</option>`;
                }

                for (i = 0; i < RegRenglon.length; i++) {
                    if (RegRenglon.options[i].text == DatoRenglon) {
                        // seleccionamos el valor que coincide
                        RegRenglon.selectedIndex = i;
                    }
                }


                OpRenglon = RegEmpresa.options[RegEmpresa.selectedIndex].text;



            },

        });

        var op = RegEmpresa.options[RegEmpresa.selectedIndex].text;
        var opval = RegEmpresa.value;
        const dataSend2 = { IdEmpresa: opval };
        $("#RegTipoperiodo1").empty();
        $('#RegTipoperiodo1').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisTipPeriodo",
            type: "POST",
            data: dataSend2,
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("RegTipoperiodo1").innerHTML += `<option value='${data[i].iId}'>${data[i].sValor}</option>`;
                }
                for (var i = 0; i < RegTipoperiodo1.length; i++) {
                    if (RegTipoperiodo1.options[i].text == datotipoperiodo) {
                        // seleccionamos el valor que coincide
                        RegTipoperiodo1.selectedIndex = i;
                    }

                }

                const dataSend4 = { iIdFinicion: IdDh };
                $.ajax({
                    url: "../Nomina/DefCancelado",
                    type: "POST",
                    data: dataSend4,
                    success: (data) => {
                        for (i = 0; i < data.length; i++) {
                            AnioPre = data[i].iAno;
                        }
                        var OpEmpresa = RegEmpresa.value;
                        const OpIdTipoperiodo = RegTipoperiodo1.value;
                        const anio = AnioPre;


                    }
                });

            },
            error: function (jqXHR, exception) {
                fcaptureaerrorsajax(jqXHR, exception);
            }
        });

        for (i = 0; i < RegEspejo.length; i++) {
            if (RegEspejo.options[i].text == datoespejo) {
                // seleccionamos el valor que coincide
                RegEspejo.selectedIndex = i;
            }

        }


    };

    //// seleccion percepcion
    FSelectPer = (e) => {
        let DatFila = document.getElementById(e + "TbPerId");
        let ElemFila = DatFila.getElementsByTagName("td");
        DatoiId = ElemFila[0].innerHTML;
        DatoEmpresa = ElemFila[1].innerHTML;
        DatoRenglon = ElemFila[2].innerHTML;
        datotipoperiodo = ElemFila[3].innerHTML;
        datoespejo = ElemFila[4].innerHTML;
    };


    // Funcion limpia los campos de percepcion de la pantalla de agregar Percepciones
    FlimpicamposPEr = () => {

        RegEmpresa.value = "0";
        RegRenglon.value = "0";
        RegTipoperiodo1.value = "0";
        RegEspejo.value = "0";
        //RegPeridoPer.value = "0";

    };



    //Funcion que guarda la actulalizacion de la percepcion en el BD

    FactulizaPer = () => {

        var EmpresaPer = RegEmpresa.options[RegEmpresa.selectedIndex].text;
        var EmpresaIDper = RegEmpresa.value;
        var RenglonPre = RegRenglon.options[RegRenglon.selectedIndex].text;
        separador = "-",
            limite = 2,
            arreglosubcadena = RegRenglon.value.split(separador, limite);
        var RenglonPerId = arreglosubcadena[0];
        var TipoPeriodoPre = Tpoperiodo1.options[Tpoperiodo1.selectedIndex].text;
        var TipoPeriodoPreId = Tpoperiodo1.value;
        var EspejoPre = iRegEspejo.options[iRegEspejo.selectedIndex].text;
        var EspejoPreId = iRegEspejo.value;
        //var PeriodoPer = iRegPeridoPer.options[iRegPeridoPer.selectedIndex].text;
        //var PeriodoPerId = iRegPeridoPer.value;




        if (DatoRenglon != RenglonPre && DatoEmpresa != EmpresaPer || datotipoperiodo != TipoPeriodoPre || datoespejo != EspejoPre  /*|| datoperiodo != PeriodoPer*/) {

            if (EspejoPreId == 2) { EspejoPreId = 0; }
            const dataSend2 = {
                iIdDefinicionln: DatoiId, iIdEmpresa: EmpresaIDper,
                iTipodeperiodo: TipoPeriodoPreId, /*iIdperiodo: PeriodoPer,*/
                iRenglon: RenglonPerId, iEsespejo: EspejoPreId, iIdAcumulado: 0
            };
            const dataSend3 = {
                iIdDefinicionHd: IdDh, iIdEmpresa: EmpresaIDper,
                iRenglon: RenglonPerId, iElementonomina: 39
            };

            $.ajax({
                url: "../Nomina/ExiteRenglon",
                type: "POST",
                data: dataSend3,
                success: function (data) {
                    console.log(data[0].iIdDefinicionHd);
                    if (data[0].iIdDefinicionHd == 0) {

                        $.ajax({
                            url: "../Nomina/UpdatePtDefinicionNl",
                            type: "POST",
                            data: dataSend2,
                            success: function (data) {
                                if (data.sMensaje == "success") {

                                    $("#TpDefinicion").click();
                                    FcargaPercepciones();
                                    fshowtypealert('Registro Actualizado!', 'Definicion de Percepción', 'success');

                                } else {
                                    fshowtypealert('Error', 'Contacte a sistemas', 'error');
                                }
                            },

                        });

                    }
                    if (data[0].iIdDefinicionHd == DatoiId) {

                        $.ajax({
                            url: "../Nomina/UpdatePtDefinicionNl",
                            type: "POST",
                            data: dataSend2,
                            success: function (data) {
                                if (data.sMensaje == "success") {

                                    $("#TpDefinicion").click();
                                    FcargaPercepciones();
                                    fshowtypealert('Registro Actualizado!', 'Percepción actualizada', 'success');

                                } else {
                                    fshowtypealert('Error', 'Contacte a sistemas', 'error');
                                }
                            },

                        });

                    }
                    if (data[0].iIdDefinicionHd != DatoiId) {
                        fshowtypealert("Nomina/Percepción", "La percepcion ya esta registrada ", "warning")

                    }

                },

            });
        }

        else {
            console.log('datos iguales');

        }

    };

    FDeleteDefinicionNLPer = (e) => {
        FSelectPer(e);
        const dataSend = {
            iIdDefinicion: DatoiId
        };
        $.ajax({
            url: "../Nomina/UpdateRenglonDefNl",
            type: "POST",
            data: dataSend,
            success: function (data) {
                if (data.sMensaje == "success") {
                    fshowtypealert('Percepción!', 'Regitro Eliminado', 'success');
                    $("#TpDefinicion").click();
                    FcargaPercepciones();


                } else {
                    fshowtypealert('Error', 'Contacte a sistemas', 'error');
                }
            },

        });
    }


    ////////// Parte de Deducciones


    FcargaDeducionesGrip = () => {

        console.log('Entra a deduc');
        $("#tabbodyDedu").remove();
        document.getElementById('TableDeduc').innerHTML += '<tbody id="tabbodyDedu"> </tbody>';
        const dataSend = { iIdDefinicionln: IdDh };
        console.log(dataSend)
        $.ajax({
            url: "../Nomina/listdatosDeducuiones",
            type: "POST",
            data: dataSend,
            success: function (data) {
                if (data[0].sMensaje == "success") {

                    for (i = 0; i < data.length; i++) {
                        TR = data[i].TR;
                        TD = data[i].TD;
                        TD = TD.replace(/ AAA /g, '"');
                        document.getElementById('tabbodyDedu').innerHTML += TR;
                        document.getElementById(i + 'TbDedId').innerHTML += TD;

                    }


                    setTimeout(function () {
                        tabCargaMasiva = $('#TableDeduc').DataTable({
                            "language": {
                                "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json"
                            },
                            "lengthMenu": [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Todos"]]
                        });
                        $("#TableDeduc").fadeIn();
                    }, 2000);

                }
                if (data[0].sMensaje == "NotDat") {
                }
            }
        });

    };



    // declaracion de Variables 

    var RegEmpresade = document.getElementById('RegEmpresaDe');
    var RegRenglonde = document.getElementById('RegRenglonDe');
    var Tpoperiodode = document.getElementById('RegTipoperiodoDe');
    const iRegEspejode = document.getElementById('RegEspejoDe');
    //var iRegPeridoDe = document.getElementById('RegPeridoDe');
    var AnioDeduc;
    var canceladode;


    // declaracion de botones 
    const BAgregardedu2 = document.getElementById('BAgregardedu2');
    const btnAgregarDedu = document.getElementById('btnAgregarDedu');
    const btnCierraDedu = document.getElementById('btnCierraDedu');
    const BActudedu = document.getElementById('BActudedu');
    const btnActualizarDedu = document.getElementById('btnActualizarDedu');
    const BEliminardedu = document.getElementById('BEliminardedu');



    // llena el drop lis de Empresa de Deduccion
    LisEmpresaDe = () => {

        $.ajax({
            url: "../Nomina/LisEmpresas",
            type: "POST",
            data: JSON.stringify(),
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("RegEmpresaDe").innerHTML += `<option value='${data[i].iIdEmpresa}'>${data[i].sNombreEmpresa}</option>`;
                }
            }
        });
    };

    LisEmpresaDe();

    // llenado de lista droplist de tipo de periodo 
    RecargaLisperiodoDe = () => {
        var op = RegEmpresade.options[RegEmpresade.selectedIndex].text;
        var opvalde = RegEmpresade.value;
        const dataSend = { IdEmpresa: opvalde };

        $("#RegTipoperiodoDe").empty();
        $('#RegTipoperiodoDe').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisTipPeriodo",
            type: "POST",
            data: dataSend,
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("RegTipoperiodoDe").innerHTML += `<option value='${data[i].iId}'>${data[i].sValor}</option>`;
                }
            },

        });

    };

    // llenado del droplist de renglon
    RecargaLisRenglonDe = () => {
        var OpRenglon = RegEmpresade.options[RegEmpresade.selectedIndex].text;
        var opvalEmpresaDe = RegEmpresade.value;
        const dataSend = { IdEmpresa: opvalEmpresaDe, iElemntoNOm: 2 };
        $("#RegRenglonDe").empty();
        $('#RegRenglonDe').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisRenglon",
            type: "POST",
            data: dataSend,
            success: (data) => {
                console.log(data + '1');
                for (i = 0; i < data.length; i++) {
                    document.getElementById("RegRenglonDe").innerHTML += `<option value='${data[i].iIdRenglon} - ${data[i].iEspejo}'>${data[i].sNombreRenglon}</option>`;
                }
            },
       
        });
    };

    // llenado del droplist de renglon
    RecargaLisRenglonDe2 = (Idempresade) => {
        var OpRenglon = RegEmpresade.options[RegEmpresade.selectedIndex].text;

        const dataSend = { IdEmpresa: Idempresade, iElemntoNOm: 2 };
        $("#RegRenglonDe").empty();
        $('#RegRenglonDe').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisRenglon",
            type: "POST",
            data: dataSend,
            success: (data) => {
                console.log('2 vuelve a entrar aqui');
                for (i = 0; i < data.length; i++) {
                    document.getElementById("RegRenglonDe").innerHTML += `<option value='${data[i].iIdRenglon} - ${data[i].iEspejo}'>${data[i].sNombreRenglon}</option>`;
                }
            },
       
        });
    };

    $('#RegEmpresaDe').change(function () {

        RecargaLisperiodoDe();
        RecargaLisRenglonDe();
    });





    FBtnAgregaModalDed = () => {
        $("#BtnModal_Ded").remove();
        document.getElementById('BtnModalDed').innerHTML += '<div id="BtnModal_Ded"> </div>';
        document.getElementById('BtnModal_Ded').innerHTML += ' <button data-toggle="modal" data-target="#Agregardedu" type="button" class="btn btn-outline-success btn-sm" id="btnAgregarDedu" onclick="FGuardarDedBD()" style="visibility:visible"> <i class="fas fa-plus mr-2"></i> Agregar Deducción</button>  <button type="button" class="btn btn-sm btn-outline-secondary" data-dismiss="modal"  onclick="FLimpiCamposDedu()" id="btnCierrapercepcion"> <i class="fas fa-times mr-2"></i> Cerrar</button> ';
    };
    BAgregardedu2.addEventListener('click', FBtnAgregaModalDed)



    // Funcion Guarda Deducion en BD

    FGuardarDedBD = () => {
        var op = RegTipoperiodoDe.options[RegTipoperiodoDe.selectedIndex].text;

        if (RegEmpresade.value != "0" && RegRenglonde.value != "0" && iRegEspejode.value != "0" && op != "Selecciona") {


            var ispejode;
            if (iRegEspejode.value == "1") {
                ispejode = "1";
            }
            else if (iRegEspejode.value == "2") {
                ispejode = "0";
            }
            const dataSend = { iIdFinicion: IdDh };
            $.ajax({
                url: "../Nomina/DefCancelado",
                type: "POST",
                data: dataSend,
                success: (data) => {

                    for (i = 0; i < data.length; i++) {
                        canceladode = data[i].iCancelado;
                    }
                    var opcanel;
                    if (canceladode == "True" || canceladode == "true") {
                        opcanel = 1;
                    }
                    else if (canceladode == "False" || canceladode == "false") {
                        opcanel = 0;
                    }
                    //var op2 = RegPeridoDe.options[RegPeridoDe.selectedIndex].text;
                    //if (op2 == "Selecciona") {
                    //    op2 = "0";
                    //}
                    const idempresade = RegEmpresade.value;
                    const idTipoPeriodode = Tpoperiodode.value;
                    //const idPeriodode = op2;
                    separador = "-",
                        limite = 2,
                        arreglosubcadena2 = RegRenglonDe.value.split(separador, limite);
                    const idRenglonde = arreglosubcadena2[0];
                    const icancede = opcanel;
                    const iEleNomde = '40';
                    const idAcumuladode = 0;

                    const dataSend2 = {
                        iIdDefinicionHd: IdDh, iIdEmpresa: idempresade,
                        iTipodeperiodo: idTipoPeriodode,/* iIdperiodo: idPeriodode,*/
                        iRenglon: idRenglonde, iCancelado: icancede, iElementonomina: iEleNomde,
                        iEsespejo: ispejode, iIdAcumulado: ''
                    };

                    const dataSend3 = {
                        iIdDefinicionHd: IdDh, iIdEmpresa: idempresade,
                        iRenglon: idRenglonde, iElementonomina: iEleNomde
                    };

                    $.ajax({
                        url: "../Nomina/ExiteRenglon",
                        type: "POST",
                        data: dataSend3,
                        success: function (data) {
                            if (data[0].iIdDefinicionHd == 0) {

                                $.ajax({
                                    url: "../Nomina/insertDefinicioNl",
                                    type: "POST",
                                    data: dataSend2,
                                    success: function (data) {
                                        if (data.sMensaje == "success") {
                                            fshowtypealert('Registro correcto!', 'Deduccion guardada', 'success');
                                            $("#TpDefinicion").click();
                                            FcargaDeducionesGrip();
                                            RegEmpresade.value = "0";
                                            RegRenglonde.value = "0";
                                            Tpoperiodode.value = "0";
                                            iRegEspejode.value = "0";
                                            //iRegPeridoDe.value = "0";

                                        } else {
                                            fshowtypealert('Error', 'Contacte a sistemas', 'error');
                                        }
                                    },
                                    error: function (jqXHR, exception) {
                                        /* fcaptureaerrorsajax(jqXHR, exception);*/
                                    }
                                });
                            }
                            if (data[0].iIdDefinicionHd > 0) {
                                fshowtypealert("Nomina/Deducción", "La deducción ya esta ingresada favor de ingresar otra deducción diferente", "warning")
                            }
                        },
                        error: function (jqXHR, exception) {
                            /*fcaptureaerrorsajax(jqXHR, exception);*/
                        }
                    });


                },
                error: function (jqXHR, exception) {
                    /*fcaptureaerrorsajax(jqXHR, exception);*/
                }
            });
        }
        else {
            fshowtypealert('warning', 'Los campos: Empresa, Renglo, Tipo de periodo, Periodo y Es espejo son obligatorios', 'warning');
        }
    };


    // Funcion limpia campos

    FLimpiCamposDedu = () => {

        RegEmpresade.value = "0";
        RegRenglonde.value = "0";
        Tpoperiodode.value = "0";
        iRegEspejode.value = "0";
        //iRegPeridoDe.value = "0";
    };

    // Funcion desaprece el boton de agregar y aparece el boton de actualizar y recarga datos
    FActualizaboton = (e) => {

        $("#BtnModal_Ded").remove();
        document.getElementById('BtnModalDed').innerHTML += '<div id="BtnModal_Ded"> </div>';
        document.getElementById('BtnModal_Ded').innerHTML += ' <button data-toggle="modal" data-target="#ActualizarDefinicion" type="button"   class=" class="btn btn-outline-success btn-sm" onclick="FActualizaDed()" id="btnActualizarDedu" > <i class="fas fa-plus mr-2"></i> Actualizar Dedución</button>  <button type="button" class="btn btn-sm btn-outline-secondary" data-dismiss="modal"  onclick="FLimpiCamposDedu()" id="btnCierrapercepcion"> <i class="fas fa-times mr-2"></i> Cerrar</button> ';
        FSelectDed(e)

        for (var i = 0; i < RegEmpresade.length; i++) {
            if (RegEmpresade.options[i].text == DatoEmpresade) {
                // seleccionamos el valor que coincide
                RegEmpresade.selectedIndex = i;
            }

        };

        var OpRenglon = RegEmpresaDe.options[RegEmpresaDe.selectedIndex].text;
        var opvalEmpresa = RegEmpresaDe.value;
        const dataSend = { IdEmpresa: opvalEmpresa, iElemntoNOm: 2 };


        $("#RegRenglonDe").empty();
        $('#RegRenglonDe').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({

            url: "../Nomina/LisRenglon",
            type: "POST",
            data: dataSend,
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("RegRenglonDe").innerHTML += `<option value='${data[i].iIdRenglon}'>${data[i].sNombreRenglon}</option>`;
                }
                console.log('Reno dedu' + RegRenglonDe.length);
                for (i = 0; i < RegRenglonDe.length; i++) {
                    if (RegRenglonDe.options[i].text == DatoRenglonde) {

                        // seleccionamos el valor que coincide

                        RegRenglonDe.selectedIndex = i;
                        i = 100000;
                    }
                }

                var opEmpresaval = RegEmpresaDe.value;
                OpRenglon = RegEmpresaDe.options[RegEmpresaDe.selectedIndex].text;



            },
        
        });


        var op = RegEmpresaDe.options[RegEmpresaDe.selectedIndex].text;
        var opvalde = RegEmpresaDe.value;
        const dataSend2 = { IdEmpresa: opvalde };

        $("#RegTipoperiodoDe").empty();
        $('#RegTipoperiodoDe').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisTipPeriodo",
            type: "POST",
            data: dataSend2,
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("RegTipoperiodoDe").innerHTML += `<option value='${data[i].iId}'>${data[i].sValor}</option>`;
                }

                for (var i = 0; i < RegTipoperiodoDe.length; i++) {
                    if (RegTipoperiodoDe.options[i].text == datotipoperiodode) {
                        // seleccionamos el valor que coincide
                        RegTipoperiodoDe.selectedIndex = i;

                    }

                }



            },
         
        });

        for (i = 0; i < RegEspejoDe.length; i++) {
            if (RegEspejoDe.options[i].text == datoespejode) {
                // seleccionamos el valor que coincide
                RegEspejoDe.selectedIndex = i;
            }

        }


    };



    FSelectDed = (e) => {
        let DatFila = document.getElementById(e + "TbDedId");
        let ElemFila = DatFila.getElementsByTagName("td");
        DatoiIdde = ElemFila[0].innerHTML;
        DatoEmpresade = ElemFila[1].innerHTML;
        DatoRenglonde = ElemFila[2].innerHTML;
        datotipoperiodode = ElemFila[3].innerHTML;
        datoespejode = ElemFila[5].innerHTML;
    };


    //// FGuardar Actualizacion de Deduccion en la BD

    FActualizaDed = () => {

        var Empresaded = RegEmpresaDe.options[RegEmpresaDe.selectedIndex].text;
        var EmpresaIDded = RegEmpresaDe.value;
        var Renglonded = RegRenglonDe.options[RegRenglonDe.selectedIndex].text;
        separador = "-",
            limite = 2,
            arreglosubcadena = RegRenglonDe.value.split(separador, limite);
        var RenglondedId = arreglosubcadena[0];
        var TipoPeriododed = RegTipoperiodoDe.options[RegTipoperiodoDe.selectedIndex].text;
        var TipoPeriododedId = RegTipoperiodoDe.value;
        var Espejoded = RegEspejoDe.options[RegEspejoDe.selectedIndex].text;
        var EspejodedId = RegEspejoDe.value


        if (DatoRenglonde != Renglonded && DatoEmpresade != Empresaded || datotipoperiodode != TipoPeriododed || datoespejode != Espejoded  /*|| datoperiodode != Periododed*/) {

            if (EspejodedId == 2) { EspejodedId = 0 }
            const dataSend2 = {
                iIdDefinicionln: DatoiIdde, iIdEmpresa: EmpresaIDded,
                iTipodeperiodo: TipoPeriododedId,/* iIdperiodo: Periododed,*/
                iRenglon: RenglondedId, iEsespejo: EspejodedId, iIdAcumulado: ''
            };
            const dataSend3 = {
                iIdDefinicionHd: IdDh, iIdEmpresa: EmpresaIDded,
                iRenglon: RenglondedId, iElementonomina: 40
            };
            $.ajax({
                url: "../Nomina/ExiteRenglon",
                type: "POST",
                data: dataSend3,
                success: function (data) {
                    if (data[0].iIdDefinicionHd == 0) {
                        $.ajax({
                            url: "../Nomina/UpdatePtDefinicionNl",
                            type: "POST",
                            data: dataSend2,
                            success: function (data) {
                                if (data.sMensaje == "success") {
                                    console.log('aaa');
                                    $("#TpDefinicion").click();
                                    FcargaDeducionesGrip();
                                    fshowtypealert('Registro Actualizado!', ' Deducción', 'success');

                                } else {
                                    fshowtypealert('Error', 'Contacte a sistemas', 'error');
                                }
                            },
                            error: function (jqXHR, exception) {
                                fcaptureaerrorsajax(jqXHR, exception);
                            }
                        });

                    }
                    if (data[0].iIdDefinicionHd == DatoiIdde) {
                        $.ajax({
                            url: "../Nomina/UpdatePtDefinicionNl",
                            type: "POST",
                            data: dataSend2,
                            success: function (data) {
                                if (data.sMensaje == "success") {

                                    $("#TpDefinicion").click();
                                    FcargaDeducionesGrip();
                                    fshowtypealert('Registro Actualizado!', ' Deducción', 'success');

                                } else {
                                    fshowtypealert('Error', 'Contacte a sistemas', 'error');
                                }
                            },
                            error: function (jqXHR, exception) {
                                fcaptureaerrorsajax(jqXHR, exception);
                            }
                        });

                    }
                    if (data[0].iIdDefinicionHd != DatoiIdde) {
                        fshowtypealert("Nomina/Deducción", "La deducción ya esta ingresada favor de ingresar otra deducción diferente", "warning")
                    }
                },
                error: function (jqXHR, exception) {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        }

        else {
            console.log('datos iguales');

        }
    };


    /// Elimina Dedución de la tabla TDefiniciones 
    FDeleteDefinicionNLdedu = (e) => {

        FSelectDed(e);

        const dataSend = {
            iIdDefinicion: DatoiIdde
        };
        console.log(dataSend);
        $.ajax({
            url: "../Nomina/UpdateRenglonDefNl",
            type: "POST",
            data: dataSend,
            success: function (data) {
                if (data.sMensaje == "success") {
                    fshowtypealert('Deducción!', 'Regitro Eliminado', 'success');
                    $("#TpDefinicion").click();
                    FcargaDeducionesGrip();


                } else {
                    fshowtypealert('Error', 'Contacte a sistemas', 'error');
                }
            },
        
        });

    };

    //// mensajes
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

});