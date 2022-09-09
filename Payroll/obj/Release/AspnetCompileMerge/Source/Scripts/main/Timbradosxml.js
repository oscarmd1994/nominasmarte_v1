$(function () {

    // Declaracion de variables

    const EmpresaTim = document.getElementById('EmpresaTim');
    const anoxml = document.getElementById('anoxml');
    const TipoPeriodoxml = document.getElementById('TipoPeriodoxml');
    const Periodoxml = document.getElementById('Periodoxml');
    const ReciboTim = document.getElementById('ReciboTim');
    const fileUpload = document.getElementById('fileUpload');
    var Path;
    const versionxml = "12";

    /// carga el nombre del archivo en upfile
    $('.custom-file input').change(function () {

        var filePath = $(this).val();
        var files = [];
        for (var i = 0; i < $(this)[0].files.length; i++) {
            files.push($(this)[0].files[i].name);
        }
        $(this).next('.custom-file-label').html(files.join(', '));

    });

    // declaracion de boton de timbrado
    btnTiembrar = document.getElementById('btnTiembrar');
    $('#btnTiembrar').on('click', function () {
        
        var aniox = anoxml.value;
        var TipPeriodox = TipoPeriodoxml.value;
        var Peridox = Periodoxml.value;
        var versionx = versionxml;

      //  if (aniox.value != "" && TipPeriodox != "" && Peridox != "" && versionx != "" && EmpresaTim.value != 0 && ReciboTim.value != 0) {
        if (aniox.value != "" && TipPeriodox != "" && Peridox != "" && versionx != "") {

            console.log('Entro aa')
            var selectFile = $("#fileUpload")[0].files[0];
            var dataString = new FormData();
            var NomArch = selectFile.name;
            var UbiArch = selectFile.Path;
            separador = ".",
                limite = 2,
                arreglosubcadena = NomArch.split(separador, limite);
            dataString.append("fileUpload", selectFile);
            if (arreglosubcadena[1] == "zip") {
                $.ajax({
                    url: "../Empleados/LoadFile",
                    type: "POST",
                    timeout: 50000,
                    data: dataString,
                    contentType: false,
                    processData: false,
                    async: false,
                    beforeSend: function (data) {
                        $('#jqxLoader2').jqxLoader('open');
                    },
                    success: function (data) {
                        if (typeof data.Value != "undefined") {
                            console.log('Entro abc');
                            const dataSend = { Anio: aniox, TipoPeriodo: TipPeriodox, Perido: Peridox, Version: versionx, NomArchivo: NomArch/*, IdEmpresa: EmpresaTim.value, Recibo: ReciboTim.value*/ };
                            console.log(dataSend);
                            $.ajax({
                                url: "../Empleados/TimbXML",
                                type: "POST",
                                data: dataSend,
                                success: (data) => {
                                    $('#jqxLoader2').jqxLoader('close');
                                    if (data[0].sMensaje != null) {
                                        $('#jqxLoader2').jqxLoader('close');
                                        var url = '\\Archivos\\Pdfzio.zip';
                                        if (data[0].iNoEjecutados == 1) {
                                            fshowtypealert(data[0].iNoEjecutados + ' PDF generado', data.Message, 'success');
                                            window.open(url);
                                        }

                                        if (data[0].iNoEjecutados > 1) {
                                            $('#jqxLoader2').jqxLoader('close');

                                            if (data[0].archXmlErr != null) {
                                                fshowtypealert(data[0].iNoEjecutados + ' PDF generados, el/los siguiente(s) PDF con No. de Nomina no fueron generados por falta de No cuenta:            ' + data[0].archXmlErr, data.Message, 'success');
                                            }

                                            else {
                                                fshowtypealert(data[0].iNoEjecutados + ' PDF generado', data.Message, 'success');

                                            }

                                            window.open(url);
                                        }

                                        if (data[0].iNoEjecutados == 0) {
                                            if (data[0].archXmlErr != null) {
                                                fshowtypealert(data[0].iNoEjecutados + ' PDF generados, el/los siguiente(s) PDF con No. de Nomina no fueron generados por falta de No cuenta:            ' + data[0].archXmlErr, data.Message, 'success');
                                            }

                                            else {
                                                fshowtypealert(data[0].iNoEjecutados + ' PDF generado', data.Message, 'success');

                                            }

                                        }

                                    }


                                }
                            });

                        }
                        else {
                            fshowtypealert('Timbrado XML', "Error no identificado en la carga del archivo Winzip", 'warning');
                        }

                    },
                    Error: function (data) {
                        fshowtypealert('Timbrado XML', "Archivo dañando", 'warning');
                        $('#jqxLoader2').jqxLoader('close');
                    }
                });
            }
            if (arreglosubcadena[1] != "zip") {

                console.log('Favor de subir archivos ZIP');
                fshowtypealert('Timbrado XML', 'El archivo que seleccionó no es un winzip favor de seleccionar uno ', 'warning');
            }

        }

        else {
            console.log('error');
            fshowtypealert('Timbrado XML', ' El año,  Tipo de periodo y periodo son campos obligatorio', 'warning');
        }

    });

    /// Validaciones
    $("#anoxml").keyup(function () {
        this.value = (this.value + '').replace(/[^0-9]/g, '');
    });
    $("#TipoPeriodoxml").keyup(function () {
        this.value = (this.value + '').replace(/[^0-9]/g, '');
    });
    $("#Periodoxml").keyup(function () {
        this.value = (this.value + '').replace(/[^0-9]/g, '');
    });


    /// Listado de empresa 


    FListadoEmpresa = () => {
        $("#EmpresaTim").empty();
        $('#EmpresaTim').append('<option value="0" selected="selected">Selecciona</option>');
        $.ajax({
            url: "../Nomina/LisEmpresas",
            type: "POST",
            data: JSON.stringify(),
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                for (i = 0; i < data.length; i++) {
                    document.getElementById("EmpresaTim").innerHTML += `<option value='${data[i].iIdEmpresa}'>${data[i].sNombreEmpresa}</option>`;
         
                }
            }
        });

    };
    FListadoEmpresa();


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
    $("#jqxLoader2").jqxLoader({ text: "Generando Timbrados en PDF", width: 300, height: 300 });
  

});