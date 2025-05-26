using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Controllers;
using Restaurant.Datos;
using Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Restaurant.Test
{
    [TestClass]
    public class PlatoTests
    {

        private ApplicationDbContext _context;
        private PlatosController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "PlatoDBTest") // DB única por prueba
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new PlatosController(_context);
        }

        
        [TestMethod]
        public async Task Create_Post_ValidData_CreatesPlato()
        {
            // Arrange
            var plato = new Plato
            {
                Nombre = "Lomo Saltado",
                Precio = 18.00m
            };

            // Act
            var result = await _controller.Create(plato) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var creado = await _context.Platos.FirstOrDefaultAsync();
            Assert.IsNotNull(creado);
            Assert.AreEqual("Lomo Saltado", creado.Nombre);
        }



        [TestMethod]
        public async Task Edit_Post_UpdatesPlato()
        {
            // Arrange
            var plato = new Plato
            {
                Nombre = "Tallarin Verde",
                Precio = 12.00m,
                Activo = true
            };
            _context.Platos.Add(plato);
            await _context.SaveChangesAsync();

            // Modificar plato
            plato.Nombre = "Tallarin Verde con Bistec";
            plato.Precio = 16.00m;

            // Act
            var result = await _controller.Edit(plato.Id, plato) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var actualizado = await _context.Platos.FindAsync(plato.Id);
            Assert.AreEqual("Tallarin Verde con Bistec", actualizado.Nombre);
            Assert.AreEqual(16.00m, actualizado.Precio);
        }

        [TestMethod]
        public async Task Create_Post_SinNombre()
        {
            // Arrange
            var plato = new Plato
            {
                Precio = 10.00m // Falta Nombre
            };

            _controller.ModelState.AddModelError("Nombre", "El campo Nombre es obligatorio");

            // Act
            var result = await _controller.Create(plato) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(plato, result.Model);
            Assert.IsFalse(_controller.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Edit_Post_SinPrecio()
        {
            var plato = new Plato
            {
                Nombre = "Chaufa",
                Precio = 9.00m
            };
            _context.Platos.Add(plato);
            await _context.SaveChangesAsync();

            plato.Precio = 0; // Suponiendo que 0 es inválido
            _controller.ModelState.AddModelError("Precio", "El campo Precio es obligatorio");

            var result = await _controller.Edit(plato.Id, plato) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(plato, result.Model);
            Assert.IsFalse(_controller.ModelState.IsValid);
        }


    }
}
