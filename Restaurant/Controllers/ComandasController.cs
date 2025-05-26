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
    [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
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
        public async Task<IActionResult> Index()
        {
            var comandas = _context.Comandas
                .Include(c => c.Mesa)
                .Include(c => c.Usuario)
                .Include(c => c.TipoConsumo);

            return View(await comandas.ToListAsync());
        }

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComandaCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var estadoEmitido = await _context.Estados
                                           .FirstOrDefaultAsync(e => e.Nombre == "Emision" && e.Tipo == "Comanda");

                //if (estadoEmitido == null)
                //{
                //    //Error por si no existe el estado "Emitido" de tipo "Comanda"
                //    ModelState.AddModelError("", "El estado 'Emitido' no está configurado.");
                //    return View(model); // O cualquier acción que quieras tomar
                //}

                var comanda = new Comanda
                {
                    Fecha = DateTime.Now,
                    MesaId = model.MesaId,
                    TipoConsumoId = model.TipoConsumoId,
                    UsuarioId = _userManager.GetUserId(User),
                    EstadoId = estadoEmitido.Id
                };
                //Actualizar el estado de la mesa a "Ocupada"
                var mesa = await _context.Mesas.FindAsync(model.MesaId);
                if (mesa != null)
                {
                    var estadoOcupado = await _context.Estados
                                                       .FirstOrDefaultAsync(e => e.Nombre == "Ocupado" && e.Tipo=="Mesa");

                    if (estadoOcupado != null)
                    {
                        mesa.EstadoId = estadoOcupado.Id; // Cambiar el estado de la mesa a "Ocupado"
                        _context.Mesas.Update(mesa);
                    }
                    else
                    {
                        ModelState.AddModelError("", "El estado 'Ocupado' no está configurado.");
                        return View(model);
                    }
                }

                /////

                foreach (var detalle in model.Detalles)
                {
                    if (detalle.PlatoId.HasValue && detalle.Cantidad > 0)
                    {
                        var plato = await _context.Platos.FindAsync(detalle.PlatoId);
                        if (plato != null)
                        {
                            comanda.DetalleComandas.Add(new DetalleComanda
                            {
                                PlatoId = detalle.PlatoId,
                                Cantidad = detalle.Cantidad,
                                Subtotal = plato.Precio * detalle.Cantidad
                            });
                        }
                    }
                }

                _context.Comandas.Add(comanda);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MesaId = new SelectList(await _context.Mesas.ToListAsync(), "Id", "Numero", model.MesaId);
            ViewBag.TipoConsumoId = new SelectList(await _context.TipoConsumos.ToListAsync(), "Id", "Nombre", model.TipoConsumoId);
            ViewBag.Platos = new SelectList(await _context.Platos.Where(p => p.Activo).ToListAsync(), "Id", "Nombre");

            return View(model);
        }

        // POST: Comanda/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Comanda comanda)
        {
            if (id != comanda.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comanda);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComandaExists(comanda.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MesaId"] = new SelectList(_context.Mesas, "Id", "NombreMesa", comanda.MesaId);
            ViewData["TipoConsumoId"] = new SelectList(_context.TipoConsumos, "Id", "Nombre", comanda.TipoConsumoId);

            return View(comanda);
        }

        // GET: Comanda/Delete/5
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

        private bool ComandaExists(int id)
        {
            return _context.Comandas.Any(e => e.Id == id);
        }


        //GET: Comanda/Details/5

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

            var viewModel = new ComandaDetalleViewModel
            {
                ComandaId = comanda.Id,
                Fecha = comanda.Fecha,
                Mesa = comanda.Mesa?.Numero,
                Usuario = comanda.Usuario?.UserName,
                TipoConsumo = comanda.TipoConsumo?.Nombre,
                Detalles = comanda.DetalleComandas.Select(dc => new DetalleItemViewModel
                {
                    Plato = dc.Plato?.Nombre ?? "",
                    Precio = dc.Plato?.Precio ?? 0,
                    Cantidad = dc.Cantidad
                }).ToList()
            };

            return View(viewModel);
        }


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

            var viewModel = new ComandaDetalleViewModel
            {
                ComandaId = comanda.Id,
                Mesa = comanda.Mesa?.Numero,
                Fecha = comanda.Fecha,
                Detalles = comanda.DetalleComandas.Select(d => new DetalleItemViewModel
                {
                    Plato = d.Plato?.Nombre,
                    Cantidad = d.Cantidad,
                    Subtotal = (d.Plato.Precio ) * d.Cantidad
                }).ToList()
            };

            return new ViewAsPdf("Details", viewModel)
            {
                FileName = $"Comanda_{id}.pdf"
            };
        }

    }
}
