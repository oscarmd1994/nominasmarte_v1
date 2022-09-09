$(function () {

    fAccessUser = () => {
        try {
            $.ajax({
                url: "../Permisos/UsuarioPermisoConsulta",
                type: "POST",
                data: {},
                success: (request) => {
                    if (request.Consulta) {
                        const elementsInput  = document.querySelectorAll('input');
                        const elementsButton = document.querySelectorAll('button');
                        const elementsSelect = document.querySelectorAll('select');
                        const elementsALink  = document.querySelectorAll('a');
                        const elementsDiv    = document.querySelectorAll('div');
                        for (var i = 0; i < elementsInput.length; i++) {
                            if (elementsInput[i].getAttribute("data-read") != null) {
                                elementsInput[i].disabled = true;
                            }
                        }
                        for (var t = 0; t < elementsButton.length; t++) {
                            if (elementsButton[t].getAttribute("btn-data-read") != null) {
                                elementsButton[t].remove();
                            }
                        }
                        for (var p = 0; p < elementsSelect.length; p++) {
                            let attribute = elementsSelect[p].getAttribute("slc-data-read");
                            if (attribute == "true") {
                                elementsSelect[p].disabled = true;
                            }
                        }
                        for (var a = 0; a < elementsALink.length; a++) {
                            if (elementsALink[a].getAttribute("link-data-read") != null) {
                                elementsALink[a].remove();
                            }
                        }
                        for (var f = 0; f < elementsDiv.length; f++) {
                            if (elementsDiv[f].getAttribute("div-read") != null) {
                                elementsDiv[f].remove();
                            }
                        }
                    }
                }, error: (jqXHR, exception) => {
                    fcaptureaerrorsajax(jqXHR, exception);

                }
            });
        } catch (error) {
            if (error instanceof EvalError) {
                console.error('EvalError: ', error.message);
            } else if (error instanceof RangeError) {
                console.error('RangeError: ', error.message);
            } else if (error instanceof TypeError) {
                console.error('TypeError: ', error.message);
            } else {
                console.error('Error ', error);
            }
        }
    }

    fAccessUser();

});