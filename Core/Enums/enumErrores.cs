namespace Core.Enums
{
    // Catalogo de errores agrupado por rango, como un catalogo de errores real:
    //   Negativos (-1 a -99)  -> errores de sistema / base de datos (no atribuibles al usuario)
    //   1    a 99             -> validaciones generales de campos
    //   100  a 199            -> autenticacion / login
    //   200  a 299            -> registro de usuario                  (reservado, sprint futuro)
    //   300  a 399            -> recuperacion / cambio de contrasena
    //   400  a 499            -> perfil / actualizacion de usuario    (reservado)
    //   500  a 599            -> productos                            (reservado)
    //   600  a 699            -> listas de compra                     (reservado)
    //   700  a 799            -> comercios / precios                  (reservado)
    //   800  a 899            -> guilds
    //   900  a 999            -> catalogos (categorias, unidades)      (reservado)
    //   1000 a 1099           -> historial de busquedas
    //   1100 a 1199           -> sincronizacion
    //   1200 a 1299           -> ordenes de pago (Tilopay)
    //   1300 a 1399           -> imagenes (Firebase Storage)
    //   1400 a 1499           -> token de dispositivo / push (Firebase Cloud Messaging)
    public enum enumErrores
    {
        // === Sistema / Base de datos ===
        errorNoControlado = -2,
        errorBaseDatos = -1,

        // === Validacion general (1-99) ===
        emailFaltante = 1,
        emailInvalido = 2,
        passwordFaltante = 3,
        nombreCategoriaFaltante = 4,
        nombreFaltante = 5,
        codigoInvitacionFaltante = 6,
        guildNoEncontrado = 7,

        nombreUnidadFaltante = 8,

        // "mensaje" del error trae el campo y su maximo, ej. "campoDemasiadoLargo: nombre (max 100)".
        passwordDebil = 9,
        campoDemasiadoLargo = 10,

        // === Autenticacion / Login (100-199) ===
        credencialesInvalidas = 100,
        usuarioInactivo = 101,
        errorGenerandoSesion = 102,

        // === Registro de usuario (200-299) ===
        correoYaRegistrado = 200,

        // === Recuperacion / cambio de contrasena (300-399) ===
        codigoRecuperacionFaltante = 300,
        codigoRecuperacionInvalido = 301,
        errorEnvioCorreo = 302,
        demasiadasSolicitudes = 303,
        demasiadosIntentos = 304,

        // === Perfil / actualizacion de usuario (400-499) ===
        usuarioNoEncontrado = 400,

        // === Guilds (800-899) ===
        nombreGuildFaltante = 800,
        codigoInvitacionDuplicado = 801,
        usuarioSinGuild = 802,
        usuarioYaTieneGuild = 803,

        // === Productos (500-599) ===
        codigoBarrasFaltante = 500,
        codigoBarrasDuplicado = 501,
        productoNoEncontrado = 502,
        nombreProductoFaltante = 503,
        guidProductoInvalido = 504,
        precioInvalido = 505,
        criterioBusquedaFaltante = 506,

        // === Comercios / precios (700-799) ===
        latitudFaltante = 700,
        longitudFaltante = 701,
        direccionComercioFaltante = 702,
        latitudFueraDeRango = 703,
        longitudFueraDeRango = 704,
        radioKmInvalido = 705,
        coordenadasInvalidas = 706,

        // === Historial de busquedas (1000-1099) ===
        guidUsuarioFaltante = 1000,
        terminoBusquedaFaltante = 1001,

        // === Listas de compra (600-699) ===
        nombreListaFaltante = 600,
        guidUsuarioListaFaltante = 601,
        listaNoExiste = 602,
        conflictoModificacionLista = 603,
        guidItemFaltante = 604,
        guidListaFaltante = 605,
        guidProductoFaltante = 606,
        itemListaNoExiste = 607,
        conflictoModificacionItem = 608,
        nombreComercioFaltante = 609,
        guidComercioFaltante = 610,
        comercioNoEncontrado = 611,
        cantidadInvalida = 612,
        precioEstimadoInvalido = 613,

        // === Sincronizacion (1100-1199) ===
        guidUsuarioFaltanteSincronizacion = 1100,

        // === Ordenes de pago / Tilopay (1200-1299) ===
        montoInvalido = 1200,
        ordenPagoNoEncontrada = 1201,
        errorProcesadorPago = 1202,
        idLinkTilopayFaltante = 1203,
        monedaInvalida = 1204,

        // === Imagenes / Firebase Storage (1300-1399) ===
        archivoImagenFaltante = 1300,
        archivoImagenInvalido = 1301,
        archivoImagenMuyGrande = 1302,

        // === Token de dispositivo / push (1400-1499) ===
        tokenDispositivoFaltante = 1400,
        plataformaInvalida = 1401,
        tokenDispositivoInvalido = 1402,
    }
}
