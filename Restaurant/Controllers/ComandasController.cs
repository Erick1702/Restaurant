using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurant.Datos;
using Restaurant.Models;
using Restaurant.Servicios;
using Rotativa.AspNetCore;

namespace Restaurant.Controllers
{
    public class ComandasController: Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private ApplicationDbContext context;

        public ComandasController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comandas
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}, {Constantes.ROL_COCINERO}")]
        public async Task<IActionResult> Index()
        {
            var comandas = _context.Comandas
                .Include(c => c.Mesa)
                .Include(c => c.Usuario)
                .Include(c => c.TipoConsumo)
                .Include(c => c.Estado);

            return View(await comandas.ToListAsync());
        }
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]

        // GET: Comanda/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.MesaId = new SelectList(await _context.Mesas.ToListAsync(), "Id", "Numero");
            ViewBag.TipoConsumoId = new SelectList(await _context.TipoConsumos.ToListAsync(), "Id", "Nombre");
            //ViewBag.Platos = new SelectList(await _context.Platos.Where(p => p.Activo).ToListAsync(), "Id", "Nombre");
            ViewBag.Platos = new SelectList(await _context.Platos
            .Where(p => p.Activo)
            .Select(p => new { p.Id, NombrePrecio = p.Nombre + " | " + p.Precio })
            .ToListAsync(), "Id", "NombrePrecio");

            return View(new ComandaCreateViewModel());
        }

        // POST: Comanda/Create

        //public async Task<IActionResult> Create(ComandaCreateViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var estadoEmitido = await _context.Estados
        //                                   .FirstOrDefaultAsync(e => e.Nombre == "Emision" && e.Tipo == "Comanda");

        //        var comanda = new Comanda
        //        {
        //            Fecha = DateTime.Now,
        //            MesaId = model.MesaId,
        //            TipoConsumoId = model.TipoConsumoId,
        //            UsuarioId = _userManager.GetUserId(User),
        //            EstadoId = estadoEmitido.Id
        //        };
        //        //Actualizar el estado de la mesa a "Ocupada"
        //        var mesa = await _context.Mesas.FindAsync(model.MesaId);
        //        if (mesa != null)
        //        {
        //            var estadoOcupado = await _context.Estados
        //                                               .FirstOrDefaultAsync(e => e.Nombre == "Ocupado" && e.Tipo == "Mesa");

        //            if (estadoOcupado != null)
        //            {
        //                // Cambiar el estado de la mesa a "Ocupado"
        //                mesa.EstadoId = estadoOcupado.Id;
        //                _context.Mesas.Update(mesa);
        //            }
        //            else
        //            {
        //                //Mensaje si el Estado no existe
        //                ModelState.AddModelError("", "El estado 'Ocupado' no está configurado.");
        //                return View(model);
        //            }
        //        }

        //        foreach (var detalle in model.Detalles)
        //        {
        //            if (detalle.PlatoId.HasValue && detalle.Cantidad > 0)
        //            {
        //                var plato = await _context.Platos.FindAsync(detalle.PlatoId);
        //                if (plato != null)
        //                {
        //                    comanda.DetalleComandas.Add(new DetalleComanda
        //                    {
        //                        PlatoId = detalle.PlatoId,
        //                        Cantidad = detalle.Cantidad,
        //                        Subtotal = plato.Precio * detalle.Cantidad
        //                    });
        //                }
        //            }
        //        }

        //        _context.Comandas.Add(comanda);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewBag.MesaId = new SelectList(await _context.Mesas.ToListAsync(), "Id", "Numero", model.MesaId);
        //    ViewBag.TipoConsumoId = new SelectList(await _context.TipoConsumos.ToListAsync(), "Id", "Nombre", model.TipoConsumoId);
        //    ViewBag.Platos = new SelectList(await _context.Platos.Where(p => p.Activo).ToListAsync(), "Id", "Nombre");

        //    return View(model);
        //}

        [HttpPost]
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComandaCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombos(model);
                return View(model);
            }

            var estadoComanda = await ObtenerEstado("Emision", "Comanda");
            if (estadoComanda == null)
            {
                ModelState.AddModelError("", "El estado 'Emisión' no está configurado.");
                await CargarCombos(model);
                return View(model);
            }

            var comanda = await CrearComandaDesdeModelo(model, estadoComanda.Id);

            var estadoOcupado = await ObtenerEstado("Ocupado", "Mesa");
            if (!await ActualizarEstadoMesa(model.MesaId, estadoOcupado?.Id))
            {
                ModelState.AddModelError("", "El estado 'Ocupado' no está configurado.");
                await CargarCombos(model);
                return View(model);
            }

            _context.Comandas.Add(comanda);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<Estado> ObtenerEstado(string nombre, string tipo)
        {
            return await _context.Estados
                .FirstOrDefaultAsync(e => e.Nombre == nombre && e.Tipo == tipo);
        }

        private async Task<Comanda> CrearComandaDesdeModelo(ComandaCreateViewModel model, int estadoId)
        {
            var comanda = new Comanda
            {
                Fecha = DateTime.Now,
                MesaId = model.MesaId,
                TipoConsumoId = model.TipoConsumoId,
                UsuarioId = _userManager.GetUserId(User),
                EstadoId = estadoId,
                DetalleComandas = new List<DetalleComanda>()
            };

            foreach (var detalle in model.Detalles)
            {
                if (detalle.PlatoId.HasValue && detalle.Cantidad > 0)
                {
                    var plato = await _context.Platos.FindAsync(detalle.PlatoId);
                    if (plato != null)
                    {
                        comanda.DetalleComandas.Add(new DetalleComanda
                        {
                            PlatoId = detalle.PlatoId.Value,
                            Cantidad = detalle.Cantidad,
                            Subtotal = plato.Precio * detalle.Cantidad
                        });
                    }
                }
            }

            return comanda;
        }

        private async Task<bool> ActualizarEstadoMesa(int? mesaId, int? nuevoEstadoId)
        {
            if (!nuevoEstadoId.HasValue)
                return false;

            var mesa = await _context.Mesas.FindAsync(mesaId);
            if (mesa == null)
                return false;

            mesa.EstadoId = nuevoEstadoId.Value;
            _context.Mesas.Update(mesa);
            return true;
        }

        private async Task CargarCombos(ComandaCreateViewModel model)
        {
            ViewBag.MesaId = new SelectList(await _context.Mesas.ToListAsync(), "Id", "Numero", model.MesaId);
            ViewBag.TipoConsumoId = new SelectList(await _context.TipoConsumos.ToListAsync(), "Id", "Nombre", model.TipoConsumoId);
            ViewBag.Platos = new SelectList(await _context.Platos.Where(p => p.Activo).ToListAsync(), "Id", "Nombre");
        }



        //Get : Comanda/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var comanda = await _context.Comandas
                 .Include(c => c.Mesa)
                 .Include(c => c.TipoConsumo)
                .Include(c => c.DetalleComandas)
                .ThenInclude(d => d.Plato)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comanda == null)
            {
                return NotFound();
            }
            //Edicion de campos
            var viewModel = new ComandaEditViewModel
            {
                Id = comanda.Id,
                MesaId = comanda.MesaId,
                TipoConsumoId = comanda.TipoConsumoId,
                Detalles = comanda.DetalleComandas.Select(d => new DetalleEditItemViewModel
                {
                    PlatoId = d.PlatoId,
                    Cantidad = d.Cantidad,
                    Precio = d.Plato.Precio
                }).ToList()
            };

            ViewBag.MesaId = new SelectList(_context.Mesas, "Id", "Numero", viewModel.MesaId);
            ViewBag.TipoConsumoId = new SelectList(_context.TipoConsumos, "Id", "Nombre", viewModel.TipoConsumoId);
            ViewBag.Platos = new SelectList(
                _context.Platos.Select(p => new {
                    p.Id,
                    Texto = p.Nombre + "|" + p.Precio
                }), "Id", "Texto");

            return View(viewModel);
        }
        // POST: Comanda/Edit/5
        [HttpPost]
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ComandaEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.MesaId = new SelectList(_context.Mesas, "Id", "Numero", model.MesaId);
                ViewBag.TipoConsumoId = new SelectList(_context.TipoConsumos, "Id", "Nombre", model.TipoConsumoId);
                ViewBag.Platos = new SelectList(
                    _context.Platos.Select(p => new {
                        p.Id,
                        Texto = p.Nombre + "|" + p.Precio
                    }), "Id", "Texto");
                return View(model);
            }

            var comanda = await _context.Comandas
                .Include(c => c.DetalleComandas)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comanda == null)
            {
                return NotFound();
            }

            // Actualizar datos principales
            comanda.MesaId = model.MesaId;
            comanda.TipoConsumoId = model.TipoConsumoId;

            // Eliminar detalles actuales
            _context.DetalleComandas.RemoveRange(comanda.DetalleComandas);

            // Agregar los nuevos detalles
            comanda.DetalleComandas = model.Detalles.Select(d => new DetalleComanda
            {
                PlatoId = d.PlatoId,
                Cantidad = d.Cantidad
            }).ToList();

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Comanda/Delete/5
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var comanda = await _context.Comandas
                .Include(c => c.Mesa)
                .Include(c => c.Usuario)
                .Include(c => c.TipoConsumo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (comanda == null) return NotFound();

            return View(comanda);
        }

        // POST: Comanda/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comanda = await _context.Comandas.FindAsync(id);
            if (comanda != null)
            {
                _context.Comandas.Remove(comanda);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        //Valida si existe id comanda
        private bool ComandaExists(int id)
        {
            return _context.Comandas.Any(e => e.Id == id);
        }

        //Detalle de Comanda
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO},{Constantes.ROL_COCINERO}")]
        public async Task<IActionResult> Details(int id)
        {
            var comanda = await _context.Comandas
                .Include(c => c.Mesa)
                .Include(c => c.TipoConsumo)
                .Include(c => c.Usuario)
                .Include(c => c.DetalleComandas)
                    .ThenInclude(d => d.Plato)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comanda == null)
            {
                return NotFound();
            }
            //añadir detalle de comonadaDetalle
            var detalles = comanda.DetalleComandas.Select(dc => new DetalleItemViewModel
            {
                Plato = dc.Plato?.Nombre ?? "",
                Precio = dc.Plato?.Precio ?? 0,
                Cantidad = dc.Cantidad,
                Subtotal = (dc.Plato?.Precio ?? 0) * dc.Cantidad
            }).ToList();
            //Añadir comanda
            var viewModel = new ComandaDetalleViewModel
            {
                ComandaId = comanda.Id,
                Fecha = comanda.Fecha,
                Mesa = comanda.Mesa?.Numero,
                Usuario = comanda.Usuario?.UserName,
                TipoConsumo = comanda.TipoConsumo?.Nombre,
                Detalles = detalles
                // Total no se asigna directamente
            };

            return View(viewModel);
        }
        
        //Generacion de PDF de Comanda
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
        public IActionResult GeneratePdf(int id)
        {
            var comanda = _context.Comandas
                .Include(c => c.Mesa)
                .Include(c => c.DetalleComandas)
                    .ThenInclude(d => d.Plato)
                .FirstOrDefault(c => c.Id == id);

            if (comanda == null)
            {
                return NotFound();
            }
            //Datos de comanda y comandaDetalle
            var viewModel = new ComandaDetalleViewModel
            {
                ComandaId = comanda.Id,
                Mesa = comanda.Mesa?.Numero,
                Fecha = comanda.Fecha,
                Detalles = comanda.DetalleComandas.Select(d => new DetalleItemViewModel
                {
                    Plato = d.Plato?.Nombre,
                    Precio = d.Plato?.Precio ?? 0,
                    Cantidad = d.Cantidad,
                    Subtotal = (d.Plato.Precio ) * d.Cantidad
                }).ToList()
            };

            return new ViewAsPdf("Details", viewModel)
            {
                FileName = $"Comanda_{id}.pdf"
            };
        }

        //Cambia el Estado de los Objetos
        [HttpPost]
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_COCINERO}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, string estado)
        {
            var comanda = await _context.Comandas.FindAsync(id);
            if (comanda == null)
            {
                return NotFound();
            }

            var nuevoEstado = await _context.Estados
                .FirstOrDefaultAsync(e => e.Nombre == estado && e.Tipo == "Comanda");

            if (nuevoEstado == null)
            {
                return BadRequest("Estado no válido.");
            }
            comanda.EstadoId = nuevoEstado.Id;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
