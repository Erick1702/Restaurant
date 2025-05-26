using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Datos;
using Restaurant.Models;
using Restaurant.Servicios;

namespace Restaurant.Controllers
{
    public class EstadosController: Controller
    {

        private readonly ApplicationDbContext _context;

        public EstadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Estados
        [Authorize(Roles = Constantes.ROL_ADMINISTRADOR)]
        public async Task<IActionResult> Index()
        {
            var estados = await _context.Estados.ToListAsync();
            return View(estados);
        }

        // GET: Estados/Create
        [Authorize(Roles = Constantes.ROL_ADMINISTRADOR)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Estados/Create
        [HttpPost]
        [Authorize(Roles = Constantes.ROL_ADMINISTRADOR)]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create(Estado estado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(estado);
        }

        // GET: Estados/Edit/5
        [Authorize(Roles = Constantes.ROL_ADMINISTRADOR)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var estado = await _context.Estados.FindAsync(id);
            if (estado == null) return NotFound();

            return View(estado);
        }

        // POST: Estados/Edit/5
        [HttpPost]
        [Authorize(Roles = Constantes.ROL_ADMINISTRADOR)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Estado estado)
        {
            if (id != estado.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(estado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(estado);
        }

        // GET: Estados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var estado = await _context.Estados.FindAsync(id);
            if (estado == null) return NotFound();

            return View(estado);
        }

        // POST: Estados/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = Constantes.ROL_ADMINISTRADOR)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estado = await _context.Estados.FindAsync(id);
            _context.Estados.Remove(estado);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
