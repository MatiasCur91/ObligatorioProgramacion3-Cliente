using System.Net;
using System.Text.Json;
using ClienteHTTPObligatorio.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClienteHTTPObligatorio.Controllers;

public class EquipoController : Controller
{
    private readonly HttpClient _httpClient;

    public EquipoController(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5268/api/");
    }

    [HttpGet]
    public async Task<IActionResult> EquiposPorMonto(double? monto)
    {
        if (monto == null)
            return View();

        // Validación de login
        string token = HttpContext.Session.GetString("Token");

        if (string.IsNullOrEmpty(token))
        {
            ViewBag.msg = "Debe iniciar sesión.";
            return View();
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync($"Equipo/{monto}");

        if (response.IsSuccessStatusCode)
        {
            string resultado = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var listaEquipos = JsonSerializer.Deserialize<List<DTOEquipo>>(resultado, options);

            if (listaEquipos == null || !listaEquipos.Any())
            {
                ViewBag.msg = "No hay equipos con pagos únicos mayores a ese monto.";
                return View();
            }


            return View(listaEquipos);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            ViewBag.msg = "No existen equipos con pagos únicos mayores a ese monto.";
            return View();
        }

        ViewBag.msg = "Error al comunicarse con la API.";
        return View();
    }
}

