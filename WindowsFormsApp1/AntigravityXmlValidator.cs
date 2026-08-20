using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace WindowsFormsApp1
{
    public class ResultadoValidacion
    {
        public string NombreArchivo { get; set; }
        public string NombreCarpeta { get; set; }
        public string TextoXmlCrudo { get; set; }
        public string RutaXml { get; set; }
        public string Sello { get; set; }
        public string Certificado { get; set; }
        public string FragmentoError { get; set; }
        public string UUID { get; set; }
        public string FormaPago { get; set; }
        public string MetodoPago { get; set; }
        public string EsValido { get; set; }
        public string TienePdf { get; set; }
        public string Diagnostico { get; set; }
        public string DetalleError { get; set; }
        public string Moneda { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? Descuento { get; set; }
        public decimal? IVA { get; set; }              // traslado de IVA específico (el más común)
        public decimal? ImpuestosTrasladados { get; set; }
        public decimal? ImpuestosRetenidos { get; set; }
        public decimal? Total { get; set; }
        public string CriterioAplicado { get; set; }   // "INCLUIDO" / "EXCLUIDO (motivo corto)"
        public string Justificacion { get; set; }      // explicación completa, legible para auditoría
        public bool? IncluidoPorRegla { get; set; }     // null = decidido por el flujo normal; true/false = forzado por una regla explícita
        public string Serie { get; set; }
        public string Folio { get; set; }
        public string Fecha { get; set; }
        public string LugarExpedicion { get; set; }
        public string Exportacion { get; set; }
        public string CondicionesDePago { get; set; }
        public string TipoDeComprobante { get; set; }
        public string RfcEmisor { get; set; }
        public string NombreEmisor { get; set; }
        public string RegimenFiscalEmisor { get; set; }
        public string RfcReceptor { get; set; }
        public string NombreReceptor { get; set; }
        public string UsoCFDI { get; set; }
        public string ConceptosDescripcion { get; set; }
        public int NumConceptos { get; set; }
    }

    public class ReglaFiltroUUID
    {
        public string Patron { get; set; }   // subcadena a buscar dentro del UUID (ej. "199B5")
        public bool Incluir { get; set; }    // true = forzar inclusión, false = forzar exclusión
        public string Motivo { get; set; }   // justificación de negocio, se muestra en la auditoría
    }

    public class HttpUserAgentResolver : XmlUrlResolver
    {
        private readonly HttpClient _client;

        public HttpUserAgentResolver()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps)
            {
                return _client.GetStreamAsync(absoluteUri).GetAwaiter().GetResult();
            }
            return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }
    }

    public class AntigravityXmlValidator
    {
        private XslCompiledTransform _xsltCompilador;
        private const string XsltUrl = "https://www.sat.gob.mx/sitio_internet/cfd/4/cadenaoriginal_4_0/cadenaoriginal_4_0.xslt";
        private const string RutaXsltLocal = "cadenaoriginal_4_0.xslt";
        private HashSet<string> _archivosPdfExistentes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public async Task InicializarAsync(string rutaCarpeta = null)
        {
            _xsltCompilador = new XslCompiledTransform();
            var settings = new XsltSettings(true, true);
            var resolver = new HttpUserAgentResolver();

            string rutaLocalCompleta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RutaXsltLocal);
            if (File.Exists(rutaLocalCompleta))
            {
                try
                {
                    using (var reader = XmlReader.Create(rutaLocalCompleta))
                    {
                        _xsltCompilador.Load(reader, settings, resolver);
                    }
                    CargarMapeoPdfs(rutaCarpeta);
                    return;
                }
                catch { }
            }

            try
            {
                using (var stream = (Stream)resolver.GetEntity(new Uri(XsltUrl), null, typeof(Stream)))
                using (var reader = XmlReader.Create(stream, new XmlReaderSettings(), XsltUrl))
                {
                    _xsltCompilador.Load(reader, settings, resolver);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al inicializar el XSLT del SAT desde {XsltUrl}. Detalle: {ex.Message}", ex);
            }

            CargarMapeoPdfs(rutaCarpeta);
        }

        public void CargarMapeoPdfs(string rutaCarpeta)
        {
            if (!string.IsNullOrEmpty(rutaCarpeta) && Directory.Exists(rutaCarpeta))
            {
                _archivosPdfExistentes = new HashSet<string>(
                    Directory.EnumerateFiles(rutaCarpeta, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => f.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                        .Select(Path.GetFileNameWithoutExtension),
                    StringComparer.OrdinalIgnoreCase
                );
            }
        }

        public async Task<List<ResultadoValidacion>> ProcesarPilaAsync(string rutaCarpeta, IProgress<int> progreso = null)
        {
            if (!Directory.Exists(rutaCarpeta))
                throw new DirectoryNotFoundException($"La carpeta no existe: {rutaCarpeta}");

            // Cargar pdfs de todas las subcarpetas para el modo Pila
            _archivosPdfExistentes = new HashSet<string>(
                Directory.EnumerateFiles(rutaCarpeta, "*.*", SearchOption.AllDirectories)
                    .Where(f => f.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    .Select(Path.GetFileNameWithoutExtension),
                StringComparer.OrdinalIgnoreCase
            );

            var archivosXml = Directory.EnumerateFiles(rutaCarpeta, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)).ToList();

            var resultados = new List<ResultadoValidacion>();
            int procesados = 0;

            await Task.Run(() =>
            {
                foreach (var rutaXml in archivosXml)
                {
                    var res = ValidarArchivoConDiagnostico(rutaXml);
                    lock (resultados)
                    {
                        resultados.Add(res);
                    }
                    procesados++;
                    if (archivosXml.Count > 0)
                    {
                        int porcentaje = (procesados * 100) / archivosXml.Count;
                        progreso?.Report(porcentaje);
                    }
                }
            });

            return resultados;
        }

        public async Task<List<ResultadoValidacion>> ProcesarCarpetaAsync(string rutaCarpeta, IProgress<int> progreso = null)
        {
            if (!Directory.Exists(rutaCarpeta))
                throw new DirectoryNotFoundException($"La carpeta no existe: {rutaCarpeta}");

            CargarMapeoPdfs(rutaCarpeta);

            var archivosXml = Directory.EnumerateFiles(rutaCarpeta, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => f.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)).ToList();

            var resultados = new List<ResultadoValidacion>();
            int procesados = 0;

            await Task.Run(() =>
            {
                foreach (var rutaXml in archivosXml)
                {
                    var res = ValidarArchivoConDiagnostico(rutaXml);
                    lock (resultados)
                    {
                        resultados.Add(res);
                    }
                    procesados++;
                    if (archivosXml.Count > 0)
                    {
                        int porcentaje = (procesados * 100) / archivosXml.Count;
                        progreso?.Report(porcentaje);
                    }
                }
            });

            return resultados;
        }

        private void AplicarReglasFiltro(ResultadoValidacion r)
        {
            if (string.IsNullOrEmpty(r.UUID) || ReglasFiltro == null || ReglasFiltro.Count == 0)
                return;

            foreach (var regla in ReglasFiltro)
            {
                if (string.IsNullOrEmpty(regla.Patron)) continue;

                if (r.UUID.IndexOf(regla.Patron, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    r.CriterioAplicado = regla.Incluir ? "INCLUSIÓN EXPLÍCITA (regla)" : "EXCLUSIÓN EXPLÍCITA (regla)";
                    r.Justificacion = $"Regla de negocio aplicada: UUID contiene '{regla.Patron}'. {regla.Motivo}";
                    r.IncluidoPorRegla = regla.Incluir;
                    return;
                }
            }
        }

        private void GenerarJustificacionAutomatica(ResultadoValidacion r)
        {
            if (r.EsValido == "SÍ")
            {
                r.CriterioAplicado = "INCLUIDO";
                r.Justificacion = "Firma digital verificada contra el certificado del emisor; PDF correlacionado presente.";
            }
            else if (r.TienePdf != null && r.TienePdf.StartsWith("NO"))
            {
                r.CriterioAplicado = "EXCLUIDO (Inconsistencia documental)";
                r.Justificacion = "Firma válida, pero no se localizó el PDF correlacionado en la carpeta; no se considera completo para trazabilidad.";
            }
            else
            {
                r.CriterioAplicado = "EXCLUIDO (Falla de validación)";
                r.Justificacion = $"{r.Diagnostico} — {r.DetalleError}";
            }
        }

        private ResultadoValidacion ValidarArchivoConDiagnostico(string rutaXml)
        {
            ResultadoValidacion resultado;
            try
            {
                resultado = ValidarArchivoInterno(rutaXml);
            }
            catch (Exception ex)
            {
                // Cualquier excepción no prevista al procesar ESTE archivo se aísla aquí:
                // se reporta como inválido/corrupto, pero el resto del lote sigue procesándose.
                resultado = new ResultadoValidacion
                {
                    NombreArchivo = Path.GetFileName(rutaXml),
                    RutaXml = rutaXml,
                    TienePdf = "NO (Inconsistencia)",
                    EsValido = "NO",
                    Diagnostico = "🔴 ERROR INESPERADO AL PROCESAR EL ARCHIVO",
                    DetalleError = $"{ex.GetType().Name}: {ex.Message}"
                };
            }

            GenerarJustificacionAutomatica(resultado);
            AplicarReglasFiltro(resultado);
            return resultado;
        }

        private ResultadoValidacion ValidarArchivoInterno(string rutaXml)
        {
            string nombreArchivoSinExt = Path.GetFileNameWithoutExtension(rutaXml);
            string nombreArchivo = Path.GetFileName(rutaXml);

            bool tienePdf = _archivosPdfExistentes.Contains(nombreArchivoSinExt);

            var resultado = new ResultadoValidacion
            {
                NombreArchivo = nombreArchivo,
                TienePdf = tienePdf ? "SÍ" : "NO (Inconsistencia)"
            };

            // Paso 1: Leer el contenido de texto del archivo XML
            string contenidoXml = LeerContenidoTexto(rutaXml);
            if (string.IsNullOrEmpty(contenidoXml))
            {
                resultado.EsValido = "NO";
                resultado.Diagnostico = "🔴 XML VACÍO O ILEGIBLE";
                resultado.DetalleError = "El archivo está vacío o bloqueado por el sistema de archivos.";
                return resultado;
            }

            // Detectar basura al inicio (Ej: "o;?<?xml...")
            bool teniaBasuraAlInicio = false;
            int idxXmlStart = contenidoXml.IndexOf("<?xml", StringComparison.OrdinalIgnoreCase);
            if (idxXmlStart > 0)
            {
                contenidoXml = contenidoXml.Substring(idxXmlStart);
                teniaBasuraAlInicio = true;
            }
            else if (idxXmlStart < 0 && contenidoXml.TrimStart().StartsWith("<"))
            {
                int idxTagStart = contenidoXml.IndexOf('<');
                if (idxTagStart > 0)
                {
                    contenidoXml = contenidoXml.Substring(idxTagStart);
                    teniaBasuraAlInicio = true;
                }
            }

            CfdiData datos = ExtraerDatosDesdeTextoXml(contenidoXml);
            if (datos == null || (string.IsNullOrEmpty(datos.Sello) && string.IsNullOrEmpty(datos.UUID)))
            {
                resultado.EsValido = "NO";
                resultado.Diagnostico = teniaBasuraAlInicio ? "🔴 XML CORRUPTO (Texto basura al inicio del archivo o;?)" : "🔴 ESTRUCTURA NO CFDI VÁLIDA";
                resultado.DetalleError = teniaBasuraAlInicio ? "El archivo contenía caracteres no válidos antes de <?xml...>" : "No se encontró el nodo Comprobante ni TimbreFiscalDigital.";
                return resultado;
            }

            resultado.UUID = string.IsNullOrEmpty(datos.UUID) ? "SIN_UUID" : datos.UUID;
            resultado.FormaPago = string.IsNullOrEmpty(datos.FormaPago) ? "N/A" : datos.FormaPago;
            resultado.MetodoPago = string.IsNullOrEmpty(datos.MetodoPago) ? "N/A" : datos.MetodoPago;
            resultado.Moneda = string.IsNullOrEmpty(datos.Moneda) ? "N/A" : datos.Moneda;
            resultado.Subtotal = datos.SubTotal;
            resultado.Descuento = datos.Descuento;
            resultado.Total = datos.Total;
            resultado.ImpuestosTrasladados = datos.TotalImpuestosTrasladados;
            resultado.ImpuestosRetenidos = datos.TotalImpuestosRetenidos;
            resultado.IVA = datos.TrasladosPorImpuesto.TryGetValue("IVA", out decimal ivaValor) ? ivaValor : (decimal?)null;
            resultado.Serie = datos.Serie;
            resultado.Folio = datos.Folio;
            resultado.Fecha = datos.Fecha;
            resultado.LugarExpedicion = datos.LugarExpedicion;
            resultado.Exportacion = datos.Exportacion;
            resultado.CondicionesDePago = datos.CondicionesDePago;
            resultado.TipoDeComprobante = datos.TipoDeComprobante;
            resultado.RfcEmisor = datos.RfcEmisor;
            resultado.NombreEmisor = datos.NombreEmisor;
            resultado.RegimenFiscalEmisor = datos.RegimenFiscalEmisor;
            resultado.RfcReceptor = datos.RfcReceptor;
            resultado.NombreReceptor = datos.NombreReceptor;
            resultado.UsoCFDI = datos.UsoCFDI;
            resultado.ConceptosDescripcion = datos.ConceptosDescripcion;
            resultado.NumConceptos = datos.NumConceptos;

            // Paso 2: Generar cadena original y verificar firma
            string cadenaOriginal;
            try
            {
                cadenaOriginal = GenerarCadenaOriginalDesdeTextoXml(contenidoXml);
            }
            catch (Exception ex)
            {
                resultado.EsValido = "NO";
                resultado.Diagnostico = "🔴 ERROR EN ESTRUCTURA XSLT";
                resultado.DetalleError = ex.Message;
                return resultado;
            }

            string errorFirma;
            bool esValido = VerificarFirma(cadenaOriginal, datos.Sello, datos.Certificado, out errorFirma);

            if (esValido)
            {
                if (teniaBasuraAlInicio)
                {
                    resultado.EsValido = "NO (Requiere Limpieza)";
                    resultado.Diagnostico = "🟡 NO MANIPULADO (Contiene texto basura al inicio o;? pero la firma es genuina)";
                    resultado.DetalleError = "Limpiar los caracteres antes de <?xml...>";
                }
                else if (!tienePdf)
                {
                    resultado.EsValido = "NO (Inconsistencia)";
                    resultado.Diagnostico = "🟡 INCONSISTENCIA DE DOCUMENTOS (Firma 100% válida pero no se encontró el PDF correlacionado)";
                    resultado.DetalleError = "Falta el archivo PDF correlacionado en la carpeta.";
                }
                else
                {
                    resultado.EsValido = "SÍ";
                    resultado.Diagnostico = "🟢 VÁLIDO Y AUTÉNTICO";
                    resultado.DetalleError = "OK";
                }
                return resultado;
            }

            // Paso 3: Si la firma falla, probar la Auto-Reparación de Codificación en memoria (Ej: segC:n -> según)
            string contenidoSanitizado = SanitizarCodificacionTexto(contenidoXml);
            if (contenidoSanitizado != contenidoXml)
            {
                try
                {
                    string cadenaSanitizada = GenerarCadenaOriginalDesdeTextoXml(contenidoSanitizado);
                    CfdiData datosSanitizados = ExtraerDatosDesdeTextoXml(contenidoSanitizado);

                    if (VerificarFirma(cadenaSanitizada, datosSanitizados.Sello, datosSanitizados.Certificado, out _))
                    {
                        resultado.EsValido = "NO (Fallo Técnico)";
                        resultado.Diagnostico = "🟡 NO MANIPULADO (Fallo técnico por codificación corrupta segC:n en Descripción)";
                        resultado.DetalleError = "El archivo fue corrompido al guardarse con codificación no-UTF8. Los datos fiscales son genuinos.";
                        return resultado;
                    }
                }
                catch { }
            }

            // Paso 4: Si no es fallo de codificación, es MANIPULACIÓN INTENCIONAL (Fraude)
            resultado.EsValido = "NO";
            resultado.Diagnostico = "🔴 SÍ MANIPULADO (Alteración intencional de Forma de Pago / Montos / Datos Fiscales)";
            resultado.DetalleError = errorFirma;

            return resultado;
        }

        private string SanitizarCodificacionTexto(string xmlTexto)
        {
            if (string.IsNullOrEmpty(xmlTexto)) return xmlTexto;

            string resultado = xmlTexto;
            resultado = Regex.Replace(resultado, @"segC:n", "según", RegexOptions.IgnoreCase);
            resultado = Regex.Replace(resultado, @"seg\?n", "según", RegexOptions.IgnoreCase);
            resultado = Regex.Replace(resultado, @"nC:mero", "número", RegexOptions.IgnoreCase);
            resultado = Regex.Replace(resultado, @"n\?mero", "número", RegexOptions.IgnoreCase);
            resultado = Regex.Replace(resultado, @"canciC:n", "canción", RegexOptions.IgnoreCase);
            resultado = Regex.Replace(resultado, @"estaciC:n", "estación", RegexOptions.IgnoreCase);
            resultado = Regex.Replace(resultado, @"estaci\?n", "eY esto que ? stación", RegexOptions.IgnoreCase);
            resultado = Regex.Replace(resultado, @"recepciC:n", "recepción", RegexOptions.IgnoreCase);
            resultado = Regex.Replace(resultado, @"verificaciC:n", "verificación", RegexOptions.IgnoreCase);
            resultado = Regex.Replace(resultado, @"habitaciC:n", "habitación", RegexOptions.IgnoreCase);

            return resultado;
        }

        private string LeerContenidoTexto(string rutaXml)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(rutaXml);
                if (bytes.Length == 0) return "";

                Encoding encoding = DetectarCodificacion(bytes);
                string texto = encoding.GetString(bytes);

                // Si quedó un BOM como carácter dentro del string, se descarta.
                if (texto.Length > 0 && texto[0] == '\uFEFF')
                    texto = texto.Substring(1);

                return texto;
            }
            catch
            {
                return "";
            }
        }

        // Detecta la codificación real del archivo: primero por BOM explícito,
        // y si no hay BOM, por el patrón de bytes 0x00 típico de UTF-16 sin BOM
        // (causa más común del error "carácter 0x00 no válido" en CFDIs).
        private Encoding DetectarCodificacion(byte[] bytes)
        {
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                return Encoding.UTF8;
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                return Encoding.Unicode;          // UTF-16 LE
            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
                return Encoding.BigEndianUnicode; // UTF-16 BE

            int limite = Math.Min(bytes.Length, 200);
            int cerosEnParidad0 = 0;
            int cerosEnParidad1 = 0;
            for (int i = 0; i < limite; i++)
            {
                if (bytes[i] == 0x00)
                {
                    if (i % 2 == 0) cerosEnParidad0++;
                    else cerosEnParidad1++;
                }
            }

            if (cerosEnParidad1 > limite / 4)
                return Encoding.Unicode;          // patrón típico de UTF-16 LE sin BOM
            if (cerosEnParidad0 > limite / 4)
                return Encoding.BigEndianUnicode; // patrón típico de UTF-16 BE sin BOM

            return Encoding.UTF8;
        }

        private CfdiData ExtraerDatosDesdeTextoXml(string xmlTexto)
        {
            var datos = new CfdiData();
            using (var sr = new StringReader(xmlTexto))
            using (var reader = XmlReader.Create(sr))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.LocalName == "Comprobante" && (reader.NamespaceURI == "http://www.sat.gob.mx/cfd/4" || reader.NamespaceURI == "http://www.sat.gob.mx/cfd/3"))
                        {
                            datos.Version = reader.GetAttribute("Version") ?? "";
                            datos.Sello = reader.GetAttribute("Sello") ?? "";
                            datos.Certificado = reader.GetAttribute("Certificado") ?? "";
                            datos.FormaPago = reader.GetAttribute("FormaPago") ?? "";
                            datos.MetodoPago = reader.GetAttribute("MetodoPago") ?? "";
                            datos.Moneda = reader.GetAttribute("Moneda") ?? "";
                            datos.SubTotal = ParsearDecimalSeguro(reader.GetAttribute("SubTotal"));
                            datos.Descuento = ParsearDecimalSeguro(reader.GetAttribute("Descuento"));
                            datos.Total = ParsearDecimalSeguro(reader.GetAttribute("Total"));
                            datos.Serie = reader.GetAttribute("Serie") ?? "";
                            datos.Folio = reader.GetAttribute("Folio") ?? "";
                            datos.Fecha = reader.GetAttribute("Fecha") ?? "";
                            datos.LugarExpedicion = reader.GetAttribute("LugarExpedicion") ?? "";
                            datos.Exportacion = reader.GetAttribute("Exportacion") ?? "";
                            datos.CondicionesDePago = reader.GetAttribute("CondicionesDePago") ?? "";
                            datos.TipoDeComprobante = reader.GetAttribute("TipoDeComprobante") ?? "";
                        }
                        else if (reader.LocalName == "TimbreFiscalDigital" &&
                                 reader.NamespaceURI == "http://www.sat.gob.mx/TimbreFiscalDigital")
                        {
                            datos.UUID = reader.GetAttribute("UUID") ?? "";
                        }
                        else if (reader.LocalName == "Impuestos")
                        {
                            // Solo nos interesa el nodo Impuestos de nivel Comprobante (totales),
                            // no los que puedan aparecer a nivel Concepto.
                            string totTraslados = reader.GetAttribute("TotalImpuestosTrasladados");
                            string totRetenidos = reader.GetAttribute("TotalImpuestosRetenidos");

                            if (totTraslados != null)
                                datos.TotalImpuestosTrasladados = ParsearDecimalSeguro(totTraslados);
                            if (totRetenidos != null)
                                datos.TotalImpuestosRetenidos = ParsearDecimalSeguro(totRetenidos);
                        }
                        else if (reader.LocalName == "Traslado")
                        {
                            string impuesto = reader.GetAttribute("Impuesto") ?? "";
                            decimal? importe = ParsearDecimalSeguro(reader.GetAttribute("Importe"));
                            string nombreImpuesto = MapearClaveImpuesto(impuesto);

                            if (!string.IsNullOrEmpty(nombreImpuesto) && importe.HasValue)
                            {
                                if (datos.TrasladosPorImpuesto.ContainsKey(nombreImpuesto))
                                    datos.TrasladosPorImpuesto[nombreImpuesto] += importe.Value;
                                else
                                    datos.TrasladosPorImpuesto[nombreImpuesto] = importe.Value;
                            }
                        }
                        else if (reader.LocalName == "Retencion")
                        {
                            string impuesto = reader.GetAttribute("Impuesto") ?? "";
                            decimal? importe = ParsearDecimalSeguro(reader.GetAttribute("Importe"));
                            string nombreImpuesto = MapearClaveImpuesto(impuesto);

                            if (!string.IsNullOrEmpty(nombreImpuesto) && importe.HasValue)
                            {
                                if (datos.RetencionesPorImpuesto.ContainsKey(nombreImpuesto))
                                    datos.RetencionesPorImpuesto[nombreImpuesto] += importe.Value;
                                else
                                    datos.RetencionesPorImpuesto[nombreImpuesto] = importe.Value;
                            }
                        }
                        else if (reader.LocalName == "Emisor")
                        {
                            datos.RfcEmisor = reader.GetAttribute("Rfc") ?? "";
                            datos.NombreEmisor = reader.GetAttribute("Nombre") ?? "";
                            datos.RegimenFiscalEmisor = reader.GetAttribute("RegimenFiscal") ?? "";
                        }
                        else if (reader.LocalName == "Receptor")
                        {
                            datos.RfcReceptor = reader.GetAttribute("Rfc") ?? "";
                            datos.NombreReceptor = reader.GetAttribute("Nombre") ?? "";
                            datos.UsoCFDI = reader.GetAttribute("UsoCFDI") ?? "";
                            datos.RegimenFiscalReceptor = reader.GetAttribute("RegimenFiscalReceptor") ?? "";
                            datos.DomicilioFiscalReceptor = reader.GetAttribute("DomicilioFiscalReceptor") ?? "";
                        }
                        else if (reader.LocalName == "Concepto")
                        {
                            datos.NumConceptos++;
                            string desc = reader.GetAttribute("Descripcion") ?? "";
                            if (!string.IsNullOrEmpty(desc))
                                datos.ConceptosDescripcion = string.IsNullOrEmpty(datos.ConceptosDescripcion) ? desc : datos.ConceptosDescripcion + " | " + desc;
                        }
                    }
                }
            }
            return datos;
        }

        // El SAT usa claves numéricas para el catálogo c_Impuesto: 001=ISR, 002=IVA, 003=IEPS
        private string MapearClaveImpuesto(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave)) return "";

            switch (clave.Trim())
            {
                case "001": return "ISR";
                case "002": return "IVA";
                case "003": return "IEPS";
                default: return clave.Trim(); // por si ya viene como texto (CFDI 3.3 antiguos, casos atípicos)
            }
        }

        private decimal? ParsearDecimalSeguro(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            if (decimal.TryParse(valor, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal resultado))
                return resultado;
            return null;
        }

        private string GenerarCadenaOriginalDesdeTextoXml(string xmlTexto)
        {
            if (_xsltCompilador == null)
                throw new InvalidOperationException("El transformador XSLT no ha sido inicializado.");

            using (var sr = new StringReader(xmlTexto))
            using (var reader = XmlReader.Create(sr))
            {
                var xpathDoc = new XPathDocument(reader);
                using (var sw = new StringWriter())
                {
                    _xsltCompilador.Transform(xpathDoc, null, sw);
                    return sw.ToString();
                }
            }
        }

        private bool VerificarFirma(string cadenaOriginal, string selloBase64, string certificadoBase64, out string error)
        {
            error = "";
            try
            {
                if (string.IsNullOrEmpty(cadenaOriginal))
                {
                    error = "La cadena original está vacía.";
                    return false;
                }
                if (string.IsNullOrEmpty(selloBase64))
                {
                    error = "El sello está vacío.";
                    return false;
                }
                if (string.IsNullOrEmpty(certificadoBase64))
                {
                    error = "El certificado está vacío.";
                    return false;
                }

                byte[] certBytes = Convert.FromBase64String(certificadoBase64.Trim());
                using (var cert = new X509Certificate2(certBytes))
                {
                    using (var rsa = cert.GetRSAPublicKey())
                    {
                        if (rsa == null)
                        {
                            error = "No se pudo obtener la clave pública RSA del certificado.";
                            return false;
                        }

                        byte[] selloBytes = Convert.FromBase64String(selloBase64.Trim());
                        byte[] cadenaBytes = Encoding.UTF8.GetBytes(cadenaOriginal);

                        bool valid = rsa.VerifyData(cadenaBytes, selloBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                        if (!valid)
                        {
                            error = "La firma no coincide con los datos transformados.";
                        }
                        return valid;
                    }
                }
            }
            catch (Exception ex)
            {
                error = $"Excepción: {ex.Message}";
                return false;
            }
        }

        public List<ReglaFiltroUUID> ReglasFiltro { get; set; } = new List<ReglaFiltroUUID>();
    }

    public class CfdiData
    {
        public string Version { get; set; } = "";
        public string Sello { get; set; } = "";
        public string Certificado { get; set; } = "";
        public string FormaPago { get; set; } = "";
        public string MetodoPago { get; set; } = "";
        public string UUID { get; set; } = "";
        public string Moneda { get; set; } = "";
        public decimal? SubTotal { get; set; }
        public decimal? Descuento { get; set; }
        public decimal? Total { get; set; }
        public decimal? TotalImpuestosTrasladados { get; set; }
        public decimal? TotalImpuestosRetenidos { get; set; }
        public string Serie { get; set; } = "";
        public string Folio { get; set; } = "";
        public string Fecha { get; set; } = "";
        public string LugarExpedicion { get; set; } = "";
        public string Exportacion { get; set; } = "";
        public string CondicionesDePago { get; set; } = "";
        public string TipoDeComprobante { get; set; } = "";
        public string RfcEmisor { get; set; } = "";
        public string NombreEmisor { get; set; } = "";
        public string RegimenFiscalEmisor { get; set; } = "";
        public string RfcReceptor { get; set; } = "";
        public string NombreReceptor { get; set; } = "";
        public string UsoCFDI { get; set; } = "";
        public string RegimenFiscalReceptor { get; set; } = "";
        public string DomicilioFiscalReceptor { get; set; } = "";
        public string ConceptosDescripcion { get; set; } = "";
        public int NumConceptos { get; set; } = 0;

        // Desglose detallado por tipo de impuesto (ej. "IVA", "ISR", "IEPS")
        public Dictionary<string, decimal> TrasladosPorImpuesto { get; set; } = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, decimal> RetencionesPorImpuesto { get; set; } = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
    }
}
