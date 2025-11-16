using ClienteHTTPObligatorio.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ClienteHTTPObligatorio.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuthController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7122/api/");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(DTOLogin dto)
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                string resultado = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; 
                
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(resultado, options);

                HttpContext.Session.SetString("Token", loginResponse.Token);
                HttpContext.Session.SetString("Rol", loginResponse.Rol);
                HttpContext.Session.SetString("Nombre", loginResponse.Rol);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.msg = "Error en los datos ingresados";
                return View();
            }

               
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
