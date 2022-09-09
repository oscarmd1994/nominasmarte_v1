$(function () {
    console.log('Funcionando')
    // ** Configuracion toastrjs ** \\

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-left",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    // ** Declaracion de variables ** \\

    const state = document.getElementById('state');
    const codpost = document.getElementById('codpost');
    const city = document.getElementById('city');
    //const delega    = document.getElementById('delega');
    const colony = document.getElementById('colony');
    const street = document.getElementById('street');
    const numberst = document.getElementById('numberst');
    const entstreet = document.getElementById('entstreet');

    const title = document.getElementById('title');
    const estciv = document.getElementById('estciv');

    const btnVerifCodPost = document.getElementById('btn-verif-codpost');

    // ** Funcion que deshabilita los inputs de domicilio  ** \\
    fdisabledfields = (flag) => {
        //city.disabled      = flag;
        //delega.disabled    = flag;
        colony.disabled = flag;
        street.disabled = flag;
        numberst.disabled = flag;
        entstreet.disabled = flag;

    }

    city.disabled = true;
    codpost.disabled = true;
    btnVerifCodPost.disabled = true;
    fdisabledfields(true);

    const getDataTabDataGen = JSON.parse(localStorage.getItem('objectTabDataGen'));

    // ** Funcion que carga los estados ** \\
    floadstates = () => {
        try {
            $.ajax({
                url: "../Empleados/LoadStates",
                type: "POST",
                data: {},
                contentType: "application/json; charset=utf-8",
                success: (data) => {
                    const quantity = data.length;
                    let stated;
                    for (t in getDataTabDataGen) {
                        if (getDataTabDataGen[t].key === "general") {
                            stated = getDataTabDataGen[t].data.state;
                        }
                    }
                    if (quantity > 0) {
                        for (i = 0; i < data.length; i++) {
                            if (stated == data[i].iIdValor) {
                                state.innerHTML += `
                                    <option selected value="${data[i].iIdValor}">${data[i].sValor}</option>
                                `;
                            } else {
                                state.innerHTML += `
                                    <option value="${data[i].iIdValor}">${data[i].sValor}</option>
                                `;
                            }
                        }
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof TypeError) {
                console.log("TypeError ", error);
            } else if (error instanceof RangeError) {
                console.log("RangeError ", error);
            } else if (error instanceof EvalError) {
                console.log("EvalError", error);
            }
        }
    }

    floadstates();

    state.addEventListener('change', () => {
        colony.innerHTML = '<option value="0">Selecciona</option>'
        fdisabledfields(true);
        city.value = "";
        street.value = "";
        numberst.value = "";
        entstreet.value = "";
        //Command: toastr["info"]("Ahora ingresa el cÃ³digo postal");
        if (state.value != "0") {
            codpost.disabled = false;
            btnVerifCodPost.disabled = false;
            btnVerifCodPost.classList.add('btn-info');
            setTimeout(() => { codpost.focus() }, 500);
        } else {
            codpost.disabled = true;
            btnVerifCodPost.disabled = true;
            btnVerifCodPost.classList.remove('btn-info');
        }
    });

    btnVerifCodPost.addEventListener('click', () => {
        if (codpost.value.length === 5) {
            document.getElementById('load-spinner').classList.remove('d-none');
            btnVerifCodPost.classList.add('d-none');
            setTimeout(() => {
                $.ajax({
                    url: "../Empleados/LoadInformationHome",
                    type: "POST",
                    data: { codepost: codpost.value, state: state.value },
                    success: (data) => {
                        console.log(data.length);
                        console.log('El estado es: ' + state.value);
                        console.log('El codigo postal es: ' + codpost.value);
                        if (data.length > 0) {
                            fdisabledfields(false);
                            for (i = 0; i < data.length; i++) {
                                city.value = data[i].sCiudad;
                                if (data[i].sColonia != "") {
                                    colony.innerHTML += `<option>${data[i].sColonia}</option>`;
                                }
                            }
                        } else {
                            Swal.fire({
                                title: "AtenciÃ³n",
                                text: "El cÃ³digo postal ingresado es incorrecto no pertenece al estado seleccionado",
                                icon: "warning",
                                showClass: { popup: 'animated fadeInDown faster' },
                                hideClass: { popup: 'animated fadeOutUp faster' },
                                confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
                            }).then((acepta) => {
                                setTimeout(() => { codpost.focus(); }, 800)
                            });
                        }
                        btnVerifCodPost.classList.remove('d-none');
                        document.getElementById('load-spinner').classList.add('d-none')
                    }, error: (jqXHR, exception) => {
                        fcaptureaerrorsajax(jqXHR, exception);
                    }
                });
            }, 2000);
        } else {
            Swal.fire({
                title: "AtenciÃ³n", text: "El cÃ³digo postal debe de contener 5 caracteres", icon: "warning",
                showClass: { popup: 'animated fadeInDown faster' },
                hideClass: { popup: 'animated fadeOutUp faster' },
                confirmButtonText: "Aceptar", allowOutsideClick: false, allowEscapeKey: false, allowEnterKey: false,
            }).then((acepta) => {
                setTimeout(() => { codpost.focus(); }, 800);
            });
        }
    });

    // ** Funcion que carga los datos del select titulo ** \\

    floadtitle = () => {
        try {
            $.ajax({
                url: "../Empleados/LoadTitles",
                type: "POST",
                data: { state: 1, type: 'Active/Desactive', keytitle: 0 },
                success: (data) => {
                    const quantity = data.length;
                    let titled;
                    for (t in getDataTabDataGen) {
                        if (getDataTabDataGen[t].key === "general") {
                            titled = getDataTabDataGen[t].data.title;
                        }
                    }
                    if (quantity > 0) {
                        for (var i = 0; i < data.length; i++) {
                            if (titled == data[i].iIdTitulo) {
                                title.innerHTML += `<option selected value="${data[i].iIdTitulo}">${data[i].sNombreTitulo}</option>`;
                            } else {
                                title.innerHTML += `<option value="${data[i].iIdTitulo}">${data[i].sNombreTitulo}</option>`;
                            }
                        }
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof TypeError) {
                console.log('TypeError ', error);
            } else if (error instanceof RangeError) {
                console.log('RangeError ', error);
            } else if (error instanceof EvalError) {
                console.log('EvalError', error);
            } else {
                console.log('Error', error);
            }
        }
    }

    floadtitle();

    // ** Funcion que carga los datos del estado civil ** \\

    floadstatecivil = () => {
        try {
            $.ajax({
                url: "../Empleados/LoadStateCivil",
                type: "POST",
                data: { state: 1, type: 'Active/Desactive', keystate: 0 },
                success: (data) => {
                    const quantity = data.length;
                    let estateciv;
                    for (t in getDataTabDataGen) {
                        if (getDataTabDataGen[t].key === "general") {
                            estateciv = getDataTabDataGen[t].data.estciv;
                        }
                    }
                    if (quantity > 0) {
                        for (var i = 0; i < data.length; i++) {
                            if (estateciv == data[i].iIdEstadoCivil) {
                                estciv.innerHTML += `<option selected value="${data[i].iIdEstadoCivil}">${data[i].sNombreEstadoCivil}</option>`;
                            } else {
                                estciv.innerHTML += `<option value="${data[i].iIdEstadoCivil}">${data[i].sNombreEstadoCivil}</option>`;
                            }
                        }
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof TypeError) {
                console.log('TypeError ', error);
            } else if (error instanceof RangeError) {
                console.log('RangeError ', error);
            } else if (error instanceof EvalError) {
                console.log('EvalError', error);
            } else {
                console.log('Error', error);
            }
        }
    }

    floadstatecivil();

    // CARGA DE CATALOGOS VISTA IMMS \\

    const nivest = document.getElementById('nivest');
    const getDataTabImss = JSON.parse(localStorage.getItem('objectDataTabImss'));

    floadnivelstudy = () => {
        try {
            $.ajax({
                url: "../Empleados/LoadNivelStudy",
                type: "POST",
                data: { state: 1, type: 'Active/Desactive', keynivel: 0 },
                success: (data) => {
                    const quantity = data.length;
                    let nivestd = 0;
                    if (JSON.parse(localStorage.getItem('objectDataTabImss')) != null) {
                        for (i in getDataTabImss) {
                            if (getDataTabImss[i].key === "imss") {
                                nivestd = getDataTabImss[i].data.nivest;
                            }
                        }
                    }
                    if (quantity > 0) {
                        for (var i = 0; i < data.length; i++) {
                            if (nivestd == data[i].iIdNivelEstudio) {
                                nivest.innerHTML += `<option selected value="${data[i].iIdNivelEstudio}">${data[i].sNombreNivelEstudio}</option>`;
                            } else {
                                nivest.innerHTML += `<option value="${data[i].iIdNivelEstudio}">${data[i].sNombreNivelEstudio}</option>`;
                            }
                        }
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof TypeError) {
                console.log('TypeError ', error);
            } else if (error instanceof RangeError) {
                console.log('RangeError ', error);
            } else if (error instanceof EvalError) {
                console.log('EvalError ', error);
            } else {
                console.log('Error ', error);
            }
        }
    }

    floadnivelstudy();

    const nivsoc = document.getElementById('nivsoc');

    floadnivelsocioecon = () => {
        try {
            $.ajax({
                url: "../Empleados/LoadNivelSocioecon",
                type: "POST",
                data: { state: 1, type: 'Active/Desactive', keynivel: 0 },
                success: (data) => {
                    const quantity = data.length;
                    let nivsocd = 0;
                    if (JSON.parse(localStorage.getItem('objectDataTabImss')) != null) {
                        for (i in getDataTabImss) {
                            if (getDataTabImss[i].key === "imss") {
                                nivsocd = getDataTabImss[i].data.nivsoc;
                            }
                        }
                    }
                    if (quantity > 0) {
                        for (var i = 0; i < data.length; i++) {
                            if (nivsocd == data[i].iIdNivelSocioeconomico) {
                                nivsoc.innerHTML += `<option selected value="${data[i].iIdNivelSocioeconomico}">${data[i].sNombreNivelSocioeconomico}</option>`;
                            } else {
                                nivsoc.innerHTML += `<option value="${data[i].iIdNivelSocioeconomico}">${data[i].sNombreNivelSocioeconomico}</option>`;
                            }
                        }
                    } else {
                        console.error('Ocurrio un problema al cargar');
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof TypeError) {
                console.log('TypeError ', error);
            } else if (error instanceof RangeError) {
                console.log('RangeError ', error);
            } else if (error instanceof EvalError) {
                console.log('EvalError ', error);
            } else {
                console.log('Error ', error);
            }
        }
    }

    floadnivelsocioecon();

    const tiphab = document.getElementById('tiphab');

    floadtypehabit = () => {
        try {
            $.ajax({
                url: "../Empleados/LoadTypeHabit",
                type: "POST",
                data: { state: 1, type: 'Active/Desactive', keytype: 0 },
                success: (data) => {
                    const quantity = data.length;
                    let tiphabd = 0;
                    if (JSON.parse(localStorage.getItem('objectDataTabImss')) != null) {
                        for (i in getDataTabImss) {
                            if (getDataTabImss[i].key === "imss") {
                                tiphabd = getDataTabImss[i].data.tiphab;
                            }
                        }
                    }
                    if (quantity > 0) {
                        for (i = 0; i < data.length; i++) {
                            if (tiphabd == data[i].iIdTipoHabitacion) {
                                tiphab.innerHTML += `<option selected value="${data[i].iIdTipoHabitacion}">${data[i].sNombreHabitacion}</option>`;
                            } else {
                                tiphab.innerHTML += `<option value="${data[i].iIdTipoHabitacion}">${data[i].sNombreHabitacion}</option>`;
                            }
                        }
                    } else {
                        console.error('Ocurrio un problema al cargar');
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);
                }
            });
        } catch (error) {
            if (error instanceof TypeError) {
                console.log('TypeError ', error);
            } else if (error instanceof RangeError) {
                console.log('RangeError ', error);
            } else if (error instanceof EvalError) {
                console.log('EvalError ', error);
            } else {
                console.log('Error ', error);
            }
        }
    }

    floadtypehabit();

});