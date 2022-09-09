﻿$(function () {
    $("#modalLiveSearchEmpleado").modal("show");
    var dateFormat = "mm/dd/yy",
        from = $("#from")
            .datepicker({
                defaultDate: "+1w",
                minDate: +7,
                maxDate: +90,
                changeMonth: true,
                numberOfMonths: 1,
                beforeShowDay: $.datepicker.noWeekends,
                changeYear: true
            })
            .on("change", function () {
                $("#to").removeAttr("disabled");
                if (!$("#to").val() == "") {
                    calcularDias()
                }
                to.datepicker("option", "minDate", getDate(this));
            }),
        to = $("#to").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            numberOfMonths: 1,
            beforeShowDay: $.datepicker.noWeekends,
            changeYear: true
        })
            .on("change", function () {
                from.datepicker("option", "maxDate", getDate(this));
                $("#btnCalcVacaciones").removeAttr("disabled");
                calcularDias();
            });
    getDate = (element) => {
        var date;
        try {
            date = $.datepicker.parseDate(dateFormat, element.value);
        } catch (error) {
            date = null;
        }
        return date;
    }
});

$(function () {
    $("#vacacionesCollapse").collapse("show");
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
                    $("#resultSearchEmpleados").empty();
                    if (data[0]["iFlag"] == 0) {
                        for (var i = 0; i < data.length; i++) {
                            $("#resultSearchEmpleados").append("<div class='list-group-item list-group-item-action btnListEmpleados font-labels  font-weight-bold' onclick='MostrarDatosEmpleado(" + data[i]["IdEmpleado"] + ")' > <i class='far fa-user-circle text-primary'></i> " + data[i]["Nombre_Empleado"] + " " + data[i]["Apellido_Paterno_Empleado"] + ' ' + data[i]["Apellido_Materno_Empleado"] + "   -   <small class='text-muted'><i class='fas fa-briefcase text-warning'></i> " + data[i]["DescripcionDepartamento"] + "</small> - <small class='text-muted'>" + data[i]["DescripcionPuesto"] + "</small></div>");
                        }
                    }
                    else {
                        $("#resultSearchEmpleados").append("<button type='button' class='list-group-item list-group-item-action btnListEmpleados font-labels'  >" + data[0]["Nombre_Empleado"] + "<br><small class='text-muted'>" + data[0]["DescripcionPuesto"] + "</small> </button>");
                    }
                }
            });
        } else {
            $("#resultSearchEmpleados").empty();
        }
    });
    $('#example1 tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });
    $('#example2 tbody').on('click', 'tr', function () {
        $(this).toggleClass('selected');
    });
    //Evento click para guardar periodo de vacaciones
    $("#btnCalcVacaciones").on("click", function (evt) {
        evt.preventDefault();
        var fechai = document.getElementById("from");
        var fechat = document.getElementById("to");
        $("#diasV").removeAttr("disabled");
        var dias = document.getElementById("diasV");
        var ln_id = document.getElementById("btn_id_periodo_ln");
        $("#diasV").disabled = true;
        $.ajax({
            url: "../Incidencias/SavePeriodosVac",
            type: "POST",
            cache: false,
            data: JSON.stringify({
                PerVacLn_id: ln_id.value,
                FechaInicio: fechai.value,
                FechaFin: fechat.value,
                Dias: dias.value
            }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                if (data[0] == "1") {
                    document.getElementById("frmAddPeriodo").reset();
                    $("#to").addClass("disabled");
                    $("#from").datepicker({
                        minDate: -30
                    });
                    $("#diasV").addClass("disabled");
                    $("#btnCalcVacaciones").addClass("disabled");
                    CargarTabPeriodos(ln_id.value);
                    Swal.fire({
                        title: "Correcto!",
                        text: data[1],
                        icon: "success",
                        timer: 1500
                    });
                } else {
                    Swal.fire({
                        title: "Aviso!",
                        text: data[1],
                        icon: "warning",
                        cancelButton: true
                    });
                    $("#from").focus();
                }
            }
        });
    });
    $(".btn-change-status").on("click", function () {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.value) {
                $.ajax({
                    url: "../Incidencias/setDisfrutadas",
                    type: "POST",
                    cache: false,
                    data: datos,
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: (data) => {

                    }
                });
                Swal.fire(
                    'Deleted!',
                    'Your file has been deleted.',
                    'success'
                );
            }
        });
    });
    var tab2, tab1;
    MostrarDatosEmpleado = (idE) => {
        var txtIdEmpleado = { "IdEmpleado": idE };
        $.ajax({
            url: "../Empleados/SearchEmpleado",
            type: "POST",
            data: JSON.stringify(txtIdEmpleado),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                console.log(data);
                $("#resultSearchEmpleados").empty();
                for (var i = 0; i < data.length; i++) {
                    $("#nomEmp").empty(); $("#nomEmp").append(data[i]["Nombre_Empleado"] + " " + data[i]["Apellido_Paterno_Empleado"] + " " + data[i]["Apellido_Materno_Empleado"]);
                    $("#puesto_emp").empty(); $("#puesto_emp").append(data[i]["DescripcionPuesto"]);
                    $("#area_emp").empty(); $("#area_emp").append(data[i]["DescripcionDepartamento"]);
                    $("#tipo_emp").empty();
                    $("#nomina_id").empty(); $("#nomina_id").html(idE);
                    if (parseInt(data[i]["TipoEmpleado"]) < 164) {
                        $("#tipo_emp").append("<span class='badge badge-pill badge-success'>" + data[i]["DescTipoEmpleado"] + "</span>");
                    } else if (parseInt(data[i]["TipoEmpleado"]) > 163) {
                        $("#tipo_emp").append("<span class='badge badge-pill badge-danger'>" + data[i]["DescTipoEmpleado"] + "</span>");
                    }
                    //$("#antig_emp").empty(); $("#antig_emp").append(data[i]["FechaIngreso"].substring(0, 10));
                    $("#vacacionesCollapse").collapse("show");
                }
                document.getElementById("tab1").innerHTML = "<tr><td colspan='4'><div class='d-flex justify-content-center'><div class='spinner-border' role='status'><span class='sr-only'>Loading...</span></div ></div></td></tr>";
                CargarTabResumen();
                document.getElementById("inputSearchEmpleados").value = '';
                ftoggle();
            }
        });
    }
    ftoggle = () => {
        $("#modalLiveSearchEmpleado").modal("toggle");
    }
    CargarTabResumen = () => {
        $.ajax({
            url: "../Incidencias/LoadPeriodosVac2",
            type: "POST",
            data: JSON.stringify(),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                var id = data[0]["Id_Per_Vac_Ln"];
                document.getElementById("tab1").innerHTML = "";
                for (var i = 0; i < data.length; i++) {
                    document.getElementById("tab1").innerHTML += "<tr><td>" + data[i]["Anio"] + " <input type='hidden' id='btn_id_periodo_ln' value=' " + data[i]["Id_Per_Vac_Ln"] + " '> </td><td>" + data[i]["DiasPrima"] + "</td><td>" + data[i]["DiasDisfrutados"] + "</td><td>" + data[i]["DiasRestantes"] + "</td></tr>";
                }
                CargarTabPeriodos(id);
            }
        });
    }
    CargarTabPeriodos = (id) => {
        $.ajax({
            url: "../Incidencias/LoadPeriodosDist",
            type: "POST",
            data: JSON.stringify({ PerVacLn_id: id }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                document.getElementById("tabBody").innerHTML = "";
                for (var i = 0; i < data.length; i++) {
                    var aprobado = "";
                    var agendado = "";
                    if (data[i]["Aprobado"] == "True") {
                        aprobado = "<div class='fas fa-check'></div>";
                    } else if (data[i]["Aprobado"] == "False") {
                        aprobado = "";
                    }
                    if (data[i]["Agendadas"] == "True") {
                        agendado = "<div class='fas fa-check'></div>";
                    } else if (data[i]["Agendadas"] == "False") {
                        agendado = "";
                    }
                    document.getElementById("tabBody").innerHTML += "<tr>" +
                        "<td>" + data[i]["Dias"] + " <input type = 'hidden' class='btn_IdPer_vac_Dist' value = ' " + data[i]["IdPer_vac_Dist"] + " ' > </td>" +
                        " <td>" + data[i]["Fecha_Inicio"] + "</td>" +
                        " <td>" + data[i]["Fecha_Fin"] + "</td>" +
                        " <td>" + agendado + "</td>" +
                        " <td>" + aprobado + "</td>" +//"<td><div class='fas fa-exchange-alt btn btn-success btn-sm btn-change-status' onclick='setDisfrutadas(" + data[i]["IdPer_vac_Dist"] + ")'></div></td>" +
                        "</tr > ";
                }
            }
        });
    }
    calcularDias = () => {
        var fechaInicial = document.getElementById("from").value;
        var fechaFinal = document.getElementById("to").value;
        inicial = fechaInicial.split("/");
        final = fechaFinal.split("/");
        // obtenemos las fechas en milisegundos
        var inicio = new Date(inicial[2], (inicial[0] - 1), inicial[1]);
        var fin = new Date(final[2], (final[0] - 1), final[1]);
        var timeDiff = Math.abs(fin.getTime() - inicio.getTime());
        var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24)); //Días entre las dos fechas
        var cuentaFinde = 0; //Número de Sábados y Domingos
        var array = new Array(diffDays);
        for (var i = 0; i < diffDays + 1; i++) {
            //0 => Domingo - 6 => Sábado
            if (inicio.getDay() == 0 || inicio.getDay() == 6) {
                cuentaFinde++;
            }
            inicio.setDate(inicio.getDate() + 1);
        }
        var txtdias = document.getElementById("diasV");
        txtdias.value = (diffDays + 1) - cuentaFinde;
    }
    setDisfrutadas = (IdPer_vac_Dist) => {
        var ln_id = document.getElementById("btn_id_periodo_ln");
        Swal.fire({
            title: '',
            text: "Deseas cambiar el periodo a disfrutado",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            cancelButtonColor: 'Cancelar',
            confirmButtonText: 'Aceptar'
        }).then((result) => {
            if (result.value) {
                $.ajax({
                    url: "../Incidencias/setDisfrutadas",
                    type: "POST",
                    cache: false,
                    data: JSON.stringify({ IdPer_vac_Dist: IdPer_vac_Dist }),
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: (data) => {
                        $.ajax({
                            url: "../Incidencias/LoadPeriodosDist",
                            type: "POST",
                            cache: false,
                            data: JSON.stringify({ PerVacLn_id: ln_id.value }),
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            success: (res) => {
                                document.getElementById("tabBody").innerHTML = "";
                                for (var i = 0; i < res.length; i++) {
                                    if (res[i]["Agendadas"] == "True") {
                                        document.getElementById("tabBody").innerHTML += "<tr>" +
                                            "<td>" + res[i]["Dias"] + " <input type = 'hidden' class='btn_IdPer_vac_Dist' value = ' " + res[i]["IdPer_vac_Dist"] + " ' > </td>" +
                                            " <td>" + res[i]["Fecha_Inicio"] + "</td>" +
                                            " <td>" + res[i]["Fecha_Fin"] + "</td>" +
                                            " <td><div class='fas fa-check'></div></td>" +
                                            " <td></td>" +//"<td><div class='fas fa-exchange-alt btn btn-success btn-sm btn-change-status' onclick='setDisfrutadas(" + res[i]["IdPer_vac_Dist"] + ")'></div></td>" +
                                            "</tr > ";
                                    } else {
                                        document.getElementById("tabBody").innerHTML += "<tr>" +
                                            "<td>" + res[i]["Dias"] + " <input type='hidden' class='btn_IdPer_vac_Dist' value=' " + res[i]["IdPer_vac_Dist"] + " '> </td>" +
                                            "<td>" + res[i]["Fecha_Inicio"] + "</td>" +
                                            "<td>" + res[i]["Fecha_Fin"] + "</td>" +
                                            "<td></td>" +
                                            "<td><div class='fas fa-check'></div></td>" +
                                            "</tr>";
                                    }
                                }
                                Swal.fire(
                                    'Hecho!',
                                    'El periodo fue puesto como disfrutado',
                                    'success'
                                );
                            }
                        });
                    }
                });
            }
        });
    }
});