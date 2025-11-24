using System.Net.Http.Headers;
using System.Text;
using ClienteHTTPObligatorio.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ClienteHTTPObligatorio.Controllers
{
    public class PagoController : Controller
    {
        private readonly HttpClient _httpClient;

        public PagoController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5268/api/");

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPagosPorUsuario(string? email)
        {
            if (email == null)
                return View();

            string token = HttpContext.Session.GetString("Token");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"Pago/usuario-pagos/{email}");

            if (response.IsSuccessStatusCode)
            {
                string resultado = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var listaPagos = JsonSerializer.Deserialize<List<DTOPago>>(resultado, options);
                return View(listaPagos);
            }

            ViewBag.msg = "Error al obtener los pagos.";
            return View();
        }
        
        private async Task<List<DTOTipoGasto>> ObtenerTipoGasto()
        {
            var response = await _httpClient.GetAsync("TipoGasto");

            if (!response.IsSuccessStatusCode)
                return new List<DTOTipoGasto>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<DTOTipoGasto>>(json,
                       new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<DTOTipoGasto>();
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            string token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.msg = "Debe iniciar sesión.";
                return View();
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var dto = new DTOAltaPago();
            dto.TiposGastos = await ObtenerTipoGasto();

            return View(dto);
        }


        [HttpPost]
        public async Task<IActionResult> Create(DTOAltaPago dto)
        {
            dto.TiposGastos = await ObtenerTipoGasto();

            if (!ModelState.IsValid)
            {
                ViewBag.msg = "Hay errores en el formulario.";
                return View(dto);
            }

            if (dto.TipoSeleccionado == "Unico")
            {
                if (dto.FechaPago == null || dto.NumeroRecibo == null)
                {
                    ViewBag.msg = "Debe ingresar FechaPago y NumeroRecibo para un pago Único.";
                    return View(dto);
                }
            }
            else if (dto.TipoSeleccionado == "Recurrente")
            {
                if (dto.FechaInicio == null || dto.FechaFin == null)
                {
                    ViewBag.msg = "Debe ingresar FechaInicio y FechaFin para un pago Recurrente.";
                    return View(dto);
                }
            }

            // Enviar a API
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Pago/AltaPago", content);
            
            if (response.IsSuccessStatusCode)
            {
                ViewBag.msg = "Pago registrado correctamente.";
                ModelState.Clear();
                dto = new DTOAltaPago();
                dto.TiposGastos = await ObtenerTipoGasto();
                return View(dto);
            }

            ViewBag.msg = "Error al registrar el pago.";
            return View(dto);
        }
    }
}

