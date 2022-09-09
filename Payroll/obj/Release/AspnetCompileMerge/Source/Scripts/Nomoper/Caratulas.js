var empresa;
$(function () {

    var tabCaratulas = "";

    /// llenad el grid de Definicion
    LoadDefinicionSelect = () => {
        var opDeNombre = "Selecciona";
        var opDeCancelados = 2;
        const dataSend = { sNombreDefinicion: opDeNombre, iCancelado: opDeCancelados };
        $.ajax({
            url: "../Nomina/TpDefinicionNominaQry",
            type: "POST",
            data: dataSend,
            success: (data) => {
                //console.log(data);
                var select = document.getElementById("definicionSelect");
                select.innerHTML = "<option class='text-center' value='0'>  --Selecciona--  </option>";
                for (var i = 0; i < data.length; i++) {
                    select.innerHTML += "<option value='" + data[i].iIdDefinicionhd + "'>" + data[i].iIdDefinicionhd + " - " + data[i].sNombreDefinicion + " / " + data[i].sDescripcion + "</option>";
                }
            }
        });
    }

    /// LLENA CAMPOS DESPUES DE ELEGIR LA DEFINICION ✔
    $("#definicionSelect").change(() => {
        if ($("#definicionSelect").val() > 0) {
            //$("#btnFloEjecutar").removeAttr("disabled");
            var definicion_id = $("#definicionSelect").val();
            LoadSelectedDefinicion(definicion_id);
        } else {
            //FLimpiaCamp();
        }
    });

    LoadSelectedDefinicion = (idDefinicion) => {
        var opDeNombre = "Selecciona";
        var opDeCancelados = 2;
        const dataSend = { sNombreDefinicion: opDeNombre, iCancelado: opDeCancelados };
        $.ajax({
            url: "../Nomina/TpDefinicionNominaQry",
            type: "POST",
            data: dataSend,
            success: (data) => {
                //console.log(data);
                for (var i = 0; i < data.length; i++) {
                    if (data[i].iIdDefinicionhd == idDefinicion) {
                        //console.log(data[i]);
                        document.getElementById("inputAnio").value = data[i].iAno;
                    }
                }
                VerifyCalculosHD();
                ListEmpresa($("#definicionSelect").val());
            },
        });
    }

    /// VERIFICA EN TBCALCULOS HD
    VerifyCalculosHD = () => {
        const dataSend = { iIdDefinicionHd: $("#definicionSelect").val(), iperiodo: 0, NomCerr: 0, Anio: inputAnio.value };
        $.ajax({
            url: "../Nomina/CompruRegistroExit",
            type: "POST",
            data: dataSend,
            success: (data) => {
                //console.log(data);
                if (data.Result[0].iIdCalculosHd == 1) {
                    // tipo de periodo de la definicion
                    $("#tipoPeriodo").val(data.LTP[0].iId);
                    // ingresa el periodo
                    if (data.LPe[0].sMensaje == "success") {
                        $("#Periodo").empty();
                        document.getElementById("Periodo").value = data.LPe[0].iPeriodo;
                        //periodo = data.LPe[0].iPeriodo;
                    }
                    //if (data.LPe[0].sMensaje == "error") {
                    //    Swal.fire({ icon: 'error', title: 'Ejecucion', text: 'Algunas empresas no tienen periodo disponible!' });
                    //}
                }
                //if (data.Result[0].iIdCalculosHd == 0) {
                //    $("#btnFloGuardar").removeAttr("disabled");
                //    $("#btnFloEjecutar").attr("disabled", "disabled");
                //}
            }
        });
    }

    /// llena el drop de empresa en la pantalla ejecucion ✔
    ListEmpresa = (CalculosHd_id) => {
        empresas = "";
        const dataSend2 = { iIdCalculosHd: CalculosHd_id, iTipoPeriodo: 0, iPeriodo: 0, idEmpresa: 0, anio: 0 };
        $("#EjeEmpresa").empty();
        $('#EjeEmpresa').append('<option value="0" class="text-center">--Selecciona--</option>');
        $.ajax({
            url: "../Nomina/EmpresaCal",
            type: "POST",
            data: dataSend2,
            success: (data) => {
                //console.log(data)
                for (i = 0; i < data.length; i++) {
                    document.getElementById("Empresas").innerHTML += `<option value='${data[i].iIdEmpresa}'>${data[i].iIdEmpresa}  ${data[i].sNombreEmpresa} </option>`;
                    //empresa = empresa + "," + data[i].iIdEmpresa;
                }
            },
        });
    };

    /// Carga caratulas 
    $("#Empresas").change(() => {
        //console.log("change empresa");
        var frm = document.getElementById('frmCaratulas');

        let NominasCerradas;
        frm.classList.add("was-validated");
        setTimeout(() => { frm.classList.remove("was-validated"); }, 3000);

        if (frm.checkValidity() === false) {

            $("#notif").html("Selecciona todos los campos necesarios!");

            $("#statusDataNotification").removeClass("d-none");
            setTimeout(() => {
                $("#statusDataNotification").addClass("d-none");
            }, 3000);

        } else {
            getCaratulas();
        }
    });

    getCaratulas = () => {
        let tipoRecibo = ($("#inrecibo2").prop("checked")) ? 1 : 0;

        //console.log("true");
        $.ajax({
            url: "../Nomina/getCaratulas",
            type: "POST",
            cache: false,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({
                Definicion_id: $("#definicionSelect").val(),
                TipoPeriodo: $("#tipoPeriodo").val(),
                Periodo: $("#Periodo").val(),
                Empresa_id: $("#Empresas").val(),
                Anio: $("#inputAnio").val(),
                TipoRecibo: tipoRecibo
            }),
            success: (data) => {

                //console.log(data);
                var tabContent = "";

                //console.log(tabContent);
                if (data.length > 0) {

                    for (var i = 0; i < data.length; i++) {
                        if (data[i].sNombreRenglon == "null") {
                            console.log("string");
                        }
                        if (data[i].sNombreRenglon == null) {
                            console.log("null");
                        }
                        tabContent += "<tr class='text-center'><td>" + data[i].iIdRenglon + "</td>";
                        tabContent += "<td>" + data[i].sNombreRenglon + "</td>";
                        tabContent += "<td>" + data[i].sTotal + "</td></tr>";
                        if (data[i].iIdRenglon == 990) {
                            per = data[i].dTotal;
                            $("#outTotalPercepciones").val(data[i].sTotal);
                        }
                        if (data[i].iIdRenglon == 1990) {
                            dedu = data[i].dTotal;
                            $("#outTotalDeducciones").val(data[i].sTotal);
                            total = per - dedu;
                            total = Math.round(total * 100);
                            total = total / 100;
                        }
                        if (data[i].iIdRenglon == 9999) {
                            $("#outTotalNeto").val(data[i].sTotal);
                        }
                    }
                    $("#detalleTabCaratulas").html(tabContent);

                    $("#tabCaratulas").removeClass("d-none");
                    //tabCaratulas = $("#tabCaratulas").DataTable({
                    //    "language": {
                    //        "url": "//cdn.datatables.net/plug-ins/1.10.15/i18n/Spanish.json",
                    //    },
                    //    ordering: false,
                    //    paging: false,
                    //    searching: false,
                    //});
                } else {
                    $("#notif").html("No se encontraron datos. Intenta con otra empresa!");
                    $("#statusDataNotification").removeClass("d-none");
                    setTimeout(() => {
                        $("#statusDataNotification").addClass("d-none");
                    }, 3000);
                    if (tabCaratulas != "") {
                        //$("#tabCaratulas").DataTable.destroy();
                    }
                    //tabCaratulas = null;

                    $("#tabCaratulas").addClass("d-none");
                }
            }
        });


    }

    LoadEmpresaSelected = (Empresa_id) => {
        $.ajax({
            url: "../Empresas/LoadEmpresa",
            type: "POST",
            data: { IdEmpresa: Empresa_id },
            success: (data) => {
                console.log("LoadEmpresa");
                console.log(data);
                if (data.length > 0) {
                    GeneraCaratulaPDF(data[2], data[9], data[12]);
                } else {
                    Swal.fire({ icon: 'warning', title: 'Consulta', text: 'Hubo un error al cargar los datos de la empresa, reinicia sesión' });
                }
            }
        });
    }

    $("#inrecibo2").change(() => {
        console.log($("#definicionSelect").val());
        console.log($("#Empresas").val());
        //let tipoRecibo = ($("#inrecibo2").prop("checked")) ? 1 : 0;
        if ($("#definicionSelect").val() == '0' || $("#definicionSelect").val() == '') {
            Swal.fire({ icon: 'info', title: 'Validación', text: 'Selecciona una definición!' });
            document.getElementById("inrecibo2").checked = false;
        } else {
            if ($("#Empresas").val() == '0' || $("#Empresas").val() == '') {
                Swal.fire({ icon: 'info', title: 'Validación', text: 'Selecciona una empresa!' });
                document.getElementById("inrecibo2").checked = false;
            } else {
                getCaratulas();
            }
        }
    });


    GeneraCaratulaPDF = (NombreEmpresa, RFCEmpresa, RegistroIMSS) => {
        const options2 = { style: 'currency', currency: 'USD' };
        const numberFormat2 = new Intl.NumberFormat('en-US', options2);

        let tipoRecibo = ($("#inrecibo2").prop("checked")) ? 1 : 0;

        var dataSend = {
            iTipoPeriodo: $("#tipoPeriodo").val(),
            iPeriodo: $("#Periodo").val(),
            idEmpresa: $("#Empresas").val(),
            Anio: $("#inputAnio").val(),
            carat: tipoRecibo
        };
        console.log("Datasend");
        console.log(dataSend);
        $.ajax({
            url: "../Nomina/ListTpCalculolnPDF",
            type: "POST",
            data: dataSend,
            success: (data) => {
                console.log("caratula body");
                console.log(data);
                var doc = new jsPDF();
                //let pageWidth = doc.internal.pageSize.getWidth();
                n = new Date();
                //Año
                y = n.getFullYear();
                //Mes
                m = n.getMonth() + 1;
                //Día
                d = n.getDate();

                //Lo ordenas a gusto.
                doc.setFontSize(15);
                doc.setFontType("bold")

                //console.log('Bloque empresa');
                doc.setFontSize(8);
                doc.setFontSize(8);
                doc.text(140, 40, 'Fecha:');
                doc.setFontType("normal");
                doc.text(150, 40, d + "/" + m + "/" + y);

                doc.setFontType("bold");
                doc.text(15, 30, "Empresa: ");
                doc.text("REPORTE DE NÓMINA", 105, 20, null, null, "center");
                doc.setFontType("normal");
                //doc.text(80, 30, 'Reporte De Nómina');
                
                doc.text(30, 30, NombreEmpresa);

                doc.setFontType("bold");
                doc.text(15, 35, "RFC: ");
                doc.setFontType("normal");
                doc.text(22, 35, RFCEmpresa);
                doc.setFontType("bold");
                doc.text(15, 40, "Reg.IMSS: ");

                doc.setFontType("normal");
                doc.text(30, 40, RegistroIMSS);
                doc.setFontType("bold");
                doc.text(140, 30, "Tipo de periodo: ");
                doc.setFontType("normal");
                doc.text(163, 30, $("#tipoPeriodo").val());

                doc.setFontType("bold");
                doc.text(140, 35, "Periodo: ");
                doc.setFontType("normal");
                doc.text(153, 35, $("#Periodo").val());

                doc.setLineWidth(0.4);
                doc.line(3, 45, 205, 45);
                doc.setFontType("bold")
                doc.setFontSize(8);
                doc.text(15, 50, "Calve");
                doc.text(30, 50, "Descripción")
                doc.text(70, 50, "Dato")
                doc.text(90, 50, "Percepción");
                doc.text(120, 50, "Deducción");
                doc.text(150, 50, "Gravado")
                doc.text(180, 50, "Exento")


                doc.line(3, 55, 205, 55);


                var rowscounts = data.length;
                var FilNomLey = 65;
                doc.setFontType("normal");
                var grav = 0;
                var exen = 0;

                doc.setFontSize(7);
                doc.setFontType("bold");
                FilNomLey = FilNomLey + 7;
                var row990;
                var row1990;
                var row9999;
                var totalp;
                var totald;
                var total;
                var totalpd;
                for (i = 0; i < rowscounts;) {

                    if (data[i].iIdRenglon != 990 && data[i].iIdRenglon != 1990 && data[i].iIdRenglon != 9999 && data[i].iInformativo == "False") {
                        doc.text(15, FilNomLey, " " + data[i].iIdRenglon);
                        doc.text(28, FilNomLey, " " + data[i].sNombreRenglon);
                        if (data[i].sValor == 'Percepciones') {

                            //     var TPercepcio = data[i].dTotalSaldo;
                            //                            TPercepcio = TPercepcio.toLocaleString("es-AR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });

                            var TPercepcio = numberFormat2.format(data[i].dTotalSaldo);
                            TPercepcio = TPercepcio.replace('$', '$ ')
                            // TPercepcio = TPercepcio.replace('.', ',')
                            //TPercepcio = TPercepcio.replace('%', '.')

                            doc.text(90, FilNomLey, TPercepcio);
                            doc.text(120, FilNomLey, "$ 0");
                        }
                        if (data[i].sValor == 'Deducciones') {
                            var TDeduc = numberFormat2.format(data[i].dTotalSaldo);
                            TDeduc = TDeduc.replace('$', '$ ')
                            // TDeduc = TDeduc.replace('.', ',')
                            // TDeduc = TDeduc.replace('%', '.')

                            doc.text(90, FilNomLey, "$ 0")
                            doc.text(120, FilNomLey, " " + TDeduc);
                        }

                        grav = grav + data[i].dTotalGravado;
                        exen = exen + data[i].dTotalExento;

                        var Grav2 = numberFormat2.format(data[i].dTotalGravado);

                        Grav2 = Grav2.replace('$', '$ ')
                        //Grav2 = Grav2.replace('.', ',')
                        //Grav2 = Grav2.replace('%', '.')

                        var Exen2 = numberFormat2.format(data[i].dTotalExento);

                        Exen2 = Exen2.replace('$', '$ ')
                        // Exen2 = Exen2.replace('.', ',')
                        // Exen2 = Exen2.replace('%', '.')

                        doc.text(150, FilNomLey, Grav2);
                        doc.text(180, FilNomLey, Exen2);



                        FilNomLey = FilNomLey + 5;
                    }
                    if (data[i].iIdRenglon == 990) {

                        totalp = data[i].dTotalSaldo;
                        row990 = numberFormat2.format(data[i].dTotalSaldo);
                        row990 = row990.replace('$', '$ ')
                        // row990 = row990.replace('.', ',')
                        // row990 = row990.replace('%', '.')



                    }
                    if (data[i].iIdRenglon == 1990) {

                        totald = data[i].dTotalSaldo
                        row1990 = numberFormat2.format(data[i].dTotalSaldo);
                        row1990 = row1990.replace('$', '$ ')
                        // row1990 = row1990.replace('.', ',')
                        // row1990 = row1990.replace('%', '.')

                    }

                    if (data[i].iIdRenglon == 9999) {

                        row9999 = numberFormat2.format(data[i].dTotalSaldo);
                        row9999 = row9999.replace('$', '$ ')
                        //row9999 = row9999.replace('.', ',')
                        //row9999 = row9999.replace('%', '.')

                    }

                    i++;

                };

                total = totalp - totald;
                totalpd = numberFormat2.format(total);
                totalpd = totalpd.replace('$', '$ ')

                grav = grav.toFixed(2);
                exen = exen.toFixed(2);
                var dTgravado = numberFormat2.format(grav);
                dTgravado = dTgravado.replace('$', '$ ')
                //dTgravado = dTgravado.replace('.', ',')
                //dTgravado = dTgravado.replace('%', '.')
                var dTExento = numberFormat2.format(exen);
                dTExento = dTExento.replace('$', '$ ')
                //dTExento = dTExento.replace('.', ',')
                //dTExento = dTExento.replace('%', '.')

                doc.line(3, FilNomLey, 205, FilNomLey);
                FilNomLey = FilNomLey + 5;
                doc.text(80, FilNomLey, "Total:");
                doc.setFontType("bold");
                doc.text(89, FilNomLey, " " + row990);
                doc.text(110, FilNomLey, "Total:");
                doc.setFontType("bold");
                doc.text(120, FilNomLey, " " + row1990);
                doc.text(140, FilNomLey, "Total:");
                doc.setFontType("bold");
                doc.text(150, FilNomLey, " " + dTgravado);
                doc.text(170, FilNomLey, "Total:");
                doc.setFontType("bold");
                doc.text(180, FilNomLey, " " + dTExento);


                FilNomLey = FilNomLey + 5;
                doc.text(150, FilNomLey, "Neto a pagar:");
                doc.setFontType("bold");
                doc.text(170, FilNomLey, row9999);



                doc.save('Caratula_E' + $("#Empresas").val() + '.pdf');



            }
        });


    };

    $("#btnCaratulasPDFdwl").click(() => { LoadEmpresaSelected($("#Empresas").val()); });

    LoadDefinicionSelect();
});