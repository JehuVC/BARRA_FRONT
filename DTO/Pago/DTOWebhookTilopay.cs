namespace DTO.Pago
{
    // Body que Tilopay manda al webhook_url. Solo se lee tilopayLinkId -- el
    // resto del payload no es de fiar (Tilopay no documenta firma/HMAC), asi
    // que ni se mapea. Ver la nota de seguridad junto a TB_ORDEN_PAGO.
    public class DTOWebhookTilopay
    {
        public long tilopayLinkId { get; set; }
    }
}
