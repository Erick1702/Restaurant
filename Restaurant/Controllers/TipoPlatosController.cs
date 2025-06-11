using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Datos;
using Restaurant.Models;
using Restaurant.Servicios;

namespace Restaurant.Controllers
{
    [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_COCINERO}")]
    public class TipoPlatosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TipoPlatosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipoPlatos
        public async Task<IActionResult> Index()
        {
            var tiposPlato = await _context.TipoPlatos.ToListAsync();
            return View(tiposPlato);
        }

        // GET: TipoPlatos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoPlatos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoPlato tipoPlato)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoPlato);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoPlato);
        }

        // GET: TipoPlatos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tipoPlato = await _context.TipoPlatos.FindAsync(id);
            if (tipoPlato == null) return NotFound();

            return View(tipoPlato);
        }

        // POST: TipoPlatos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoPlato tipoPlato)
        {
            if (id != tipoPlato.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(tipoPlato);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoPlato);
        }

        // GET: TipoPlatos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tipoPlato = await _context.TipoPlatos.FindAsync(id);
            if (tipoPlato == null) return NotFound();

            return View(tipoPlato);
        }

        // POST: TipoPlatos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoPlato = await _context.TipoPlatos.FindAsync(id);
            _context.TipoPlatos.Remove(tipoPlato);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
