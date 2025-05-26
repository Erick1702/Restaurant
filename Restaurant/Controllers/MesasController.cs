using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Restaurant.Datos;
using Restaurant.Models;
using Restaurant.Servicios;

namespace Restaurant.Controllers
{
    public class MesasController: Controller
    {
        private readonly ApplicationDbContext _context;

        public MesasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mesas
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
        public async Task<IActionResult> Index()
        {
            var mesas = await _context.Mesas
                .Include(m => m.Estado) // Traer el Estado relacionado
                .ToListAsync();
            return View(mesas);
        }

        // GET: Mesas/Create
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
        public async Task<IActionResult> Create()
        {
            ViewData["EstadoId"] = new SelectList(await _context.Estados.ToListAsync(), "Id", "Nombre");
            return View();
        }

        // POST: Mesas/Create
        [HttpPost]
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mesa model)
        {
            if (ModelState.IsValid)
            {
                var estadoLibre = await _context.Estados
                                        .FirstOrDefaultAsync(e => e.Nombre == "Libre");

                var mesa = new Mesa
                {
                    Numero = model.Numero,
                    Capacidad = model.Capacidad,
                    Descripcion = model.Descripcion,
                    Activo = true,
                    EstadoId = estadoLibre?.Id // Asignar el ID del estado "Libre"
                };

                _context.Add(mesa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EstadoId"] = new SelectList(await _context.Estados.ToListAsync(), "Id", "Nombre", model.EstadoId);
            return View(model);
        }

        // GET: Mesas/Edit/5
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var mesa = await _context.Mesas.FindAsync(id);
            if (mesa == null) return NotFound();

            ViewData["EstadoId"] = new SelectList(await _context.Estados.ToListAsync(), "Id", "Nombre", mesa.EstadoId);
            return View(mesa);
        }

        // POST: Mesas/Edit/5
        [HttpPost]
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Mesa mesa)
        {
            if (id != mesa.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(mesa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EstadoId"] = new SelectList(await _context.Estados.ToListAsync(), "Id", "Nombre", mesa.EstadoId);
            return View(mesa);
        }

        // GET: Mesas/Delete/5
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var mesa = await _context.Mesas
                .Include(m => m.Estado)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mesa == null) return NotFound();

            return View(mesa);
        }

        // POST: Mesas/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = $"{Constantes.ROL_ADMINISTRADOR},{Constantes.ROL_MESERO}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mesa = await _context.Mesas.FindAsync(id);
            _context.Mesas.Remove(mesa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
