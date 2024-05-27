namespace api_001_prueba_cajanegra.Model
{
    public static class DocumentXpaths
    {
        public static readonly Dictionary<string, (string ClientIdXpath, string DocumentNameXpath)> Paths = new Dictionary<string, (string ClientIdXpath, string DocumentNameXpath)>
        {
            { "ECF", (".//Encabezado//Emisor//RNCEmisor", ".//Encabezado//IdDoc//eNCF") },
            { "ACECF", (".//DetalleAprobacionComercial//RNCEmisor", ".//DetalleAprobacionComercial//eNCF") },
            { "ANECF", (".//Encabezado//RncEmisor", null) }
        };
    }
}
