using System.Net;
using System.Text.Json;
using ClienteHTTPObligatorio.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClienteHTTPObligatorio.Controllers;

public class AuditoriaController : Controller
{
    private readonly HttpClient _httpClient;

    public AuditoriaController(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://webapioblip3.azurewebsites.net/api/");
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerAuditoriaPorTipoGasto(int? idTipoGasto)
    {
        // mostrar la vista inicialmente sin datos
        if (idTipoGasto == null)
            return View();

        string token = HttpContext.Session.GetString("Token");

        if (string.IsNullOrEmpty(token))
        {
            ViewBag.msg = "Debe iniciar sesión.";
            return View();
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync($"Auditorias/tipogasto/{idTipoGasto}");

        if (response.IsSuccessStatusCode)
        {
            string resultado = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var listaAuditorias = JsonSerializer.Deserialize<List<DTOAuditoria>>(resultado, options);

            if (listaAuditorias == null || !listaAuditorias.Any())
            {
                ViewBag.msg = "No hay auditorías para este tipo de gasto.";
                return View();
            }

            return View(listaAuditorias); 
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            ViewBag.msg = "No existen auditorías para ese tipo de gasto.";
            return View();
        }

        ViewBag.msg = "Error al comunicarse con la API.";
        return View();
    }
}

