using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurant.Datos;
using Restaurant.Models;
using Restaurant.Servicios;
using Rotativa.AspNetCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Restaurant.Controllers
{
    [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_CAJERO}")]
    public class FacturasController:Controller
    {
        private readonly ApplicationDbContext _context;

        public FacturasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Facturas
        public async Task<IActionResult> Index()
        {
            var facturas = _context.Facturas.Include(f => f.Comanda);
            return View(await facturas.ToListAsync());
        }

        // GET: Facturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var factura = await _context.Facturas
                .Include(f => f.Comanda)
                 .ThenInclude(c => c.DetalleComandas)
                .ThenInclude(dc => dc.Plato)
                .FirstOrDefaultAsync(m => m.Id == id);
                

            if (factura == null)
                return NotFound();

            return View(factura);
        }

        // GET: Facturas/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.ComandaId = new SelectList(_context.Comandas.Include(c => c.Mesa), "Id", "Id");


            ViewBag.ComandaId = new SelectList(await _context.Comandas.ToListAsync(), "Id", "Id");
            return View();
        }

        


        // POST: Facturas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Factura factura)
        {
            if (ModelState.IsValid)
            {
                factura.Fecha = DateTime.Now;

                // Obtener la comanda para calcular el total
                var comanda = await _context.Comandas
                    .Include(c => c.DetalleComandas)
                    .FirstOrDefaultAsync(c => c.Id == factura.ComandaId);

                if (comanda == null)
                {
                    ModelState.AddModelError("", "La comanda seleccionada no existe.");
                    ViewBag.ComandaId = new SelectList(await _context.Comandas.ToListAsync(), "Id", "Id", factura.ComandaId);
                    return View(factura);
                }

                factura.Total = comanda.DetalleComandas.Sum(d => d.Subtotal);

                _context.Facturas.Add(factura);

                // Buscar la Comanda relacionada
                 comanda = await _context.Comandas
                    .Include(c => c.Mesa) // También cargamos la Mesa
                    .FirstOrDefaultAsync(c => c.Id == factura.ComandaId);

                if (comanda != null)
                {
                    // Buscar el Estado "Cerrado" para Comanda
                    var estadoCerrado = await _context.Estados
                        .FirstOrDefaultAsync(e => e.Nombre == "Cerrado" && e.Tipo == "Comanda");

                    if (estadoCerrado != null)
                    {
                        comanda.EstadoId = estadoCerrado.Id;
                    }

                    // Buscar el Estado "Libre" para Mesa
                    var estadoLibre = await _context.Estados
                        .FirstOrDefaultAsync(e => e.Nombre == "Libre" && e.Tipo == "Mesa");

                    if (estadoLibre != null && comanda.Mesa != null)
                    {
                        comanda.Mesa.EstadoId = estadoLibre.Id;
                    }
                }



                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ComandaId = new SelectList(await _context.Comandas.ToListAsync(), "Id", "Id", factura.ComandaId);
            return View(factura);
        }

        // GET: Facturas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
                return NotFound();

            ViewBag.ComandaId = new SelectList(await _context.Comandas.ToListAsync(), "Id", "Id", factura.ComandaId);
            return View(factura);
        }

        // POST: Facturas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Factura factura)
        {
            if (id != factura.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(factura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacturaExists(factura.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ComandaId = new SelectList(await _context.Comandas.ToListAsync(), "Id", "Id", factura.ComandaId);
            return View(factura);
        }

        // GET: Facturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var factura = await _context.Facturas
                .Include(f => f.Comanda)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (factura == null)
                return NotFound();

            return View(factura);
        }

        // POST: Facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura != null)
            {
                _context.Facturas.Remove(factura);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.Id == id);
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerDetallesComanda(int comandaId)
        {
            var detalles = await _context.DetalleComandas
                .Where(d => d.ComandaId == comandaId)
                .Select(d => new
                {
                    Plato = d.Plato.Nombre,
                    Cantidad = d.Cantidad,
                    Subtotal = d.Subtotal
                })
                .ToListAsync();

            return Json(detalles);
        }

        public async Task<IActionResult> DescargarPdf(int id)
        {
            var factura = await _context.Facturas
                .Include(f => f.Comanda)
                    .ThenInclude(c => c.DetalleComandas)
                        .ThenInclude(dc => dc.Plato)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (factura == null)
            {
                return NotFound();
            }

            return new ViewAsPdf("FacturaPdf", factura)
            {
                FileName = $"Factura_{factura.Id}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }


    }
}
