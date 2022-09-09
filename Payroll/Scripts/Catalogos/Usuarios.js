///////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////
//////////////////////     CATALOGO USUARIOS     //////////////////////
///////////////////////////////////////////////////////////////////////
//                    Oscar Mejia Doroteo
//                    02/12/2021
///////////////////////////////////////////////////////////////////////
var tableUsers = null;
var tableProfiles = null;
//
// Carga tabla de Usuarios disponibles
loadtabusers = () => {
    $.ajax({
        url: "../Catalogos/LoadUsers",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: (data) => {
            var tab = document.getElementById("tabusersbody");
            tab.innerHTML = "";
            for (var i = 0; i < data.length; i++) {
                if (data[i]["Cancelado"] == "True") {
                    text = "<i class='fas fa-times-circle fa-lg text-danger'></i>";
                    btns = "<a class='badge badge-light btn btns border mx-1' onclick='changeuserstatus(0," + data[i]["IdUsuario"] + ")'><i class='fas fa-check-circle fa-lg text-primary'></i> Activar</a>";
                } else {
                    text = "<i class='fas fa-check-circle fa-lg text-primary'></i>";
                    btns = "<a class='badge badge-light btn btns border mx-1' onclick='changeuserstatus(1," + data[i]["IdUsuario"] + ")'><i class='fas fa-times-circle fa-lg text-danger'></i> Desactivar</a>";
                }
                tab.innerHTML += "" +
                    "<tr>" +
                    "<td>" + data[i]["Usuario"] + "</td>" +
                    "<td>" + data[i]["Perfil_id"] + "</td>" +
                    "<td>" + data[i]["Ps"] + "</td>" +
                    "<td class='text-center'>" + text + "</td>" +
                    "<td>" +
                    //"<div class='badge badge-success btn mx-1'><i class='fas fa-edit fa-lg'></i> Editar</div>" +
                    //"<div class='badge badge-info btn mx-1' onclick='mostarmodalnewperfil();'><i class='fas fa-plus fa-lg'></i> Nuevo</div>" +
                    btns +
                    "</td>" +
                    "</tr>";
            }
            tableUsers = $('#tabUsers').DataTable({
                "language": {
                    "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json"
                },
            });
        }
    });
}
//
// Carga tabla de Perfiles de usuarios
loadtabprofiles = () => {

    $.ajax({
        url: "../Catalogos/LoadProfiles",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: (data) => {
            var tab = document.getElementById("tabprofilesbody");
            tab.innerHTML = "";
            var text = "";
            var btns = "";
            for (var i = 0; i < data.length; i++) {
                if (data[i]["Cancelado"] == "True") {
                    //text = "<i class='fas fa-eye-slash fa-lg text-danger'></i>";
                    btns = "<div class='badge badge-light btn btns border mr-1'><i class='fas fa-eye fa-lg text-primary'></i> Activar</div>";
                } else {
                    //text = "<i class='fas fa-eye fa-lg text-primary'></i>";
                    btns = "<div class='badge badge-light btn btns border mr-1'><i class='fas fa-eye-slash fa-lg text-danger'></i> Desactivar</div>";
                }
                tab.innerHTML += "" +
                    "<tr ondblclick='loadtwo(" + data[i]["IdPerfil"] + ");' onclick=''>" +
                    "<td>" + data[i]["IdPerfil"] + "</td>" +
                    "<td>" + data[i]["Perfil"] + "</td>" +
                    "<td>" + data[i]["Fecha_Alta"] + "</td>" +
                    //"<td>" + text + "</td>" +
                    "<td> " + btns + "<div class='badge badge-info btn mr-1'><i class='fas fa-edit fa-lg'></i> Editar</div>" +
                    "<div class='badge badge-dark btn mr-1'><i class='fas fa-stream fa-lg'></i> Menus</div>" +
                    "<div class='badge badge-primary btn mr-1'><i class='fas fa-stream fa-lg'></i> Empresas</div>" +
                    "</td>" +
                    "</tr>";
            }
            tableProfiles = $('#tabProfiles').DataTable({
                "language": {
                    "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json"
                }
            });
        }
    });
}
//
// FUNCION PARA HACER SECUENCIA PARA DAR MOVIEMIENTO A LOS BADGES
$(".btns").hover(function () {
    $('.btns').removeClass('badge-light').addClass('badge-dark');
}, function () {
    $('.btns').removeClass('badge-dark').addClass('badge-light');
});

// FUNCION DE un CLICK EN EL ROW 
$("#collapseProfiles").show(function () {

    $(this).html("<div class='container row col-md-12'>" +
        "<div class='col-sm'>Empresas</div>" +
        "<div class='col-sm'>Empleado</div>" +
        "<div class='col-sm'>Incidencias</div>" +
        "<div class='col-sm'>Nomina</div>" +
        "<div class='col-sm'>Reportes</div>" +
        "<div class='col-sm'>Catalogos</div>" +
        "<div class='col-sm'><label class='col-md-12'>Consulta <button class='btn btn-light btn-sm py-0' data-toggle='collapse' data-target='.collapse-consulta' type='button' onclick='loadcheck(35, \"collapse-cons\");'><i class='fas fa-caret-down fa-lg'></i></button></label>" +
        "<div class='collapse collapse-cons col-md-12'></div>" +
        "</div>" +
        "<div class='col-sm'>Kiosko</div>" +
        "</div>"
    );
});
//
// cambia el status del usuario
//
changeuserstatus = (status, iduser) => {
    Swal.fire({
        title: 'Aviso!',
        text: "Se cambiará el estado del usuario",
        icon: 'info',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Guardar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed == true || result.value == true) {

            $.ajax({
                url: "../Catalogos/ChangeUserStatus",
                type: "POST",
                data: JSON.stringify({ status: status, iduser: iduser }),
                contentType: "application/json; charset=utf-8",
                success: (data) => {
                    if (data["iFlag"] == "0") {
                        tableUsers.destroy();
                        Swal.fire(
                            'Deleted!',
                            data["sRespuesta"],
                            'success'
                        );
                        setTimeout(() => {
                            loadtabusers();
                        }, 250);
                    } else {
                        Swal.fire(
                            'Error!',
                            data["sRespuesta"],
                            'error'
                        );
                    }
                }
            });
        }
    });
}

loadmainchecks = () => {

    var btns = "";
    var item;
    var nom;

    $.ajax({
        url: "../Catalogos/Loadmainmenus",
        type: "POST",
        data: JSON.stringify({}),
        contentType: "application/json; charset=utf-8",
        success: (data) => {

            for (var i = 0; i < data.length; i++) {
                item = data[i]['iIdItem'];
                nom = data[i]['sNombre'];

                var collapse =
                    "<div class='accordion col-md-6 my-0 py-0' id='accordion" + item + "'>" +
                    "<div class='card' >" +
                    "<div class='my-0 py-0 btn btn-light' id='heading" + item + "'>" +
                    "<div class='row d-flex justify-content-between align-content-end flex-wrap'>" +
                    "<div class='custom-control custom-checkbox ml-3  h6 py-1'>" +
                    "<input type='checkbox' class='custom-control-input' id='check" + item + "' name='maincheck' value='" + item + "' status='0' onclick='loadsubchecks(\"check" + item + "\",\"collapse" + item + "\", " + item + ");'>" +
                    "<label class='custom-control-label pt-1' for='check" + item + "'>" + nom + "</label>" +
                    "</div>" +
                    "<div class='btn btn-link' onclick='changeicon(\"collapse" + item + "\",\"icon" + item + "\");' data-toggle='collapse' data-target='#collapse" + item + "' aria-expanded='true' aria-controls='collapse" + item + "'>" +
                    "<span id='icon" + item + "' class='fas fa-bars text-success fa-lg' status='0'></span>" +
                    "</div>" +
                    "</div>" +
                    "</div>" +
                    "<div id='collapse" + item + "' class='collapse my-0 py-0' aria-labelledby='heading" + item + "' data-parent='#accordion" + item + "'>" +
                    "<div class='card-body my-0 py-0' id='card-body-" + item + "'>" +
                    "</div>" +
                    "</div>" +
                    "</div>" +
                    "</div>";
                btns += collapse
            }

            $("#formbodyprofiles").html(btns);

        }
    });

}

loadsubchecks = (check, collapse, item) => {
    var status = $("#" + check).attr("status");
    var subcheck = "";
    var subchecks = "";
    if (status == "0" || status == 0) {

        $.ajax({
            url: "../Catalogos/Loadonemenu",
            type: "POST",
            data: JSON.stringify({ Id: item }),
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                subchecks = "<ul class='list-group list-group-flush'>"
                if (data.length == 0 || data == null || data == "") {

                } else {
                    for (var i = 0; i < data.length; i++) {
                        subcheck =
                            "<li class='list-group-item my-0 py-0 d-flex justify-content-start font-labels align-items-center'>" +
                            "<div class='custom-control custom-checkbox ml-3'>" +
                            "<input type='checkbox' class='custom-control-input' id='check" + data[i]["iIdItem"] + "' name='subcheck' value='" + data[i]["iIdItem"] + "' parent='" + item + "'>" +
                            "<label class='custom-control-label' for='check" + data[i]["iIdItem"] + "'>" + data[i]["sNombre"] + "</label>" +
                            "</div>" +
                            "</li>";
                        subchecks += subcheck;
                    }
                }
                subchecks += "</ul>";
                $("#" + "card-body-" + item).html(subchecks);
            }
        });

        $("#" + check).attr("status", "1");
        changeicon(collapse, "icon" + item);
    } else if (status == "1" || status == 1) {
        changeicon(collapse, "icon" + item);
    }
}

changeicon = (collapse, icon) => {

    var status = $("#" + icon).attr("status");

    if (status == "0" || status == 0) {
        $("#" + collapse).collapse("show");

        $("#" + icon).attr("class", "fas fa-times text-success fa-lg");
        $("#" + icon).attr("status", "1");
    } else if (status == "1" || status == 1) {
        $("#" + collapse).collapse("hide");

        $("#" + icon).attr("class", "fas fa-bars text-success fa-lg");
        $("#" + icon).attr("status", "0");
    }
}

// carga los checks que pertenecen a cada menu principal
loadcheck = (id, collapse) => {
    if ($('.' + collapse).prop('show')) {

    } else {
        $.ajax({
            url: "../Catalogos/Loadonemenu",
            type: "POST",
            data: JSON.stringify({ Id: id }),
            contentType: "application/json; charset=utf-8",
            success: (data) => {
                $("." + collapse).html("");
                var body = "";
                for (var i = 0; i < data.length; i++) {
                    body +=
                        "<div class='custom-control custom-checkbox'>" +
                        "<input class='custom-control-input' type='checkbox' id='" + data[i]["iParent"] + "-" + data[i]["iIdItem"] + "' name='menuitem' value='" + data[i]["iIdItem"] + "'>" +
                        "<label class='custom-control-label' for='" + data[i]["iParent"] + "-" + data[i]["iIdItem"] + "'>" + data[i]["sNombre"] + "</label>" +
                        "</div><div class='dropdown-divider'></div>";
                }
                $("." + collapse).html(body);
                $("." + collapse).collapse("show");

            }
        });
    }
}

carganewperfil = () => {

    var form = $("#frmProfiles").serialize();

    $.ajax({
        url: "../Catalogos/SaveNewPerfil",
        type: "POST",
        data: JSON.stringify({ form }),
        contentType: "application/json; charset=utf-8",
        success: (data) => {

        }
    });
}

// CARGA MODAL DEL NUEVO USUARIO
mostarmodalnewperfil = () => {
    $("#modalnewusuario").modal("show");
}

$("#btnnewperfil").on("click", function () {
    var data = $("#frmProfiles").serialize();
});

$("#btnsavenewuser").click(() => {
    var form = document.getElementById("frmNewUser");
    if (form.checkValidity() === false) {
        form.classList.add("was-validated");
    }
});

loadselectprofiles = () => {

    $.ajax({
        url: "../Catalogos/LoadProfiles",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: (data) => {
            console.log(data);
            var select = document.getElementById("inuserperfilid");
            select.innerHTML = "";
            var options = "";
            for (var i = 0; i < data.length; i++) {
                console.log(data[i]["Cancelado"]);
                if (data[i]["Cancelado"] == "False") {
                    options += "<option value='" + data[i]["IdPerfil"] + "'>" + data[i]["Perfil"] + "</option>";
                }
            }
            console.log(options);
            select.innerHTML = options
        }
    });
}