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
    public class TipoPlatoTest
    {
        private ApplicationDbContext _context;
        private TipoPlatosController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TipoPlatoDb")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new TipoPlatosController(_context);
        }

        [TestMethod]
        public async Task Create_Post_CreaNuevoTipoPlato()
        {
            // Arrange
            var tipoPlato = new TipoPlato { Nombre = "Entradas" };

            // Act
            var result = await _controller.Create(tipoPlato) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual(1, _context.TipoPlatos.Count());
        }

        [TestMethod]
        public async Task Edit_Post_EditaTipoPlato()
        {
            // Arrange
            var tipoPlato = new TipoPlato { Nombre = "Bebidas" };
            _context.TipoPlatos.Add(tipoPlato);
            await _context.SaveChangesAsync();

            tipoPlato.Nombre = "Bebidas con alcohol1";

            // Act
            var result = await _controller.Edit(tipoPlato.Id, tipoPlato) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            var tipoEditado = await _context.TipoPlatos.FindAsync(tipoPlato.Id);
            Assert.AreEqual("Bebidas con alcohol1", tipoEditado.Nombre);
        }
    }
}
