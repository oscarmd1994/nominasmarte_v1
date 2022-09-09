﻿$(document).ready(function () {
    var tabCargaMasiva;
    $("#tabCargasMasivas").fadeOut();
    beforeValidarFile = () => {
        $("#btnCargaMasiva").html("<span class='spinner-grow spinner-grow-sm' role='status' aria-hidden='true'></span> Cargando...");
        document.getElementById("btnCargaMasiva").disabled = true;
        setTimeout(function () {
            validateUploadFile();
        }, 500);
    }
    validateUploadFile = () => {
        var selectedFile = $("#file-toup").prop("files")[0];
        var selectedF = ($("#file-toup"))[0].files[0];
        var fileType = document.getElementById("file-type").value;
        if (!selectedF) {
            Swal.fire({
                icon: 'warning',
                title: 'Aviso!',
                text: 'Aun no selecciona un archivo'
            });
            $("#btnCargaMasiva").html("<i class='fas fa-check-circle mr-2'></i> Cargar archivo");
            document.getElementById("btnCargaMasiva").disabled = false;
        } else {
            Swal.fire({
                title: 'Mensaje de confirmación',
                html: "<h5>Se cargará Layout de <strong class='text-danger h4 text-uppercase'>" + fileType + "</strong>, <br/>¿Es correcto?</h5>",
                icon: 'warning',
                showCancelButton: true,
                CancelButtonText: 'Cancelar',
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#ff3f3f',//'#d33',
                confirmButtonText: 'Confirmar'
            }).then((result) => {
                if (result.value) {
                    var datos = new FormData();
                    datos.append("fileUpload", selectedF);
                    datos.append("fileType", fileType);
                    $.ajax({
                        url: "../Incidencias/LoadFile",
                        type: "POST",
                        data: datos,
                        processData: false,
                        contentType: false,
                        async: false,
                        success: function (data) {
                            if (data[0] == "0") {
                                document.getElementById("btnCargaMasiva").disabled = false;
                                $("#btnCargaMasiva").html("<i class='fas fa-check-circle mr-2'></i> Cargar archivo");
                                var btn = "<div class='col-md-12 d-flex justify-content-center'><a class=' btn btn-success btn-sm btn-icon-split' href='" + data[1] + "' download><span class='icon'> <i class='fas fa-download text-white'></i> </span><span class='text'> Descargar archivo log .txt </span></a></div>";
                                var txt = "<div class='alert alert-warning col-md-12 my-3' role='alert' id='alert-validation'>" +
                                    "<button type='button' class='close' data-dismiss='alert' aria-label='Close'> <span aria-hidden='true'>&times;</span></button>" +
                                    "<strong> Atención </strong> Hubo errores en el archivo Layout de carga.\n Descargue en archivo log con los errores " + btn + "</div >";
                                $("#collapse-validation-cm").html(txt);
                                $("#collapse-validation-cm").collapse("show");
                                $("a.btn-success").focus();
                                $("#file-toup").val('');
                            }
                            if (data[0] == "1") {
                                document.getElementById("btnCargaMasiva").disabled = false;
                                $("#btnCargaMasiva").html("<i class='fas fa-check-circle mr-2'></i> Cargar archivo");
                                var txt = "<div class='alert alert-success col-md-12 my-3' role='alert' id='alert-validation'>" +
                                    "<button type='button' class='close' data-dismiss='alert' aria-label='Close'> <span aria-hidden='true'>&times;</span></button>" +
                                    "<strong> Listo </strong> " + data[1] + " </div >";
                                $("#collapse-validation-cm").html(txt);
                                $("#collapse-validation-cm").collapse("show");
                                $("#file-toup").val('');
                                $("#tabCargasMasivas").fadeOut();
                                $('.table').DataTable.destroy();
                                loadCargasMasivas();
                            }
                        }
                    });
                }
                else if (result.dismiss) {
                    document.getElementById("btnCargaMasiva").disabled = false;
                    $("#btnCargaMasiva").html("<i class='fas fa-check-circle mr-2'></i> Cargar archivo");
                } else {
                }
            });
        }
    }
    $("#btnDownLoadCM").mouseenter(function () {
        //alert("ENTRA");
        $("#btnDownLoadCM").append("<span> Descargar Layout </span>");
        //document.getElementById("btnDownLoadCM").style.width = 50;
    });
    $("#btnDownLoadCM").mouseleave(function () {
        //alert("SALE");
        $("#btnDownLoadCM").find("span").remove();
        //document.getElementById("btnDownLoadCM").style.width = 30;
    });
    loadCargasMasivas = () => {
        $.ajax({
            url: "../Incidencias/LoadCargasMasivas",
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                var status;
                var tab = "";
                for (var i = 0; i < data.length; i++) {
                    if (data[i][3] == "False") {
                        status = "<span class='center text-success col-md-12'><i class='fas fa-check-circle fa-lg'></i></span>";
                    } else if (data[i][3] == "True") {
                        status = "<span class='center text-danger col-md-12'><i class='fas fa-times-circle fa-lg'></i></span>";
                    }
                    tab += "<tr>" +
                        "<td>" + data[i][0] + "</td>" +
                        "<td>" + data[i][1] + "</td>" +
                        "<td>" + data[i][2] + "</td>" +
                        "<td>" + data[i][4] + "</td>" +
                        "<td class='text-center'>" +
                        "<a class='badge badge-pill badge-danger text-white' title='Cancelar carga' onclick='loadmodalcancelar(\"" + data[i][4] + "\",\"" + data[i][0] + "\",\"" + data[i][1] + "\",\"" + data[i][2] +"\")'><i class='fas fa-minus'></i></a>" +
                        "</td>" +
                        "</tr>";
                }
                document.getElementById("tabCargasMasivasDetalle").innerHTML = tab;
                setTimeout(function () {
                    tabCargaMasiva = $('.table').DataTable({
                        "language": {
                            "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json"
                        },
                        "lengthMenu": [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Todos"]]
                    });
                    $("#tabCargasMasivas").fadeIn();
                }, 2000);
            }
        });
    }
    loadCargasMasivas();
    loadmodalcancelar = (registros, tabla, folio, referencia) => {
        var fecha = referencia.substr(0, 10);
        var hora = referencia.substr(11, 5);
        var usuario = referencia.substr(16);
        $("#cm_tabla").val(tabla);
        $("#cm_fecha").val(fecha);
        $("#cm_hora").val(hora);
        $("#cm_usuario").val(usuario);
        $("#cm_registros").val(registros);
        $("#cm_referencia").val(referencia);
        $("#staticBackdrop").modal("show");
    }
    cancelarCargaMasiva = () => {
        $.ajax({
            url: "../Incidencias/CancelaCargaMasiva",
            type: "POST",
            data: JSON.stringify({
                tabla: $("#cm_tabla").val(),
                referencia: $("#cm_referencia").val(),
            }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                document.getElementById("tabCargasMasivasDetalle").innerHTML = "";
                if (data[0] == '0' ) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Aviso!',
                        text: data[1],
                        timer: 1500
                    });
                } else if (data[0] == '1') {
                    $("#tabCargasMasivas").fadeOut();
                    tabCargaMasiva.destroy();
                    tabCargaMasiva = null;
                    loadCargasMasivas();
                    //setTimeout(function () {
                    //    $("#tabCargasMasivas").fadeIn();
                    //}, 2000);
                    Swal.fire({
                        icon: 'success',
                        title: 'Aviso!',
                        text: data[1],
                        timer: 2500
                    });
                    
                }
            }
        });
    }
}); 