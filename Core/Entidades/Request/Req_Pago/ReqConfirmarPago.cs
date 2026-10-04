namespace Core.Entidades.Request.Req_Pago
{
    // Solo trae el ID_LINK_TILOPAY que llego en el webhook/callback. Nunca se
    // usa nada mas de ese payload como fuente de verdad -- ver la nota de
    // seguridad junto a TB_ORDEN_PAGO en
    // Script_Base_Datos/equipo_correcciones_y_borrador_sprint6_9.sql.
    public class ReqConfirmarPago
    {
        public long IdLinkTilopay { get; set; }
    }
}
