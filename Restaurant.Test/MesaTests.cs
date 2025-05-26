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
    public sealed class MesaTests
    {
        
        private ApplicationDbContext _context;
        private MesasController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Mesas")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new MesasController(_context);
        }

        [TestMethod]
        public async Task CrearMesa_DeberiaGuardarCorrectamente()
        {
            // Arrange
            var mesa = new Mesa
            {
                Numero = "5",
                Capacidad = 4,
                Descripcion = "Mesa 3",
                //Activo = false,
                EstadoId = 1
            };



            // Act
            _context.Mesas.Add(mesa);
            await _context.SaveChangesAsync();

            // Assert
            var mesaEnDb = await _context.Mesas.FirstOrDefaultAsync(m => m.Id == mesa.Id);
            Assert.IsNotNull(mesaEnDb);
            Assert.AreEqual("5", mesaEnDb.Numero);
            Assert.AreEqual(4, mesaEnDb.Capacidad);
            Assert.IsTrue(mesaEnDb.Activo);
            Assert.AreEqual(1, mesaEnDb.EstadoId);


            //// Arrange
            //var mesa = new Mesa
            //{
            //    Numero = "1",
            //    Capacidad = 4,
            //    Descripcion = "Mesa 3",
            //    Activo = true,
            //    EstadoId = 1
            //}; // Supongamos que está vacía o no cumple validaciones
            //_controller.ModelState.AddModelError("Numero", "Required");

            //// Act
            //var result = await _controller.Create(mesa);

            //// Assert
            //var viewResult = result as ViewResult;
            //Assert.IsNotNull(viewResult);
            //var model = viewResult.Model as Mesa;
            //Assert.IsNotNull(model);

        }

        [TestMethod]
        public async Task CambiarEstadoMesa_DeberiaActualizarCorrectamente()
        {
            // Arrange
            var mesa = new Mesa
            {
                Numero = "4",
                Capacidad = 2,
                Descripcion = "Mesa 1",
                Activo = true
            };
            _context.Mesas.Add(mesa);
            await _context.SaveChangesAsync();

            // Act
            mesa.Activo = false;
            
            _context.Mesas.Update(mesa);
            await _context.SaveChangesAsync();

            // Assert
            var mesaActualizada = await _context.Mesas.FirstOrDefaultAsync(m => m.Id == mesa.Id);
            Assert.IsNotNull(mesaActualizada);
            Assert.IsFalse(mesaActualizada.Activo);
        }

        [TestMethod]
        public async Task DeleteConfirmed_RemovesMesa()
        {
            // Arrange
            var mesa = new Mesa { Numero = "10", Capacidad = 4 };
            _context.Mesas.Add(mesa);
            await _context.SaveChangesAsync();

            var id = mesa.Id;
            // Act
            var result = await _controller.DeleteConfirmed(mesa.Id) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.IsFalse(_context.Mesas.Any(e => e.Id == id));
            //var mesaEnDb = await _context.Mesas.FindAsync(mesa.Id == id);
            //Assert.IsNull(mesaEnDb); // ✅ La mesa ya no debe existir
        }


        [TestMethod]
        public async Task Edit_Post_UpdatesMesa()
        {
            // Arrange
            var mesa = new Mesa { Numero = "5", Capacidad = 4 };
            _context.Mesas.Add(mesa);
            await _context.SaveChangesAsync();

            // Cambiamos los valores de la mesa
            mesa.Numero = "10";
            mesa.Capacidad = 6;

            // Act
            var result = await _controller.Edit(mesa.Id, mesa) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            var mesaActualizada = await _context.Mesas.FindAsync(mesa.Id);
            Assert.AreEqual("10", mesaActualizada.Numero);
            Assert.AreEqual(6, mesaActualizada.Capacidad);
        }

    }
}
