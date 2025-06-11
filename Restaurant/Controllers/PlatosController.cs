using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurant.Datos;
using Restaurant.Models;
using Restaurant.Servicios;

namespace Restaurant.Controllers
{
    public class PlatosController: Controller
    {
        private readonly ApplicationDbContext _context;       
        public PlatosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Platos
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_COCINERO}")]
        public async Task<IActionResult> Index(int? tipoPlatoId)
        {
            ViewData["TipoPlatoId"] = new SelectList(_context.TipoPlatos.Where(t => t.Activo), "Id", "Nombre");
            var platos = _context.Platos.Include(p => p.TipoPlato).AsQueryable();

            if (tipoPlatoId.HasValue)
            {
                platos = platos.Where(p => p.TipoPlatoId == tipoPlatoId.Value);
            }
            return View(await platos.ToListAsync());
        }

        // GET: Platos/Create
        public IActionResult Create()
        {
            ViewData["TipoPlatoId"] = new SelectList(_context.TipoPlatos.Where(t => t.Activo), "Id", "Nombre");
            return View();
        }

        // POST: Platos/Create
        //Registro de platos
        [HttpPost]
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_COCINERO}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Plato plato)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plato);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoPlatoId"] = new SelectList(_context.TipoPlatos.Where(t => t.Activo), "Id", "Nombre", plato.TipoPlatoId);
            return View(plato);
        }

        // GET: Platos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var plato = await _context.Platos.FindAsync(id);
            if (plato == null) return NotFound();

            ViewData["TipoPlatoId"] = new SelectList(_context.TipoPlatos.Where(t => t.Activo), "Id", "Nombre", plato.TipoPlatoId);
            return View(plato);
        }

        // POST: Platos/Edit/5
        [HttpPost]
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_COCINERO}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Plato plato)
        {
            if (id != plato.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(plato);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoPlatoId"] = new SelectList(_context.TipoPlatos.Where(t => t.Activo), "Id", "Nombre", plato.TipoPlatoId);
            return View(plato);
        }

        // GET: Platos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var plato = await _context.Platos
                .Include(p => p.TipoPlato)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (plato == null) return NotFound();

            return View(plato);
        }

        // POST: Platos/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_COCINERO}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plato = await _context.Platos.FindAsync(id);
            _context.Platos.Remove(plato);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
