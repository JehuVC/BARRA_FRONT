using Core.Entidades.Enti_Clases;
using Core.Enums;

namespace BarraApp.Mobile.Services.Api;

// Traduce los codigos de enumErrores del backend a mensajes para el usuario
// (FE-S10-03). Las pantallas nuevas llaman a Traducir() en vez de mostrar el
// "mensaje" crudo (que es el nombre del enum, ej. "correoYaRegistrado").
public static class MensajesError
{
    public const string PorDefecto = "Ocurrió un error. Intenta de nuevo.";

    public static string Traducir(IEnumerable<Clase_Error>? errores, string porDefecto = PorDefecto)
    {
        var primero = errores?.FirstOrDefault();
        return primero is null ? porDefecto : Traducir(primero, porDefecto);
    }

    public static string Traducir(Clase_Error error, string porDefecto = PorDefecto)
    {
        if (error.codigo == enumErrores.campoDemasiadoLargo)
        {
            return CampoDemasiadoLargo(error.mensaje);
        }

        return error.codigo switch
        {
            enumErrores.errorNoControlado => "Ocurrió un error inesperado. Intenta de nuevo.",
            enumErrores.errorBaseDatos => "No se pudo completar la operación. Intenta de nuevo más tarde.",

            enumErrores.emailFaltante => "Ingresa tu correo electrónico.",
            enumErrores.emailInvalido => "El correo electrónico no es válido.",
            enumErrores.passwordFaltante => "Ingresa tu contraseña.",
            enumErrores.passwordDebil => "La contraseña debe tener al menos 8 caracteres, con letras y números.",
            enumErrores.nombreFaltante => "Ingresa tu nombre.",

            enumErrores.credencialesInvalidas => "Correo o contraseña incorrectos.",
            enumErrores.usuarioInactivo => "Esta cuenta está inactiva. Contacta a soporte.",
            enumErrores.errorGenerandoSesion => "No se pudo iniciar la sesión. Intenta de nuevo.",
            enumErrores.correoYaRegistrado => "Ya existe una cuenta con ese correo.",

            enumErrores.codigoRecuperacionFaltante => "Ingresa el código que recibiste por correo.",
            enumErrores.codigoRecuperacionInvalido => "El código es inválido o ya venció.",
            enumErrores.errorEnvioCorreo => "No pudimos enviar el correo. Intenta de nuevo más tarde.",
            enumErrores.demasiadasSolicitudes => "Pediste demasiados códigos. Espera unos minutos e intenta de nuevo.",
            enumErrores.demasiadosIntentos => "Demasiados intentos fallidos. Espera unos minutos e intenta de nuevo.",
            enumErrores.usuarioNoEncontrado => "No encontramos tu usuario.",

            enumErrores.nombreGuildFaltante => "Ingresa el nombre del gremio.",
            enumErrores.codigoInvitacionFaltante => "Ingresa el código de invitación.",
            enumErrores.guildNoEncontrado => "No existe un gremio con ese código.",
            enumErrores.codigoInvitacionDuplicado => "No se pudo generar el código del gremio. Intenta de nuevo.",
            enumErrores.usuarioSinGuild => "No perteneces a ningún gremio.",
            enumErrores.usuarioYaTieneGuild => "Ya perteneces a un gremio. Sal de él antes de crear o unirte a otro.",

            enumErrores.nombreCategoriaFaltante => "Ingresa el nombre de la categoría.",
            enumErrores.nombreUnidadFaltante => "Ingresa el nombre de la unidad.",

            enumErrores.codigoBarrasFaltante => "Falta el código de barras.",
            enumErrores.codigoBarrasDuplicado => "Ya existe un producto con ese código de barras.",
            enumErrores.productoNoEncontrado => "No se encontró el producto.",
            enumErrores.nombreProductoFaltante => "Ingresa el nombre del producto.",
            enumErrores.guidProductoInvalido => "El producto no es válido.",
            enumErrores.precioInvalido => "El precio no es válido.",
            enumErrores.criterioBusquedaFaltante => "Escribe qué producto buscas.",

            enumErrores.latitudFaltante or enumErrores.longitudFaltante or enumErrores.coordenadasInvalidas
                => "La ubicación no es válida.",
            enumErrores.latitudFueraDeRango or enumErrores.longitudFueraDeRango => "La ubicación está fuera de rango.",
            enumErrores.radioKmInvalido => "El radio de búsqueda no es válido.",
            enumErrores.direccionComercioFaltante => "Ingresa la dirección del comercio.",
            enumErrores.nombreComercioFaltante => "Ingresa el nombre del comercio.",
            enumErrores.comercioNoEncontrado => "No se encontró el comercio.",
            enumErrores.guidComercioFaltante => "Falta seleccionar el comercio.",

            enumErrores.nombreListaFaltante => "Ingresa el nombre de la lista.",
            enumErrores.listaNoExiste => "La lista ya no existe.",
            enumErrores.conflictoModificacionLista => "Otra persona modificó esta lista. Recárgala e intenta de nuevo.",
            enumErrores.itemListaNoExiste => "El producto ya no está en la lista.",
            enumErrores.conflictoModificacionItem => "Otra persona modificó este producto. Recarga la lista e intenta de nuevo.",
            enumErrores.cantidadInvalida => "La cantidad debe ser mayor que 0.",
            enumErrores.precioEstimadoInvalido => "El precio estimado no es válido.",
            enumErrores.guidListaFaltante or enumErrores.guidItemFaltante or enumErrores.guidProductoFaltante
                or enumErrores.guidUsuarioFaltante or enumErrores.guidUsuarioListaFaltante
                or enumErrores.guidUsuarioFaltanteSincronizacion
                => "Falta un dato requerido. Intenta de nuevo.",
            enumErrores.terminoBusquedaFaltante => "Escribe qué quieres buscar.",

            enumErrores.montoInvalido => "El monto no es válido.",
            enumErrores.monedaInvalida => "La moneda no es válida.",
            enumErrores.ordenPagoNoEncontrada => "No se encontró la orden de pago.",
            enumErrores.errorProcesadorPago => "No se pudo comunicar con el procesador de pagos. Intenta más tarde.",
            enumErrores.idLinkTilopayFaltante => "Falta el identificador del pago.",

            enumErrores.archivoImagenFaltante => "Selecciona una imagen.",
            enumErrores.archivoImagenInvalido => "La imagen debe ser JPG, PNG o WebP.",
            enumErrores.archivoImagenMuyGrande => "La imagen es demasiado grande (máximo 5 MB).",

            enumErrores.tokenDispositivoFaltante or enumErrores.tokenDispositivoInvalido
                or enumErrores.plataformaInvalida
                => "No se pudo registrar el dispositivo para notificaciones.",

            _ => porDefecto
        };
    }

    // El backend manda "campoDemasiadoLargo: nombre (max 100)".
    private static string CampoDemasiadoLargo(string? mensaje)
    {
        if (!string.IsNullOrWhiteSpace(mensaje))
        {
            int separador = mensaje.IndexOf(':');
            string detalle = separador >= 0 ? mensaje[(separador + 1)..].Trim() : mensaje.Trim();
            int abre = detalle.IndexOf('(');
            if (abre > 0 && detalle.EndsWith(')'))
            {
                string campo = detalle[..abre].Trim();
                string maximo = detalle[(abre + 1)..^1].Replace("max", string.Empty).Trim();
                return $"El campo «{campo}» es demasiado largo (máximo {maximo} caracteres).";
            }
        }

        return "Uno de los textos es demasiado largo.";
    }
}
