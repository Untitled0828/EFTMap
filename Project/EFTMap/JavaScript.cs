using Microsoft.Web.WebView2.WinForms;

using System.Diagnostics;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
namespace EFTMap
{
    internal static partial class JavaScript
    {
        private static async void ExecuteScriptAsyncSafe(this WebView2 browser, string script)
        {
            if (browser == null || browser.IsDisposed) return;

            if (browser.InvokeRequired)
            {
                browser.Invoke(new Action(() =>
                {
                    _ = browser.ExecuteScriptAsync(script);
                }));
            }
            else
            {
                await browser.ExecuteScriptAsync(script);
            }
        }

        public static double GetYawFromQuaternion(double x, double y, double z, double w)
        {
            double siny_cosp = 2 * (w * y + x * z);
            double cosy_cosp = 1 - 2 * (y * y + z * z);
            double yaw = Math.Atan2(siny_cosp, cosy_cosp);
            return yaw * (180.0 / Math.PI);
        }

        public static async void SiteScale(this WebView2 browser, float scale)
        {
            await browser.ExecuteScriptAsync($"document.body.style.transform = \"scale({scale})\";\r\ndocument.body.style.transformOrigin = \"0 0\";");
        }

        public static void InputEnable(this WebView2 browser)
        {
            browser.ExecuteScriptAsyncSafe(
                """
                 var input = document.querySelector('#__nuxt>div>div>div.page-content>div>div>div.panel_top>div>div.d-flex.ml-15.fs-0>input[type=text]');
                 if (!input)
                 {
                    document.querySelector('#__nuxt>div>div>div.page-content>div>div>div.panel_top>div>div.d-flex.ml-15.fs-0>button').click();
                 }
            """);
        }

        public static void HidePannels(this WebView2 browser)
        {

            browser.ExecuteScriptAsyncSafe("""
                var button = document.querySelector('#__nuxt>div>div>div.page-content>div>div>div.panel_top.d-flex>div.mr-15>button');
                var buttonText = button.innerText;
                if (buttonText === 'Hide pannels') {
                    document.querySelector("#__nuxt>div>div>div.page-content>div>div>div.panel_top.d-flex>div.mr-15>button").click();
                }
            """);
        }

        // 좌표 input에 넣는 자바스크립트
        public static string SetLocation(this WebView2 browser, string location)
        {
            string[] parts = location.Split('_');
            if (parts.Length < 3) return string.Empty;

            string[] posParts = parts[1].Split(',');
            if (posParts.Length != 3) return string.Empty;

            double x = double.Parse(posParts[0]);
            double y = double.Parse(posParts[1]);
            double z = double.Parse(posParts[2]);

            


            string[] qParts = parts[2].Split(',');
            if (qParts.Length != 4) return string.Empty;

            double qx = double.Parse(qParts[0]);
            double qy = double.Parse(qParts[1]);
            double qz = double.Parse(qParts[2]);
            double qw = double.Parse(qParts[3].Replace(" (0)", ""));

            double yaw = GetYawFromQuaternion(qx, qy, qz, qw);

            string script = string.Format("""
                var input = document.querySelector('input[type="text"]');
                if (input) {{
                    input.value = '{0}';
                    input.dispatchEvent(new Event('input'));
                }}
                var marker = document.getElementsByClassName('marker')[0];
                if (marker) {{
                    marker.style.transform = 'translate(-50%, -50%) scale(1.42857) rotate(123.45deg)';
                }}
                """, location, yaw);

            browser.ExecuteScriptAsyncSafe(script);

            return $"{x} {y} {z}";
        }

        public static void Marker(this WebView2 browser)
        {
            string script = """
                (function() {
                    const marker = document.querySelector('.marker');
                    if (!marker || marker.querySelector('.arrow')) return;

                    // 스타일 ID
                    const styleId = 'marker-arrow-style';

                    // 기존 스타일 제거
                    const oldStyle = document.getElementById(styleId);
                    if (oldStyle) oldStyle.remove();

                    // 스타일 생성 및 추가
                    const style = document.createElement('style');
                    style.id = styleId;
                    style.textContent = `
                        .marker {
                            position: absolute !important;
                            width: 24px !important;
                            height: 24px !important;
                            background: red !important;
                            border-radius: 50% !important;
                            transform: translate(-50%, -50%) scale(1.42857);
                        }
                        .marker .arrow {
                            position: absolute;
                            top: -20px;
                            left: 50%;
                            transform: translateX(-50%);
                            width: 0;
                            height: 0;
                            border-left: 9px solid transparent;
                            border-right: 9px solid transparent;
                            border-bottom: 23px solid red;
                            z-index: 10;
                        }
                    `;
                    document.head.appendChild(style);

                    // 화살표 요소 생성 및 추가
                    const arrow = document.createElement('div');
                    arrow.className = 'arrow';
                    marker.appendChild(arrow);
                })();
            """;

            browser.ExecuteScriptAsyncSafe(script);
        }

        [GeneratedRegex(@"left:\s*([^;]+);.*?top:\s*([^;]+);.*?transform:\s*([^""]+)")]
        private static partial Regex MarkerRegex();
        public static void OrderPlayer(this WebView2 browser, string html, string id)
        {
            string markerId = id;

            var match = MarkerRegex().Match(html);
            if (!match.Success) return;

            string left = match.Groups[1].Value.Trim();
            string top = match.Groups[2].Value.Trim();
            string transform = match.Groups[3].Value.Trim();

            string js = $@"
                (function() {{
                    const markerId = '{markerId}';
                    const left = '{left}';
                    const top = '{top}';
                    const transform = '{transform}';

                    const mapContainer = document.querySelector('#map');
                    if (!mapContainer) {{
                        console.warn('map container not found');
                        return;
                    }}

                    let marker = document.querySelector('.marker[marker-id=""' + markerId + '""]');
                    if (marker) {{
                        marker.style.left = left;
                        marker.style.top = top;
                        marker.style.transform = transform;
                    }} else {{
                        marker = document.createElement('div');
                        marker.className = 'marker';
                        marker.setAttribute('marker-id', markerId);
                        marker.style.left = left;
                        marker.style.top = top;
                        marker.style.transform = transform;
                        marker.style.border = '2px solid #00000033';
                        marker.style.width = '24px';
                        marker.style.height = '24px';
                        marker.style.borderRadius = '50%';
                        marker.style.position = 'absolute';
                        marker.innerHTML = '&nbsp;';
                        mapContainer.appendChild(marker);
                    }}
                }})();";

            browser.ExecuteScriptAsyncSafe(js);
        }
    }
}
