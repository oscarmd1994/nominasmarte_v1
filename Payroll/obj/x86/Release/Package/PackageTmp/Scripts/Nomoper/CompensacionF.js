﻿$(function () {

    const btnNuevaComp = document.getElementById('btnNuevaComp');
    const DropEmpresa = document.getElementById('DropEmpresa');
    const DropPuesto = document.getElementById('DropPuesto');
    const DropRenglon = document.getElementById('DropRenglon');
    const ChekPYA = document.getElementById('ChekPYA');
    const TxtImporte = document.getElementById('TxtImporte');
    const TxtDesp = document.getElementById('TxtDesp');
    const btnRegistrar = document.getElementById('btnRegistrar');
    const btnCancelar = document.getElementById('btnCancelar');
    const btnActu = document.getElementById('btnActu');
    const BActuComp = document.getElementById('BActuComp');
    const BtDeletComp = document.getElementById('BtDeletComp');

    var valorCheckpya = document.getElementById('ChekPYA');
    var idTb;
    var IdEmpresaTb;
    var iPPyPAtb
    var Puestotb;
    var RenglonTB;
    var ImporteTb;
    var DescripcioTb;
    var clickEdit = 0;
    var clickDele = 0;

    /// el btnNuevaComp.value guarda el perfil de escritura o lectura del empleado
 

    /// carga tabla de compesacion fija 
    FTbCompensacion = () => {
        document.getElementById('content-tableComp').classList.remove("d-none");
        $.ajax({
            url: "../Nomina/CompFijasEmpre",
            type: "POST",
            data: JSON.stringify(),
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                
                if (data.Bandera == true) {
                    if (data.Datos[0].sMensaje == "success") {
                        var source =
                        {
                            localdata: data.Datos,
                            datatype: "array",
                            datafields:
                                [
                                    { name: 'iId', type: 'int' },
                                    { name: 'sNombreEmpresa', type: 'string' },
                                    { name: 'sPuesto', type: 'string' },
                                    { name: 'iPremioPyA', type: 'int' },
                                    { name: 'sNombreRenglon', type: 'string' },
                                    { name: 'iImporte', type: 'money' },
                                    { name: 'sDescripcion', type: 'string' },

                                ]
                        };
                        var dataAdapter = new $.jqx.dataAdapter(source);
                        $("#TBCompensacion").jqxGrid(
                            {
                                width: 840,
                                source: dataAdapter,
                                selectionmode: 'multiplerowsextended',
                                sortable: true,
                                pageable: true,
                                autoheight: true,
                                autoloadstate: false,
                                autosavestate: false,
                                columnsresize: true,
                                showtoolbar: true,
                                rendertoolbar: function (statusbar) {

                                    if (btnNuevaComp.value != "True") {
                                        var container = $("<div style='overflow: hidden; position: relative; margin: 4px;'></div>");
                                        var ActuButton = $("<div style='float: left; '><img style='position: relative; ' src='../../Scripts/jqxGrid/jqwidgets/styles/images/icon-edit.png'/></div>");
                                        var DeletButton = $("<div style='float: left; '><img style='position: relative; ' src='../../Scripts/jqxGrid/jqwidgets/styles/images/icon-delete.png'/></div>");

                                        container.append(ActuButton);
                                        container.append(DeletButton);
                                        statusbar.append(container);
                                        ActuButton.jqxButton({ template: "link", width: 40, height: 25 });
                                        DeletButton.jqxButton({ template: "link", width: 40, height: 25 });

                                        ActuButton.click(function (event) {
                                            $("#BActuComp").click();
                                        });
                                        DeletButton.click(function (event) {
                                            $("#BtDeletComp").click();
                                        });

                                    }

                                    

                                },
                                columns: [
                                    { text: 'No.', datafield: 'iId', width: 50 },
                                    { text: 'Nombre de empresa', datafield: 'sNombreEmpresa', width: 140 },
                                    { text: 'PPyPA', datafield: 'iPremioPyA', width: 60 },
                                    { text: 'Puesto', datafield: 'sPuesto', width: 150 },
                                    { text: 'Nombre de renglón', datafield: 'sNombreRenglon', width: 100 },
                                    { text: 'iImporte', datafield: 'iImporte', width: 100 },
                                    { text: 'Descripción', datafield: 'sDescripcion', whidt: 100 },
                                ]
                            });
                    }
                } else {
                    alert('No hay datos')
                }
               
                
            }
        });

    };
    FTbCompensacion();
         
    // visualiza el bloque de insert
    FBlockInsert = () => {
        $("#DropRenglon").attr('readonly', false).trigger('chosen:updated');
        $("#TxtImporte").attr('readonly', false).trigger('chosen:updated'); 
        document.getElementById('content-blockInsert').classList.remove("d-none");
        FListadoEmpresa();
        btnActu.style.visibility = 'hidden';
        btnRegistrar.style.visibility = 'visible';
    };

    FListadoEmpresa = () => {
        $("#DropEmpresa").empty();
        $('#DropEmpresa').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisEmpresas",
            type: "POST",
            data: JSON.stringify(),
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("DropEmpresa").innerHTML += `<option value='${data[i].iIdEmpresa}'>${data[i].sNombreEmpresa}</option>`;

                }
            }
        });
    };

    $('#DropEmpresa').change(function () {
        FListPuesto();
        FLisRenglon();
    });

    FListPuesto = () => {
        const dataSend = { iIdEmpresa: DropEmpresa.value };
        $("#DropPuesto").empty();
        $('#DropPuesto').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisPuestosEmpresa",
            type: "POST",
            data: dataSend,
            success: (data) => {
                console.log('entra');
                for (i = 0; i < data.length; i++) {
                    document.getElementById("DropPuesto").innerHTML += `<option value='${data[i].iIdPuesto}'>${data[i].sNombrePuesto}</option>`;
                }
            },        
        });

    };

    FListPuestoActu = (Idempresa) => {
        console.log('activa');
        const dataSend = { iIdEmpresa: Idempresa };
        $("#DropPuesto").empty();
        $('#DropPuesto').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisPuestosEmpresa",
            type: "POST",
            data: dataSend,
            success: (data) => {
                console.log('entra');
                for (i = 0; i < data.length; i++) {
                    document.getElementById("DropPuesto").innerHTML += `<option value='${data[i].iIdPuesto}'>${data[i].sNombrePuesto}</option>`;
                }
            },
        });

    };

    FLisRenglon = () => {

        const dataSend = { IdEmpresa: DropEmpresa.value, iElemntoNOm: 0 };
        $("#DropRenglon").empty();
        $('#DropRenglon').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisRenglon",
            type: "POST",
            data: dataSend,
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("DropRenglon").innerHTML += `<option value='${data[i].iIdRenglon}'>${data[i].sNombreRenglon}</option>`;
                }
            },
            error: function (jqXHR, exception) {
                fcaptureaerrorsajax(jqXHR, exception);
            }
        });

    };
    FLisRenglonActu = (Idempresa) => {

        const dataSend = { IdEmpresa: Idempresa, iElemntoNOm: 0 };
        $("#DropRenglon").empty();
        $('#DropRenglon').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisRenglon",
            type: "POST",
            data: dataSend,
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("DropRenglon").innerHTML += `<option value='${data[i].iIdRenglon}'>${data[i].sNombreRenglon}</option>`;
                }
            },
            error: function (jqXHR, exception) {
                fcaptureaerrorsajax(jqXHR, exception);
            }
        });

    };

    btnNuevaComp.addEventListener('click', FBlockInsert);

    // inserta Compensancion 
    FNewCompensacion = () => {

        var dataSendComp = { iIdempresa: DropEmpresa.value, iPyA: 0, iIdpuesto: DropPuesto.value, iIdRenglon: DropRenglon.value, iImporte: TxtImporte.value, sDescripcion: TxtDesp.value };

        if (valorCheckpya.checked == true) {
            dataSendComp = { iIdempresa: DropEmpresa.value, iPyA: 1, iIdpuesto: DropPuesto.value, iIdRenglon: 0, iImporte: 0, sDescripcion: TxtDesp.value };
        }
        if (valorCheckpya.checked == false) {

            dataSendComp = { iIdempresa: DropEmpresa.value, iPyA: 0, iIdpuesto: DropPuesto.value, iIdRenglon: DropRenglon.value, iImporte: TxtImporte.value, sDescripcion: TxtDesp.value };
        }
    
        $.ajax({
            url: "../Nomina/NewCompFija",
            type: "POST",
            data: dataSendComp,
            success: (data) => {
                if (data[0].sMensaje == "success") {
                    if (data[0].iId < 1) {
                        fshowtypealert('Compensación fija!', 'Guardada', 'success');
                        FLimpCamp();
                        FTbCompensacion();
                    }
                    if (data[0].iId > 0) {
                        fshowtypealert('La compensación ya existe!', 'se encuentra en el renglo No.' + data[0].iId , 'warning');
                    }
                }
                else {
                    fshowtypealert('Error', 'Contacte a sistemas', 'error');
                }
            },
        });

    };

    btnRegistrar.addEventListener('click', FNewCompensacion);

    /// Premio y asistencia 

    FClicpya = () => {
        if (valorCheckpya.checked == true) {
            $("#DropRenglon").attr('readonly', true).trigger('chosen:updated'); 
            $("#TxtImporte").attr('readonly', true).trigger('chosen:updated'); 
            DropRenglon.value = 0;
            TxtImporte.value = "";
        }
        if (valorCheckpya.checked == false) {
            $("#DropRenglon").attr('readonly', false).trigger('chosen:updated'); 
            $("#TxtImporte").attr('readonly', false).trigger('chosen:updated'); 
        }

    }   

    ChekPYA.addEventListener('click',FClicpya)


    // limpia campos de pantalla compensacion

    FLimpCamp = () => {
        DropEmpresa.value = 0;
        DropPuesto.value = 0;
        DropRenglon.value = 0;
        ChekPYA.checked = false;
        TxtImporte.value = "";
        TxtDesp.value = "";
        document.getElementById('content-blockInsert').classList.add("d-none");
    };

    //cancela operacion de insertar

    btnCancelar.addEventListener('click', FLimpCamp);


    //Funcion Carga datos
    FCargadatos = () => {
       // actualiza empresa 
        for (var i = 0; i < DropEmpresa.length; i++) {
            if (DropEmpresa.options[i].text == IdEmpresaTb) {
                // seleccionamos el valor que coincide
                DropEmpresa.selectedIndex = i;
            }

        };
        if (iPPyPAtb == 1) {
            ChekPYA.checked = true;
            $("#DropRenglon").attr('readonly', true).trigger('chosen:updated');
            $("#TxtImporte").attr('readonly', true).trigger('chosen:updated'); 
        }
        if (ChekPYA.checked == 0) {
            ChekPYA.checked = false;
            $("#DropRenglon").attr('readonly', false).trigger('chosen:updated');
            $("#TxtImporte").attr('readonly', false).trigger('chosen:updated'); 
        }
        for (var i = 0; i < DropPuesto.length; i++) {
            
            if (DropPuesto.options[i].text == Puestotb) {
                // seleccionamos el valor que coincide
                DropPuesto.selectedIndex = i;
            }

        };
        for (var i = 0; i < DropRenglon.length; i++) {

            if (DropRenglon.options[i].text == RenglonTB) {
                // seleccionamos el valor que coincide
                DropRenglon.selectedIndex = i;
            }

        };
        TxtImporte.value = ImporteTb;
        TxtDesp.value = DescripcioTb;
        btnActu.style.visibility = 'visible';
        btnRegistrar.style.visibility = 'hidden';

    };

    $("#TBCompensacion").on('rowselect', function (event) {
        var args = event.args;
        var index = args.index;
        var row = args.row;

        idTb = row.iId;
        IdEmpresaTb = row.sNombreEmpresa;
        iPPyPAtb = row.iPremioPyA;
        Puestotb = row.sPuesto;
        RenglonTB = row.sNombreRenglon;
        ImporteTb = row.iImporte;
        DescripcioTb = row.sDescripcion;

        console.log(idTb + IdEmpresaTb + iPPyPAtb + Puestotb + RenglonTB + ImporteTb + DescripcioTb);
        FListadoEmpresa();

        separador = " ",
        limite = 2,
        arreglosubcadena = IdEmpresaTb.split(separador, limite);
        FListPuestoActu(arreglosubcadena[0]);
        FLisRenglonActu(arreglosubcadena[0]);
        FLimpCamp();
        clickEdit = 0;
        clickDele = 0;
    });


    // Actualiza los datos 
    FActualiza = () => {

        var dataSendComp = { iIDComp: idTb, iIdempresa: DropEmpresa.value, iPyA: 0, iIdpuesto: DropPuesto.value, iIdRenglon: DropRenglon.value, iImporte: TxtImporte.value, sDescripcion: TxtDesp.value, iCancel: 0  };

        if (valorCheckpya.checked == true) {
            dataSendComp = { iIDComp: idTb, iIdempresa: DropEmpresa.value, iPyA: 1, iIdpuesto: DropPuesto.value, iIdRenglon: 0, iImporte: 0, sDescripcion: TxtDesp.value, iCancel: 0  };
        }
        if (valorCheckpya.checked == false) {

            dataSendComp = { iIDComp: idTb, iIdempresa: DropEmpresa.value, iPyA: 0, iIdpuesto: DropPuesto.value, iIdRenglon: DropRenglon.value, iImporte: TxtImporte.value, sDescripcion: TxtDesp.value, iCancel: 0 };
        }

        $.ajax({
            url: "../Nomina/UpdateCompFija",
            type: "POST",
            data: dataSendComp,
            success: (data) => {
                if (data.sMensaje == "success") {
                    fshowtypealert('Compensación fija!', 'Actualizada', 'success');
                    FLimpCamp();
                    FTbCompensacion();
                }
                else {
                    fshowtypealert('Error', 'Contacte a sistemas', 'error');
                }
            },
        });

    };
    btnActu.addEventListener('click', FActualiza);

    /// cancela Compnesacion
    FCancela = () => {

        dataSendComp = { iIDComp: idTb, iIdempresa: 0, iPyA: 0, iIdpuesto: 0, iIdRenglon: DropRenglon.value, iImporte: 0, sDescripcion: 0, iCancel: 1 };

        $.ajax({
            url: "../Nomina/UpdateCompFija",
            type: "POST",
            data: dataSendComp,
            success: (data) => {
                if (data.sMensaje == "success") {
                    fshowtypealert('Compensación fija!', 'Eliminada', 'success');
                    FLimpCamp();
                    FTbCompensacion();
                }
                else {
                    fshowtypealert('Error', 'Contacte a sistemas', 'error');
                }
            },
        });
    };

    FActualizaCopm = () => {
        FLimpCamp();
        $("#TBCompensacion").click();
        document.getElementById('content-blockInsert').classList.remove("d-none");
        FCargadatos();
        clickEdit = 0;
    };

    BActuComp.addEventListener('click', FActualizaCopm)
    BtDeletComp.addEventListener('click', FCancela)

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
    $("#TxtImporte").keyup(function () {
        this.value = (this.value + '').replace(/[^0-9,.,]/g, '');
    });
    if (btnNuevaComp.value == "True") {
        btnNuevaComp.style.visibility = 'hidden';

    }

}); 
