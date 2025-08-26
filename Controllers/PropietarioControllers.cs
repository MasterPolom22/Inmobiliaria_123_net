using WebInmo.Data;
using WebInmo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inmobiliaria.Web.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly AppDbContext _ctx;
        public PropietariosController(AppDbContext ctx) => _ctx = ctx;

        // GET: Propietarios
        public async Task<IActionResult> Index(string? q)
        {
            var query = _ctx.Propietario.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(p =>
                    (p.Apellido != null && p.Apellido.Contains(q)) ||
                    (p.Nombre != null && p.Nombre.Contains(q)) ||
                    (p.Dni != null && p.Dni.Contains(q))
                );
            }
            var list = await query.OrderBy(p => p.Apellido).ThenBy(p => p.Nombre).ToListAsync();
            return View(list);
        }

        // GET: Propietarios/Create
        public IActionResult Create() => View();

        // POST: Propietarios/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Propietario propietario)
        {
            if (!ModelState.IsValid) return View(propietario);

            // Unicidad de DNI (extra por si no llegó a DB)
            bool dniExiste = await _ctx.Propietario.AnyAsync(p => p.Dni == propietario.Dni);
            if (dniExiste)
            {
                ModelState.AddModelError(nameof(Propietario.Dni), "El DNI ya está registrado.");
                return View(propietario);
            }

            _ctx.Add(propietario);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Propietarios/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var prop = await _ctx.Propietario.FindAsync(id);
            if (prop == null) return NotFound();
            return View(prop);
        }

        // POST: Propietarios/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Propietario propietario)
        {
            if (id != propietario.Id) return BadRequest();
            if (!ModelState.IsValid) return View(propietario);

            // proteger DNI duplicado al editar
            bool dniOcupado = await _ctx.Propietario
                .AnyAsync(p => p.Dni == propietario.Dni && p.Id != propietario.Id);
            if (dniOcupado)
            {
                ModelState.AddModelError(nameof(Propietario.Dni), "El DNI pertenece a otro propietario.");
                return View(propietario);
            }

            try
            {
                _ctx.Update(propietario);
                await _ctx.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _ctx.Propietario.AnyAsync(p => p.Id == id)) return NotFound();
                throw;
            }
        }

        // GET: Propietarios/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var prop = await _ctx.Propietario.FirstOrDefaultAsync(p => p.Id == id);
            if (prop == null) return NotFound();
            return View(prop);
        }

        // POST: Propietarios/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prop = await _ctx.Propietario.FindAsync(id);
            if (prop == null) return NotFound();

            _ctx.Propietario.Remove(prop);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Propietarios/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var prop = await _ctx.Propietario.FirstOrDefaultAsync(p => p.Id == id);
            if (prop == null) return NotFound();
            return View(prop);
        }

        public async Task<IActionResult> DetailsPartial(int id)
        {
            var prop = await _ctx.Propietario.FirstOrDefaultAsync(p => p.Id == id);
            if (prop == null) return NotFound();
            return PartialView("_DetailsPartial", prop);
        }
    }

}
