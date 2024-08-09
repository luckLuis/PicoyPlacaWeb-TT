using Microsoft.AspNetCore.Mvc;
using PicoYPlacaApp.Services;
using PicoYPlacaApp.Models;
using System;

namespace PicoYPlacaApp.Controllers
{
    public class PicoPlacaController : Controller
    {
        private readonly PicoPlacaService _picoPlacaService;
        private readonly DatabaseService _databaseService;

        public PicoPlacaController(PicoPlacaService picoPlacaService, DatabaseService databaseService)
        {
            _picoPlacaService = picoPlacaService;
            _databaseService = databaseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Verificar(string placa, DateTime fecha, TimeSpan hora)
        {
            // Validar los campos
            if (string.IsNullOrEmpty(placa))
            {
                ViewBag.Mensaje = "Error: La placa no puede ser nula o vacía. Por favor, ingrese un número de placa válido.";
                return View("Respuesta");
            }

            if (fecha == default(DateTime))
            {
                ViewBag.Mensaje = "Error: La fecha seleccionada no es válida. Asegúrese de que la fecha esté correctamente ingresada.";
                return View("Respuesta");
            }

            if (hora == default(TimeSpan))
            {
                ViewBag.Mensaje = "Error: La hora seleccionada no es válida. Por favor, ingrese una hora correcta.";
                return View("Respuesta");
            }

            bool puedeCircular;
            try
            {
                puedeCircular = _picoPlacaService.PuedeCircular(placa, fecha, hora);
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error al verificar la placa: {ex.Message}. Por favor, inténtelo de nuevo más tarde.";
                return View("Respuesta");
            }

            var consulta = new Consulta
            {
                Placa = placa,
                Fecha = fecha,
                Hora = hora,
                Resultado = puedeCircular ? "Permitido" : "No Permitido"
            };

            try
            {
                _databaseService.GuardarConsulta(consulta);
                ViewBag.Mensaje = "Consulta guardada exitosamente en la base de datos.";
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error al guardar la consulta en la base de datos: {ex.Message}. Por favor, inténtelo de nuevo.";
            }

            try
            {
                ViewBag.Resultado = puedeCircular ?
                    $"El vehículo con placa {placa} puede circular el {fecha:dd/MM/yyyy} a las {hora:hh\\:mm}." :
                    $"El vehículo con placa {placa} NO puede circular el {fecha:dd/MM/yyyy} a las {hora:hh\\:mm}.";
            }
            catch (FormatException ex)
            {
                ViewBag.Resultado = $"Error en el formato de la fecha o la hora: {ex.Message}. Verifique los valores ingresados.";
            }

            return View("Respuesta");
        }
    }
}
