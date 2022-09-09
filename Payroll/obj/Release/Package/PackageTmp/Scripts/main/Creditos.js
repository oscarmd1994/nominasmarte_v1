﻿$(function () {

    $("#btnUpdateCredito").addClass("invisible");
    $("#modalLiveSearchEmpleado").modal("show");
    //Eventos 
    $("#btnSaveCredito").on("click", function () {
        var tdescuento = document.getElementById("inTipoDescuento");
        var descuento = document.getElementById("inDescuento");
        var ncredito = document.getElementById("inNoCredito");
        var fechaa = document.getElementById("inFechaAprovacionCredito");
        var descontar = document.getElementById("inDescontar");
        var fechab = document.getElementById("inFechaBajaCredito");
        var factor = 0;
        if (tdescuento.value == "291" || tdescuento.value == 291) { factor = descuento.value; }
        else { factor = 0 }
        var aseg;
        var form = document.getElementById("frmCreditos");
        

        if ($("#inNoCredito").val().length < 10) {
            $("#inNoCredito").addClass("is-invalid");
            setTimeout(() => {
                $("#inNoCredito").removeClass("is-invalid");
            }, 5000);
        } else if (form.checkValidity() == false) {
            form.classList.add("was-validated");
            setTimeout(() => {
                form.classList.remove("was-validated");
            }, 5000);
        } else {
            if ($("#inAplicaSeguro").is(":checked")) {
                aseg = 1;
            } else {
                aseg = 0;
            }
            $.ajax({
                url: "../Incidencias/SaveCredito",
                data: JSON.stringify({
                    TipoDescuento: tdescuento.value,
                    Descuento: descuento.value,
                    NoCredito: ncredito.value,
                    FechaAprovacion: fechaa.value,
                    Descontar: descontar.value,
                    FechaBaja: fechab.value,
                    FactorDesc: factor
                }),
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: (data) => {
                    if (data[0] == '0') {
                        Swal.fire({
                            icon: 'warning',
                            title: 'Aviso!',
                            text: data[1]
                        });
                    } else if (data[0] == '1') {
                        createTab();
                        Swal.fire({
                            icon: 'success',
                            title: 'Completado!',
                            text: data[1],
                            timer: 1000
                        });
                        form.reset();
                        createTab();
                    }
                }
            });
        }
    });
    //Busqueda de empleado
    $("#inputSearchEmpleados").on("keyup", function () {
        $("#inputSearchEmpleados").empty();
        var txt = $("#inputSearchEmpleados").val();
        if ($("#inputSearchEmpleados").val() != "") {
            var txtSearch = { "txtSearch": txt };
            $.ajax({
                url: "../Empleados/SearchEmpleados",
                type: "POST",
                cache: false,
                data: JSON.stringify(txtSearch),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: (data) => {

                    //$("#resultSearchEmpleados").empty();
                    //if (data[0]["iFlag"] == 0) {
                    //    for (var i = 0; i < data.length; i++) {
                    //        $("#resultSearchEmpleados").append("<div class='list-group-item list-group-item-action btnListEmpleados font-labels  font-weight-bold' onclick='MostrarDatosEmpleado(" + data[i]["IdEmpleado"] + ")'> <i class='far fa-user-circle text-primary'></i> <span class='text-primary'>" + data[i]["Nomina"] + "</span> - " + data[i]["Apellido_Paterno_Empleado"] + ' ' + data[i]["Apellido_Materno_Empleado"] + "  " + data[i]["Nombre_Empleado"] + "  -   <small class='text-muted'><i class='fas fa-briefcase text-warning'></i> " + data[i]["DescripcionDepartamento"] + "</small> - <small class='text-muted'>" + data[i]["DescripcionPuesto"] + "</small></div>");
                    //    }
                    //}
                    //else {
                    //    $("#resultSearchEmpleados").append("<button type='button' class='list-group-item list-group-item-action btnListEmpleados font-labels'  >" + data[0]["Nombre_Empleado"] + "<br><small class='text-muted'>" + data[0]["DescripcionPuesto"] + "</small> </button>");
                    //}
                    $("#resultSearchEmpleados").empty();
                    if (data[0]["iFlag"] == 0) {
                        for (var i = 0; i < data.length; i++) {
                            var badgecolor;
                            var fechabaja;
                            if (data[i]["TipoEmpleado"] >= 164) {
                                badgecolor = "badge-danger";
                                fechabaja = data[i]["FechaBaja"]; //"<span class='text danger'>" + data[i]["FechaBaja"] + "</span>";
                            } else if (data[i]["TipoEmpleado"] < 164) {
                                badgecolor = "badge-success";
                                fechabaja = "";
                            }

                            $("#resultSearchEmpleados").append("" +
                                "<button type='button' class='text-dark h5 font-weight-bold text-left list-group-item list-group-item-action font-labels' onclick='MostrarDatosEmpleado(" + data[i]["IdEmpleado"] + ")'>" +
                                "<i class='fas fa-user-circle text-primary'></i> <span class='text-primary'>" + data[i]["Nomina"] + "</span> - " + data[i]["Apellido_Paterno_Empleado"] + ' ' + data[i]["Apellido_Materno_Empleado"] + " " + data[i]["Nombre_Empleado"] + " <div class='badge " + badgecolor + " badge-pill px-1'>" + data[i]["TipoEmpleado"] + " - " + data[i]["DescTipoEmpleado"] + " - " + fechabaja + "</div>" + " <br> " +
                                "<small class=''><i class='fas fa-building text-success'></i> " + data[i]["Empresa_id"] + " - " + data[i]["Nombre_Empresa"] + "</small><br> " +
                                "<small><i class='fas fa-briefcase text-warning'></i> " + data[i]["DescripcionDepartamento"] + " - " + data[i]["DescripcionPuesto"] + "</small>" +
                                "</button> ");
                        }
                    }
                    else {
                        $("#resultSearchEmpleados").append("<button type='button' class='text-left list-group-item list-group-item-action font-labels'  >" + data[0]["Nombre_Empleado"] + "<br><small class='text-muted'>" + data[0]["DescripcionPuesto"] + "</small> </button>");
                    }
                }
            });
        } else {
            $("#resultSearchEmpleados").empty();
        }
    });
    //VALIDACION DE CAMPOS NUMERICOS
    $('.input-number').on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

    //Funcion de mostrar empleado seleccionado en la busqueda
    MostrarDatosEmpleado = (idE) => {
        var txtIdEmpleado = { "IdEmpleado": idE };
        $.ajax({
            url: "../Empleados/SearchEmpleado",
            type: "POST",
            data: JSON.stringify(txtIdEmpleado),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                var iconb = "";
                var colorb = "";
                if (data[0]["TipoEmpleado"] > 163) {
                    colorb = "badge-danger";
                    iconb = "fa-times-circle";
                } else {
                    colorb = "badge-success";
                    iconb = "fa-check-circle";
                }

                document.getElementById("EmpDes").innerHTML = "<i class='fas fa-hashtag text-primary'></i>&nbsp;&nbsp;" + data[0]["IdEmpleado"] + "&nbsp;&nbsp;<i class='far fa-user-circle text-primary'></i> " + data[0]["Nombre_Empleado"] + " " + data[0]["Apellido_Paterno_Empleado"] + ' ' + data[0]["Apellido_Materno_Empleado"] + " - <small class='text-muted'>" + data[0]["DescripcionPuesto"] + "</small>&nbsp;&nbsp;<div class='badge " + colorb + "'><i class='fas " + iconb + "'></i>&nbsp;" + data[0]["TipoEmpleado"] + "&nbsp;-&nbsp;" + data[0]["DescTipoEmpleado"] + "</div>";
                $("#modalLiveSearchEmpleado").modal("hide");
                createTab();
            }

        });

    }
    //crea la tabla con los creditos que tiene activos el empleado
    createTab = () => {
        $.ajax({
            method: "POST",
            url: "../Incidencias/LoadCreditosEmpleado",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: (data) => {
                document.getElementById("tcbody").innerHTML = "";
                for (var i = 0; i < data.length; i++) {
                    if (data[i]["Cancelado"] == "True") {
                        document.getElementById("tcbody").innerHTML +=
                            "<tr>"
                            + "<td>" + data[i]["NoCredito"] + "</td>"
                            + "<td>" + data[i]["TipoDescuento"] + "</td>"
                            + "<td>" + data[i]["Descuento"] + "</td>"
                            + "<td>" + data[i]["Descontar"] + "</td>"
                            //+ "<td>" + data[i]["FactorDescuento"] + "</td>"
                            + "<td>" + data[i]["FechaBaja"].substr(0, 10) + "</td>"
                            //+ "<td>" + data[i]["Effdt"] + "</td>"
                            + "<td>"
                            + "<a href='#' class='btn badge badge-light btn-priority text-center mx-1' onclick='activarCredito(" + data[i]["IdCredito"] + "," + data[i]["IncidenciaProgramada_id"] + ");' title='Activar'><i class='fas fa-lock text-danger'></i> </a>"
                            + "</td>"
                            + "</tr>";
                    } else {
                        document.getElementById("tcbody").innerHTML +=
                            "<tr>"
                            + "<td>" + data[i]["NoCredito"] + "</td>"
                            + "<td>" + data[i]["TipoDescuento"] + "</td>"
                            + "<td>" + data[i]["Descuento"] + "</td>"
                            + "<td>" + data[i]["Descontar"] + "</td>"
                            //+ "<td>" + data[i]["FactorDescuento"] + "</td>"
                            + "<td>" + data[i]["FechaBaja"].substr(0, 10) + "</td>"
                            //+ "<td>" + data[i]["Effdt"] + "</td>"
                            + "<td>"
                        + "<a href='#' class='btn badge btn-priority badge-light text-center mx-1' title='Desactivar'><i class='fas fa-lock-open text-primary'></i> </a>"
                            //+ "<a href='#' class='btn badge badge-light text-center mx-1' onclick='desactivarCredito(" + data[i]["IdCredito"] + "," + data[i]["IncidenciaProgramada_id"] + ");' title='Desactivar'><i class='fas fa-lock-open text-primary'></i> </a>"
                        + "<a href='#' class='btn badge btn-priority badge-success text-center mx-1' onclick='updateCredito(" + data[i]["IdCredito"] + "," + data[i]["IncidenciaProgramada_id"] + ");' title='Modificar'><i class='fas fa-edit'></i> </a>"
                        + "<a href='#' class='btn badge btn-priority badge btn-priority-danger text-center mx-1' onclick='deleteCredito(" + data[i]["IdCredito"] + ");' title='Desactivar'><i class='fas fa-minus'></i> </a>"
                            + "</td>"
                            + "</tr>";
                    }
                }
                setTimeout(function () {
                    getProfileType();
                }, 1000);
            }
        });
    }
    //carga el tipo de descuentos con los que trabaja el credito
    LoadSelectTipoDescuento = () => {
        $.ajax({
            method: "POST",
            url: "../Incidencias/LoadTipoDescuento",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: (data) => {
                var select = document.getElementById("inTipoDescuento");
                //select.innerHTML = "<option value=''> Selecciona </option>";
                for (var i = 0; i < data.length; i++) {
                    select.innerHTML += "<option value='" + data[i]["Id"] + "'>" + data[i]["Nombre"] + "</option>";
                }
            }
        });
    }
    //carga el tiempo en el que se descontara el credito
    LoadSelectDescontar = () => {
        $.ajax({
            method: "POST",
            url: "../Incidencias/LoadDescontar",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({ catalogoid: 35 }),
            success: (data) => {
                var select = document.getElementById("inDescontar");
                //select.innerHTML = "<option value=''> Selecciona </option>";
                for (var i = 0; i < data.length; i++) {
                    if (i == 0) {
                        select.innerHTML += "<option class='active' value='" + data[i]["iId"] + "'>" + data[i]["sValor"] + "</option>";
                    }
                }
            }
        });
    }

    //borra el credito seleccionado
    deleteCredito = (Credito_id) => {
        Swal.fire({
            title: 'Estas seguro?',
            text: "El crédito sera borrado definitivamente!",
            icon: 'warning',
            showCancelButton: true,
            cancelButtonText: 'Cancelar',
            confirmButtonColor: '#d33',
            cancelButtonColor: '#98959B',
            confirmButtonText: 'Confirmar'
        }).then((result) => {
            if (result.value) {
                $.ajax({
                    method: "POST",
                    url: "../Incidencias/DeleteCredito",
                    data: JSON.stringify({ Credito_id: Credito_id }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: (data) => {
                        if (data[0] == '0') {
                            Swal.fire({
                                icon: 'warning',
                                title: 'Aviso!',
                                text: data[1]
                            });
                        } else if (data[0] == '1') {
                            document.getElementById("tcbody").innerHTML = "";
                            createTab();
                            Swal.fire({
                                icon: 'success',
                                title: 'Completado!',
                                text: data[1],
                                timer: 1000
                            });
                        }
                    }
                });
            }
        });
    }
    //cambia el valor del campo segun el select 
    $("#inTipoDescuento").change(function () {
        var select = document.getElementById("inTipoDescuento");
        switch (select.value) {
            case '289':
                $("#lblInDescuento").html(" Monto ");
                break;
            case '290':
                $("#lblInDescuento").html(" Porcentaje ");
                break;
            case '291':
                $("#lblInDescuento").html(" No. Veces ");
                break;
            case '292':
                $("#lblInDescuento").html(" Factor Descuento ");
                break;
            default:
                $("#lblInDescuento").html(" Monto ");
                break;
        }
    });
    //selecciona el credito y lo dej alisto para modificarlo 
    updateCredito = (Credito_id, IncidenciaProg_id) => {
        $.ajax({
            method: "POST",
            data: JSON.stringify({ Credito_id: Credito_id }),
            url: "../Incidencias/LoadCreditoEmpleado",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: (data) => {
                document.getElementById("inTipoDescuento").value = data[0]["Descontar"];
                document.getElementById("inDescuento").value = data[0]["Descuento"];
                document.getElementById("inNoCredito").value = data[0]["NoCredito"];

                $('#inFechaAprovacionCredito').val(data[0]["FechaAprovacionCredito"].substr(6, 4) + '-' + data[0]["FechaAprovacionCredito"].substr(3, 2) + '-' + data[0]["FechaAprovacionCredito"].substr(0, 2));

                $('#inFechaBajaCredito').val(data[0]["FechaBaja"].substr(6, 4) + '-' + data[0]["FechaBaja"].substr(3, 2) + '-' + data[0]["FechaBaja"].substr(0, 2));

                $("#btnUpdateCredito").removeClass("invisible");
                $("#btnSaveCredito").addClass("invisible"); 
                document.getElementById("inCredito_id").value = Credito_id;
            }
        });
    }

    //reinicia el form de insertar credito
    $("#btnResetFormCredito").click(function () {
        $("#btnUpdateCredito").addClass("invisible");
        $("#btnSaveCredito").removeClass("invisible");
    });

    //actualizar el credito 
    $("#btnUpdateCredito").click(function () {
        var tdescuento = document.getElementById("inTipoDescuento");
        var descuento = document.getElementById("inDescuento");
        var ncredito = document.getElementById("inNoCredito");
        var fechaa = document.getElementById("inFechaAprovacionCredito");
        var descontar = document.getElementById("inDescontar");
        var fechab = document.getElementById("inFechaBajaCredito");
        var credito_id = document.getElementById("inCredito_id");

        var form = document.getElementById("frmCreditos");
        if (form.checkValidity() == false) {
            form.classList.add("was-validated");
            setTimeout(() => {
                form.classList.remove("was-validated");
            }, 5000);
        } else {

            $.ajax({
                url: "../Incidencias/UpdateCredito",
                data: JSON.stringify({
                    Credito_id: credito_id.value,
                    TipoDescuento_id: tdescuento.value,
                    Descontar_id: descontar.value,
                    Descuento: descuento.value,
                    NoCredito: ncredito.value,
                    FechaAprovacion: fechaa.value,
                    FechaBaja: fechab.value
                }),
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: (data) => {
                    if (data[0] == '0') {
                        Swal.fire({
                            icon: 'warning',
                            title: 'Aviso!',
                            text: data[1]
                        });
                    } else if (data[0] == '1') {
                        createTab();
                        Swal.fire({
                            icon: 'success',
                            title: 'Completado!',
                            text: data[1],
                            timer: 1000
                        });
                        $("#btnResetFormCredito").click();
                        createTab();
                    }
                }
            });
        }
    });
});