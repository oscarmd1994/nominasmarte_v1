$(function () {
    /// Declaracion de variables 

    const TxIdRenglon = document.getElementById('TxIdRenglon');
    const txNomRenglon = document.getElementById('txNomRenglon');
    const DropEmpresa2 = document.getElementById('DropEmpresa2');
    const DroplisSat = document.getElementById('DroplisSat');
    const BtnAgreRe = document.getElementById('BtnAgreRe');
    const ActuRengl = document.getElementById('ActuRenglon');
    const Latit = document.getElementById('Latitu');
    const AgregarRenglo = document.getElementById('AgregarRenglon');
    const BtnActualiza = document.getElementById('BtnActualiza');
    const LaTipoReng = document.getElementById('LaTipoReng');
    const LaEleNom = document.getElementById('LaEleNom');
    const LalisCalculo = document.getElementById('LalisCalculo');

    /// el boton BtnAgreRe.value poton que guarda el perfil de escritura y lectura



    var Cancel = 0;
    var espejo = 0;
    var titu = "Reglon Nuevo";
    $('#Latitu').html(titu);
    // Lis empresa 
    LisEmpresa = () => {
        $.ajax({
            url: "../Nomina/LisEmpresas",
            type: "POST",
            data: JSON.stringify(),
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("DropEmpresa2").innerHTML += `<option value='${data[i].iIdEmpresa}'>${data[i].sNombreEmpresa}</option>`;


                }
            }
        });


    };
    LisEmpresa();

    $('#DropEmpresa2').change(function () {
        FTabCRenglones(DropEmpresa2.value, 0);
    });




    /// Lista SAt 

    LisSat = () => {

        $.ajax({
            url: "../Catalogos/ListSat",
            type: "POST",
            data: JSON.stringify(),
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("DroplisSat").innerHTML += `<option value='${data[i].idSat}'> ${data[i].idSat} ${data[i].sSat}</option>`;
                }
            }
        });
    };
    LisSat();



    Fbotones = () => {

        BtnActualiza.style.visibility = "visible";
        BtnAgreRe.style.visibility = "hidden";
    }
    ActuRenglon.addEventListener('click', Fbotones);


    // Actualiza renglon
    FActuRenglon = () => {

        var op1 = Cancel;
        var op2 = espejo;
        const dataSend = {
            iIdRenglon: TxIdRenglon.value, sNombreRenglon: txNomRenglon.value,
            iIdEmpresa: DropEmpresa2.value,
            iIdSat: DroplisSat.value,
            iEspejo: espejo, PenAlin: 0

        };
        $.ajax({
            url: "../Catalogos/UpdateRenglon",
            type: "POST",
            data: dataSend,
            success: function (data) {
                if (data.sMensaje == "success") {
                    FTabCRenglones(DropEmpresa2.value, 0);
                    fshowtypealert('Renglon Actualizado!', 'Renglón actualizado ', 'success');
                }
                if (data.sMensaje == "error") {

                    fshowtypealert('Error', 'Contacte a sistemas', 'error');
                }
            },
            error: function (jqXHR, exception) {
                fcaptureaerrorsajax(jqXHR, exception);
            }
        });

    };

    BtnActualiza.addEventListener('click', FActuRenglon);


    FTitu = () => {
        BtnActualiza.style.visibility = "hidden";
        BtnAgreRe.style.visibility = "visible";
        titu = 'Nuevo Renglón';
        $('#Latitu').html(titu);
        $("#TxIdRenglon").removeAttr("readonly");
        $("#txNomRenglon").removeAttr("readonly");
        LaTipoReng.style.visibility = 'visible';
        LaEleNom.style.visibility = 'visible';
        LalisCalculo.style.visibility = 'visible';
    }
    AgregarRenglo.addEventListener('click', FTitu);

    /* FUNCION QUE MUESTRA ALERTAS */
    fshowtypealert = (title, text, icon) => {
        Swal.fire({
            title: title, text: text, icon: icon,
            showClass: { popup: 'animated fadeInDown faster' },
            hideClass: { popup: 'animated fadeOutUp faster' },
            confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
        }).then((acepta) => {
            //  Nombrede.value       = '';
            // Descripcionde.value  = '';
            //iAnode.value         = '';
            //cande.value          = '';
            //$("html, body").animate({
            //    scrollTop: $(`#${element.id}`).offset().top - 50
            //}, 1000);
            //if (clear == 1) {
            //    setTimeout(() => {
            //        element.focus();
            //        setTimeout(() => { element.value = ""; }, 300);
            //    }, 1200);
            //} else {
            //    setTimeout(() => {
            //        element.focus();
            //    }, 1200);
            //}
        });
    };


    // validaciones


    //themas 
    $("#dTabCrenglones").jqxDataTable({ theme: 'bootstrap' })
    $("#ChecCancel").jqxCheckBox({ theme: 'bootstrap' })
    $("#ChecEspejo").jqxCheckBox({ theme: 'bootstrap' })

    /// Carga Tb Renglones 
    FTabCRenglones = (idempre, iElement) => {


        const dataSend = { IdEmpresa: idempre, iElemntoNOm: iElement };
        //console.log(dataSend);

        $.ajax({
            url: "../Catalogos/datRenglones",
            type: "POST",
            data: dataSend,
            success: (data) => {
                var source =
                {
                    localdata: data,
                    datatype: "array",
                    datafields:
                        [
                            { name: 'sIdEmpresa', type: 'cadena' },
                            { name: 'sNombreRenglon', type: 'cadena' },
                            { name: 'sIdElementoNomina', type: 'cadena' },
                            { name: 'sEspejo', type: 'cadena' },
                            { name: 'sIdSat', type: 'cadena' }
                        ]
                };

                var dataAdapter = new $.jqx.dataAdapter(source);

                $("#dTabCrenglones").jqxDataTable(
                    {
                        source: dataAdapter,
                        width: 800,
                        pageable: true,
                        altRows: true,
                        filterable: true,
                        pagerButtonsCount: 10,
                        columnsResize: true,
                        filterMode: 'simple',
                        columns: [
                            //{ text: 'Empresa', datafield: 'sIdEmpresa', width: 150 },
                            { text: 'Nombre de renglón', datafield: 'sNombreRenglon', width: 200 },
                            { text: 'Elemento de Nomina', datafield: 'sIdElementoNomina', whidth: 200 },
                            { text: 'Espejo', datafield: 'sEspejo', width: 200 },
                            { text: 'Id Sat', datafield: 'sIdSat', width: 200 }
                        ]
                    });


            }
        });
    };
    FTabCRenglones(0, 0);

    if (BtnAgreRe.value != "True") {
        $("#dTabCrenglones").on('rowDoubleClick', function (event) {
            var args = event.args;
            var index = args.index;
            var row = args.row;

            $("#TxIdRenglon").attr("readonly", "readonly");
            $("#txNomRenglon").attr("readonly", "readonly");
            titu = 'Actualizar Renglon';

            var idEmpresa = row.sIdEmpresa;
            var NombreRenglon = row.sNombreRenglon;
            var NombreSat = row.sIdSat;

            //console.log('Nombre del Sat: ' + NombreSat);
            for (var i = 0; i < DroplisSat.length;) {

                //console.log(DroplisSat.options[i].text);

                if (DroplisSat.options[i].text == NombreSat) {
                    // seleccionamos el valor que coincide
                    DroplisSat.selectedIndex = i;
                }
                i++;
            };

            if (row.sEspejo == 0) {
                $("#ChecEspejo").jqxCheckBox({ checked: false });
            }
            if (row.sEspejo == 1) {
                $("#ChecEspejo").jqxCheckBox({ checked: true });
            }



            separador = " ",
                limite = 5,
                arreglosubcadena = NombreRenglon.split(separador, limite);
            TxIdRenglon.value = arreglosubcadena[0];
            if (arreglosubcadena.length > 2) {
                var text = arreglosubcadena[1]
                i = 2
                for (i; i < arreglosubcadena.length; i++) {
                    text = text + " " + arreglosubcadena[i];
                }
                //console.log(text);
                txNomRenglon.value = text;
            }
            else {
                txNomRenglon.value = arreglosubcadena[1];
            }


            $('#Latitu').html(titu);
            $("#ActuRenglon").click();




        });


    }

    
    $("#ChecCancel").jqxCheckBox({ width: 120, height: 25 });
    $("#ChecCancel").bind('change', function (event) {
        var checked = event.args.checked;
        if (checked == true) {
            Cancel = 1;
        }
        if (checked == false) {
            Cancel = 0;
        }


    });
    $("#ChecEspejo").jqxCheckBox({ width: 120, height: 25 });
    $("#ChecEspejo").bind('change', function (event) {
        var checked = event.args.checked;
        if (checked == true) {
            espejo = 1;
        }
        if (checked == false) {
            espejo = 0;
        }


    });
    $("#orderID").jqxInput({ disabled: true, width: 150, height: 30 });
    $("#save").jqxButton({ height: 30, width: 80 });
    $("#cancel").jqxButton({ height: 30, width: 80 });
    $("#cancel").mousedown(function () {
        // close jqxWindow.
        $("#dialog").jqxWindow('close');
    });
    $("#save").mousedown(function () {
        // close jqxWindow.
        $("#dialog").jqxWindow('close');
        // update edited row.
        var editRow = parseInt($("#dialog").attr('data-row'));
        var rowData = {
            OrderID: $("#orderID").val()
        };
        $("#dTabCrenglones").jqxDataTable('updateRow', editRow, rowData);

    });
    $("#dialog").on('close', function () {
        // enable jqxDataTable.

        $("#dTabCrenglones").jqxDataTable({ disabled: false });

    });
    $("#dialog").jqxWindow({
        theme: 'darkblue',
        resizable: false,

        position: { left: $("#dTabCrenglones").offset().left + 75, top: $("#dTabCrenglones").offset().top + -805 },

        width: 270, height: 230,

        autoOpen: false

    });
    $("#dialog").css('visibility', 'visible');
    $("#button1").jqxButton({ width: 120, imgPosition: "left", textPosition: "left", imgSrc: "../../images/facebook.png", textImageRelation: "imageBeforeText" });

    /* FUNCION QUE MUESTRA ALERTAS */
    fshowtypealert = (title, text, icon) => {
        Swal.fire({
            title: title, text: text, icon: icon,
            showClass: { popup: 'animated fadeInDown faster' },
            hideClass: { popup: 'animated fadeOutUp faster' },
            confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
        }).then((acepta) => {
            //  Nombrede.value       = '';
            // Descripcionde.value  = '';
            //iAnode.value         = '';
            //cande.value          = '';
            //$("html, body").animate({
            //    scrollTop: $(`#${element.id}`).offset().top - 50
            //}, 1000);
            //if (clear == 1) {
            //    setTimeout(() => {
            //        element.focus();
            //        setTimeout(() => { element.value = ""; }, 300);
            //    }, 1200);
            //} else {
            //    setTimeout(() => {
            //        element.focus();
            //    }, 1200);
            //}
        });
    };


});