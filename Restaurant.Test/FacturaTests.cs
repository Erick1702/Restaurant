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
using System.Web.Mvc;

namespace Restaurant.Test
{
    [TestClass]
    public class FacturaTests
    {
        private ApplicationDbContext context;
        private FacturasController controller;
        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // base de datos única por prueba
            .Options;

            context = new ApplicationDbContext(options);
            controller = new FacturasController(context);
        }

        //    [TestMethod]
        //    public async Task Create_Post_CreatesFactura_WhenModelIsValid()
        //    {
        //        // Arrange
        //        var comanda = new Comanda { Fecha = DateTime.Now };
        //        _context.Comandas.Add(comanda);
        //        await _context.SaveChangesAsync();

        //        var factura = new Factura
        //        {
        //            Fecha = DateTime.Now,
        //            Total = 50.0m,
        //            ComandaId = comanda.Id
        //        };

        //        var controller = new FacturasController(_context);
        //        var detalles = await _context.DetalleComandas
        //.Where(d => d.ComandaId == comanda.Id)
        //.ToListAsync();
        //        Console.WriteLine($"Detalles encontrados: {detalles.Count}, Subtotal total: {detalles.Sum(d => d.Subtotal)}");

        //        // Act
        //        var result = await controller.Create(factura) as RedirectToActionResult;

        //        // Assert
        //        Assert.IsNotNull(result);
        //        Assert.AreEqual("Index", result.ActionName);
        //        Assert.AreEqual(1, _context.Facturas.Count());
        //        Assert.AreEqual(50.0m, _context.Facturas.First().Total);
        //    }

        [TestMethod]
        public async Task Create_Post()
        {
            // Crear datos necesarios
            var plato = new Plato { Nombre = "Pizza", Precio = 25 };
            context.Platos.Add(plato);

            var estadoMesa = new Estado { Nombre = "Ocupado", Tipo = "Mesa" };
            var estadoComanda = new Estado { Nombre = "Emision", Tipo = "Comanda" };
            var estadoCerrado = new Estado { Nombre = "Cerrado", Tipo = "Comanda" };
            var estadoLibre = new Estado { Nombre = "Libre", Tipo = "Mesa" };

            context.Estados.AddRange(estadoMesa, estadoComanda, estadoCerrado, estadoLibre);
            await context.SaveChangesAsync();

            var mesa = new Mesa { Numero = "M1", EstadoId = estadoMesa.Id };
            context.Mesas.Add(mesa);
            await context.SaveChangesAsync();

            var comanda = new Comanda
            {
                MesaId = mesa.Id,
                EstadoId = estadoComanda.Id,
                DetalleComandas = new List<DetalleComanda>()
            };
            context.Comandas.Add(comanda);
            await context.SaveChangesAsync();

            var detalle = new DetalleComanda
            {
                PlatoId = plato.Id,
                Cantidad = 2,
                Subtotal = 50,
                ComandaId = comanda.Id
            };
            context.DetalleComandas.Add(detalle);
            await context.SaveChangesAsync();

            // Act - Ejecutar el método Create
            var factura = new Factura
            {
                ComandaId = comanda.Id
            };

            var result = await controller.Create(factura);

            // Assert - Verificar resultado
            var redirect = result as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            var facturaGuardada = await context.Facturas.FirstOrDefaultAsync();
            Assert.IsNotNull(facturaGuardada);
            Assert.AreEqual(50, facturaGuardada.Total);

            // Verificar que se actualizó el estado de la comanda y mesa
            var comandaActualizada = await context.Comandas.Include(c => c.Mesa).FirstOrDefaultAsync(c => c.Id == comanda.Id);
            Assert.AreEqual("Cerrado", context.Estados.First(e => e.Id == comandaActualizada.EstadoId).Nombre);
            Assert.AreEqual("Libre", context.Estados.First(e => e.Id == comandaActualizada.Mesa.EstadoId).Nombre);

        }


        [TestMethod]
        public async Task Edit_Post_UpdatesFactura_WhenModelIsValid()
        {
            // Arrange
            var comanda = new Comanda { Fecha = DateTime.Now };
            context.Comandas.Add(comanda);
            var factura = new Factura { Fecha = DateTime.Now, Total = 100, ComandaId = comanda.Id };
            context.Facturas.Add(factura);
            await context.SaveChangesAsync();

            var controller = new FacturasController(context);

            // Modificamos la factura
            factura.Total = 150;

            // Act
            var result = await controller.Edit(factura.Id, factura) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual(150, context.Facturas.First().Total);
        }

    }
}
