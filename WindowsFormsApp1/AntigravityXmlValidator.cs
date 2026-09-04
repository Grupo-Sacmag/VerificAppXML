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

            // Cargar pdfs y xmls de todas las subcarpetas de manera segura y recursiva
            ObtenerArchivosRecursivosSeguros(rutaCarpeta, out var archivosPdf, out var archivosXml);

            _archivosPdfExistentes = new HashSet<string>(
                archivosPdf.Select(Path.GetFileNameWithoutExtension),
                StringComparer.OrdinalIgnoreCase
            );

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

        private static void ObtenerArchivosRecursivosSeguros(string rutaRaiz, out List<string> archivosPdf, out List<string> archivosXml)
        {
            archivosPdf = new List<string>();
            archivosXml = new List<string>();

            if (string.IsNullOrEmpty(rutaRaiz) || !Directory.Exists(rutaRaiz))
                return;

            var pila = new Stack<string>();
            pila.Push(rutaRaiz);

            while (pila.Count > 0)
            {
                string dirActual = pila.Pop();

                // 1. Obtener archivos de la carpeta actual con protección
                try
                {
                    var archivos = Directory.GetFiles(dirActual, "*.*", SearchOption.TopDirectoryOnly);
                    foreach (var arch in archivos)
                    {
                        if (arch.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                            archivosXml.Add(arch);
                        else if (arch.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                            archivosPdf.Add(arch);
                    }
                }
                catch { }

                // 2. Encolar subcarpetas para explorarlas una a una con protección
                try
                {
                    var subdirs = Directory.GetDirectories(dirActual);
                    foreach (var s in subdirs)
                    {
                        pila.Push(s);
                    }
                }
                catch { }
            }
        }

        private ResultadoValidacion ValidarArchivoConDiagnostico(string rutaXml)
        {
            string nombreArchivoSinExt = Path.GetFileNameWithoutExtension(rutaXml);
            string nombreArchivo = Path.GetFileName(rutaXml);
            string nombreCarpeta = Path.GetFileName(Path.GetDirectoryName(rutaXml)) ?? "";

            bool tienePdf = _archivosPdfExistentes.Contains(nombreArchivoSinExt);

            var resultado = new ResultadoValidacion
            {
                NombreArchivo = nombreArchivo,
                NombreCarpeta = nombreCarpeta,
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
                using (var fs = new FileStream(rutaXml, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.UTF8, true))
                {
                    return sr.ReadToEnd();
                }
            }
            catch
            {
                return "";
            }
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
                        if (reader.LocalName == "Comprobante" &&
                            (reader.NamespaceURI == "http://www.sat.gob.mx/cfd/4" || reader.NamespaceURI == "http://www.sat.gob.mx/cfd/3"))
                        {
                            datos.Version = reader.GetAttribute("Version") ?? "";
                            datos.Sello = reader.GetAttribute("Sello") ?? "";
                            datos.Certificado = reader.GetAttribute("Certificado") ?? "";
                            datos.FormaPago = reader.GetAttribute("FormaPago") ?? "";
                            datos.MetodoPago = reader.GetAttribute("MetodoPago") ?? "";
                        }
                        else if (reader.LocalName == "TimbreFiscalDigital" &&
                                 reader.NamespaceURI == "http://www.sat.gob.mx/TimbreFiscalDigital")
                        {
                            datos.UUID = reader.GetAttribute("UUID") ?? "";
                        }
                    }
                }
            }
            return datos;
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
    }

    public class CfdiData
    {
        public string Version { get; set; } = "";
        public string Sello { get; set; } = "";
        public string Certificado { get; set; } = "";
        public string FormaPago { get; set; } = "";
        public string MetodoPago { get; set; } = "";
        public string UUID { get; set; } = "";
    }
}
