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
            _httpClient.BaseAddress = new Uri("https://localhost:7122/api/");

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



    }
}
