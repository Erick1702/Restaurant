using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Restaurant.Controllers;
using Restaurant.Models;

using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Restaurant.Datos;

namespace Restaurant.Test
{
    [TestClass]
    public class ComandasTests
    {
        private ApplicationDbContext _context;
        private Mock<UserManager<Usuario>> _userManagerMock;
        private ComandasController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ComandasTestDb")
                .Options;

            _context = new ApplicationDbContext(options);

            // Mock de UserManager
            var userStoreMock = new Mock<IUserStore<Usuario>>();
            _userManagerMock = new Mock<UserManager<Usuario>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Setup usuario
            var usuario = new Usuario { Id = "usuario123", UserName = "test@correo.com", Email = "test@correo.com" };
            _userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(usuario.Id);

            // Insertar datos necesarios
            var estadoComanda = new Estado { Id = 1, Nombre = "Emision", Tipo = "Comanda" };
            var estadoMesa = new Estado { Id = 2, Nombre = "Ocupado", Tipo = "Mesa" };
            var mesa = new Mesa { Id = 1, Numero = "Mesa 1", EstadoId = 1 };
            var tipoConsumo = new TipoConsumo { Id = 1, Nombre = "Presencial" };
            var plato = new Plato { Id = 1, Nombre = "Arroz", Precio = 10, Activo = true };

            _context.Estados.AddRange(estadoComanda, estadoMesa);
            _context.Mesas.Add(mesa);
            _context.TipoConsumos.Add(tipoConsumo);
            _context.Platos.Add(plato);
            _context.SaveChanges();

            _controller = new ComandasController(_context, _userManagerMock.Object);

            // Simular usuario autenticado
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [TestMethod]
        public async Task Create_Post_CreaComandaYDetalle()
        {
            // Arrange
            var model = new ComandaCreateViewModel
            {
                MesaId = 1,
                TipoConsumoId = 1,
                Detalles = new List<DetalleComandaViewModel>
            {
                new DetalleComandaViewModel
                {
                    PlatoId = 1,
                    Cantidad = 2
                }
            }
            };

            // Act
            var result = await _controller.Create(model) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual(1, _context.Comandas.Count());
            Assert.AreEqual(1, _context.DetalleComandas.Count());
        }

        [TestMethod]
        public async Task Edit_Post_ActualizaComanda()
        {
            // Arrange: Crear una comanda
            var comanda = new Comanda
            {
                MesaId = 1,
                TipoConsumoId = 1,
                UsuarioId = "usuario123",
                Fecha = DateTime.Now,
                EstadoId = 1
            };
            _context.Comandas.Add(comanda);
            await _context.SaveChangesAsync();

            // Modificar datos
            comanda.MesaId = 1;
            comanda.TipoConsumoId = 1;

            // Act
            var result = await _controller.Edit(comanda.Id, comanda) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual(1, _context.Comandas.Count());
        }
    }
}

