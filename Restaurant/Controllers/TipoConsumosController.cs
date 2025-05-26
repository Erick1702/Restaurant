using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Datos;
using Restaurant.Models;
using Restaurant.Servicios;

namespace Restaurant.Controllers
{
    [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
    public class TipoConsumosController:Controller
    {
        private readonly ApplicationDbContext _context;

        public TipoConsumosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipoConsumo
        public async Task<IActionResult> Index()
        {
            var lista = await _context.TipoConsumos.ToListAsync();
            return View(lista);
        }

        // GET: TipoConsumo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoConsumo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoConsumo tipoConsumo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoConsumo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoConsumo);
        }

        // GET: TipoConsumo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var tipoConsumo = await _context.TipoConsumos.FindAsync(id);
            if (tipoConsumo == null)
                return NotFound();

            return View(tipoConsumo);
        }

        // POST: TipoConsumo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoConsumo tipoConsumo)
        {
            if (id != tipoConsumo.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoConsumo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoConsumoExists(tipoConsumo.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tipoConsumo);
        }

        // GET: TipoConsumo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var tipoConsumo = await _context.TipoConsumos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tipoConsumo == null)
                return NotFound();

            return View(tipoConsumo);
        }

        // POST: TipoConsumo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoConsumo = await _context.TipoConsumos.FindAsync(id);
            if (tipoConsumo != null)
            {
                _context.TipoConsumos.Remove(tipoConsumo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TipoConsumoExists(int id)
        {
            return _context.TipoConsumos.Any(e => e.Id == id);
        }

    }
}
