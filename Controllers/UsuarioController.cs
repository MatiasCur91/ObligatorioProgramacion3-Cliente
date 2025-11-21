using ClienteHTTPObligatorio.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ClienteHTTPObligatorio.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly HttpClient _httpClient;

        public UsuarioController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7122/api/");

        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string? email)
        {
            if (email == null)
                return View();

            string token = HttpContext.Session.GetString("Token");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PutAsync($"Usuario/reset-password/{email}", new StringContent(""));



            if (response.IsSuccessStatusCode)
            {
                string resultado = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var mensaje = JsonSerializer.Deserialize<ResetPasswordResponse>(resultado, options);
                ViewBag.msg = $"{mensaje.Mensaje} Nueva Password: {mensaje.NuevaPassword}";
                  
                return View();
            }

            ViewBag.msg = "Error al ingresar el email.";
            return View();
        }

    }
}
