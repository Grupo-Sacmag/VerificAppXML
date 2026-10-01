using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public static class AutoActualizadorGitHub
    {
        public const string RepositorioOwner = "Grupo-Sacmag";
        public const string RepositorioNombre = "VerificAppXML";
        public const string ApiUrlReleases = "https://api.github.com/repos/Grupo-Sacmag/VerificAppXML/releases/latest";

        public class GitHubAsset
        {
            public string name { get; set; }
            public string browser_download_url { get; set; }
            public long size { get; set; }
        }

        public class GitHubRelease
        {
            public string tag_name { get; set; }
            public string name { get; set; }
            public string body { get; set; }
            public string html_url { get; set; }
            public List<GitHubAsset> assets { get; set; }
        }

        public static Version ObtenerVersionActual()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        public static Version ParsearVersionGit(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return new Version(0, 0, 0, 0);

            // Extraer números tipo "1.0.1" o "v1.2.3.4"
            var match = Regex.Match(tag, @"\d+(\.\d+)+");
            if (match.Success)
            {
                string verStr = match.Value;
                // Version requiere al menos 2 componentes (ej. 1.0) y máximo 4
                var partes = verStr.Split('.');
                if (partes.Length == 1) verStr += ".0";
                if (partes.Length == 2) verStr += ".0";
                if (Version.TryParse(verStr, out Version v))
                {
                    return v;
                }
            }
            return new Version(0, 0, 0, 0);
        }

        /// <summary>
        /// Se invoca al iniciar la aplicación en segundo plano sin interrumpir ni demorar la apertura.
        /// </summary>
        public static void VerificarAlIniciar(Form parent)
        {
            Task.Run(async () =>
            {
                try
                {
                    // Esperar 2 segundos para no competir con el arranque del formulario
                    await Task.Delay(2000);
                    await ComprobarActualizacionInternaAsync(parent, modoSilencioso: true);
                }
                catch { }
            });
        }

        /// <summary>
        /// Se invoca cuando el usuario hace clic en "Buscar Actualizaciones...".
        /// </summary>
        public static async Task VerificarManualmenteAsync(Form parent)
        {
            await ComprobarActualizacionInternaAsync(parent, modoSilencioso: false);
        }

        private static async Task ComprobarActualizacionInternaAsync(Form parent, bool modoSilencioso)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Add("User-Agent", "VerificAppXML-Updater");
                    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

                    var response = await client.GetAsync(ApiUrlReleases);

                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (!modoSilencioso)
                        {
                            parent?.Invoke((MethodInvoker)(() =>
                            {
                                MessageBox.Show(
                                    "Aún no hay ningún Release público publicado en el repositorio de GitHub.\n\nCuando publiques un Release en la rama 'main', aquí aparecerán las actualizaciones automáticamente.",
                                    "Sin Releases Publicados",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                            }));
                        }
                        return;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        if (!modoSilencioso)
                        {
                            parent?.Invoke((MethodInvoker)(() =>
                            {
                                MessageBox.Show(
                                    $"No fue posible verificar actualizaciones en GitHub (Código: {response.StatusCode}).\nVerifica tu conexión a internet.",
                                    "Verificar Actualizaciones",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                            }));
                        }
                        return;
                    }

                    string json = await response.Content.ReadAsStringAsync();
                    var serializer = new JavaScriptSerializer();
                    var release = serializer.Deserialize<GitHubRelease>(json);

                    if (release == null || string.IsNullOrWhiteSpace(release.tag_name))
                    {
                        if (!modoSilencioso)
                        {
                            parent?.Invoke((MethodInvoker)(() =>
                            {
                                MessageBox.Show("No se encontró información de versiones en GitHub.", "Verificar Actualizaciones", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }));
                        }
                        return;
                    }

                    Version versionRemota = ParsearVersionGit(release.tag_name);
                    Version versionActual = ObtenerVersionActual();

                    // Comparar versiones
                    if (versionRemota > versionActual)
                    {
                        parent?.Invoke((MethodInvoker)(() =>
                        {
                            NotificarNuevaVersion(parent, release, versionActual, versionRemota);
                        }));
                    }
                    else
                    {
                        if (!modoSilencioso)
                        {
                            parent?.Invoke((MethodInvoker)(() =>
                            {
                                MessageBox.Show(
                                    $"¡Ya tienes la versión más reciente instalada!\n\nVersión actual: v{versionActual.Major}.{versionActual.Minor}.{versionActual.Build}\nVersión en GitHub: {release.tag_name}",
                                    "Sistema Actualizado",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information
                                );
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!modoSilencioso)
                {
                    parent?.Invoke((MethodInvoker)(() =>
                    {
                        MessageBox.Show(
                            "Ocurrió un error al contactar el servidor de GitHub:\n" + ex.Message,
                            "Error de Conexión",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }));
                }
            }
        }

        private static void NotificarNuevaVersion(Form parent, GitHubRelease release, Version versionActual, Version versionRemota)
        {
            string notas = string.IsNullOrWhiteSpace(release.body) ? "Mejoras generales y corrección de errores." : release.body;
            if (notas.Length > 400) notas = notas.Substring(0, 400) + "...";

            string mensaje = $"🚀 ¡Hay una nueva versión disponible de VerificAppXML!\n\n" +
                             $"• Tu versión actual: v{versionActual.Major}.{versionActual.Minor}.{versionActual.Build}\n" +
                             $"• Nueva versión: {release.tag_name} ({release.name ?? ""})\n\n" +
                             $"Novedades:\n{notas}\n\n" +
                             $"¿Deseas descargar e instalar la actualización ahora?";

            var dr = MessageBox.Show(
                parent,
                mensaje,
                "Actualización Disponible",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dr == DialogResult.Yes)
            {
                EjecutarProcesoDescarga(parent, release);
            }
        }

        private static void EjecutarProcesoDescarga(Form parent, GitHubRelease release)
        {
            // Buscar si hay un archivo .zip en los assets del release
            var assetZip = release.assets?.FirstOrDefault(a => a.name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase));

            if (assetZip == null || string.IsNullOrWhiteSpace(assetZip.browser_download_url))
            {
                // Si no hay zip empaquetado, abrir la página de GitHub del Release para descarga manual
                var r = MessageBox.Show(
                    "Esta versión no contiene un archivo comprimido (.zip) para instalación automática directa.\n\n¿Deseas abrir la página del Release en GitHub para descargarlo manualmente?",
                    "Abrir Release en GitHub",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (r == DialogResult.Yes && !string.IsNullOrEmpty(release.html_url))
                {
                    try { Process.Start(new ProcessStartInfo(release.html_url) { UseShellExecute = true }); } catch { }
                }
                return;
            }

            // Descargar el zip y aplicar reemplazo
            Form modalProgreso = new Form
            {
                Text = "Descargando Actualización...",
                Size = new System.Drawing.Size(420, 160),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false
            };

            TemaGamerEmpresarial.AplicarTema(modalProgreso);

            Label lblMensaje = new Label
            {
                Text = $"Descargando {assetZip.name}...\nPor favor espera un momento.",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(360, 40),
                Font = new System.Drawing.Font("Segoe UI", 9F)
            };
            modalProgreso.Controls.Add(lblMensaje);

            ProgressBar pb = new ProgressBar
            {
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(360, 24),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30
            };
            modalProgreso.Controls.Add(pb);

            string rutaTempZip = Path.Combine(Path.GetTempPath(), "VerificAppXML_NuevaVersion.zip");

            modalProgreso.Shown += async (s, ev) =>
            {
                try
                {
                    using (var wc = new WebClient())
                    {
                        wc.Headers.Add("User-Agent", "VerificAppXML-Updater");
                        await wc.DownloadFileTaskAsync(new Uri(assetZip.browser_download_url), rutaTempZip);
                    }

                    modalProgreso.Close();

                    // Aplicar el script de actualización
                    LanzarActualizadorYSalir(rutaTempZip);
                }
                catch (Exception ex)
                {
                    modalProgreso.Close();
                    MessageBox.Show("Error al descargar la actualización: " + ex.Message, "Error de Descarga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            modalProgreso.ShowDialog(parent);
        }

        private static void LanzarActualizadorYSalir(string rutaZip)
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            string exeName = Process.GetCurrentProcess().MainModule.ModuleName;
            string batPath = Path.Combine(Path.GetTempPath(), "actualizar_verificapp.bat");

            // Script .bat que:
            // 1. Espera 2 segundos a que el proceso actual termine
            // 2. Extrae el .zip sobre la carpeta de la app con PowerShell Expand-Archive
            // 3. Inicia la app actualizada
            // 4. Limpia el zip y el script bat
            string contenidoBat = $@"@echo off
timeout /t 2 /nobreak > nul
powershell -NoProfile -ExecutionPolicy Bypass -Command ""Expand-Archive -Path '{rutaZip}' -DestinationPath '{appDir}' -Force""
start """" ""{Path.Combine(appDir, exeName)}""
if exist ""{rutaZip}"" del /f /q ""{rutaZip}""
del ""%~f0""
";

            File.WriteAllText(batPath, contenidoBat, System.Text.Encoding.Default);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = batPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(psi);
            Application.Exit();
        }
    }
}
