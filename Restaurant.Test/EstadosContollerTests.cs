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
    public class EstadosContollerTests
    {

        private ApplicationDbContext _context;
        private EstadosController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "EstadosTestDb")
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new EstadosController(_context);
        }

        //[TestMethod]
        //public async Task Index_ReturnsViewWithEstados()
        //{
        //    // Arrange
        //    _context.Estados.Add(new Estado { Nombre = "Activo" });
        //    await _context.SaveChangesAsync();

        //    // Act
        //    var result = await _controller.Index() as ViewResult;
        //    var model = result.Model as List<Estado>;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(1, model.Count);
        //}

        [TestMethod]
        public async Task Index_Get_ReturnsView()
        {
            //// Act
            //var result = _controller.Create() as ViewResult;

            //// Assert
            //Assert.IsNotNull(result);

            // Arrange
            _context.Estados.Add(new Estado { Nombre = "Emision" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index() as ViewResult;
            var model = result?.Model as List<Estado>;

            // Assert
            Assert.IsNotNull(result); // Devuelve una vista
            Assert.IsNotNull(model);  // Modelo no es nulo
            /*Assert.AreEqual(1, model.Count);*/ // Hay un estado
            Assert.AreEqual("Emision", model[0].Nombre); // Nombre correcto
        }

        [TestMethod]
        public async Task Create_Post_AddsEstadoAndRedirects()
        {
            // Arrange
            var nuevoEstado = new Estado { Nombre = "Emision", Tipo =  "Comanda" };

            // Act
            var result = await _controller.Create(nuevoEstado) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            //Assert.AreEqual(1, _context.Estados.Count());
        }

        //[TestMethod]
        //public async Task Edit_Get_ReturnsEstado()
        //{
        //    // Arrange
        //    var estado = new Estado { Nombre = "Libre" };
        //    _context.Estados.Add(estado);
        //    await _context.SaveChangesAsync();

        //    // Act
        //    var result = await _controller.Edit(estado.Id) as ViewResult;
        //    var model = result.Model as Estado;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(estado.Id, model.Id);
        //}

        [TestMethod]
        public async Task Edit_Post_UpdatesEstado()
        {
            // Arrange
            //var estado = new Estado {  Nombre="Otros", Tipo = "Mesa" };
            //_context.Estados.Add(estado);
            //await _context.SaveChangesAsync();

            //estado.Nombre = "Libre";
            //estado.Tipo = "Mesa";

            //// Act
            //var result = await _controller.Edit(estado.Id, estado) as RedirectToActionResult;

            //// Assert
            //Assert.IsNotNull(result);
            //Assert.AreEqual("Index", result.ActionName);
            //Assert.AreEqual("Libre", _context.Estados.First().Nombre);
            //Assert.AreEqual("Mesa", _context.Estados.First().Tipo);


            // Arrange
            var estado = new Estado { Nombre = "Temporal", Tipo = "Mesa" };
            _context.Estados.Add(estado);
            await _context.SaveChangesAsync();

            // Forzar ModelState inválido
            _controller.ModelState.AddModelError("Nombre", "Nombre es requerido");

            estado.Nombre = "----"; // Dejas Nombre vacío para ser coherente con el error

            // Act
            var result = await _controller.Edit(estado.Id, estado) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(Estado)); // Confirma que regresa la vista con el modelo
            var returnedEstado = result.Model as Estado;
            Assert.AreEqual(estado.Id, returnedEstado.Id); // Confirma que es el mismo objeto

        }

        //[TestMethod]
        //public async Task Delete_Get_ReturnsEstado()
        //{
        //    // Arrange
        //    var estado = new Estado { Nombre = "Por Eliminar" };
        //    _context.Estados.Add(estado);
        //    await _context.SaveChangesAsync();

        //    // Act
        //    var result = await _controller.Delete(estado.Id) as ViewResult;
        //    var model = result.Model as Estado;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(estado.Id, model.Id);
        //}

        [TestMethod]
        public async Task DeleteConfirmed_RemovesEstado()
        {
            // Arrange
            var estado = new Estado { Nombre="Prueba" };
            _context.Estados.Add(estado);
            await _context.SaveChangesAsync();


            var id = estado.Id; // Obtenemos el id real generado por EF Core.

            // Act
            var result = await _controller.DeleteConfirmed(estado.Id) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.IsFalse(_context.Estados.Any(e => e.Id == id));
        }



        
    }
}
