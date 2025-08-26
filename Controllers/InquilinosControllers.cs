using WebInmo.Data;
using WebInmo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebInmo.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly AppDbContext _ctx;
        public InquilinosController(AppDbContext ctx) => _ctx = ctx;

        // GET: Inquilinos
        public async Task<IActionResult> Index(string? q)
        {
            var query = _ctx.Inquilino.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(i =>
                    (i.Nombre != null && i.Nombre.Contains(q)) ||
                    (i.Dni != null && i.Dni.Contains(q))
                );
            }

            var list = await query
                .OrderBy(i => i.Nombre)
                .ToListAsync();

            return View(list);
        }

        // GET: Inquilino/Create
        public IActionResult Create() => View();

        // POST: Inquilino/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inquilino inquilino)
        {
            if (!ModelState.IsValid) return View(inquilino);

            bool dniExiste = await _ctx.Inquilino.AnyAsync(i => i.Dni == inquilino.Dni);
            if (dniExiste)
            {
                ModelState.AddModelError(nameof(Inquilino.Dni), "El DNI ya está registrado.");
                return View(inquilino);
            }

            _ctx.Add(inquilino);
            await _ctx.SaveChangesAsync();
            TempData["ok"] = "Inquilino creado.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Inquilino/Edit/5        
        public async Task<IActionResult> Edit(int id)
        {
            var inq = await _ctx.Inquilino.FindAsync(id);
            if (inq == null) return NotFound();
            return View(inq);
        }

        // POST: Inquilino/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Inquilino inquilino)
        {
            if (id != inquilino.Id) return BadRequest();
            if (!ModelState.IsValid) return View(inquilino);

            bool dniOcupado = await _ctx.Inquilino
                .AnyAsync(i => i.Dni == inquilino.Dni && i.Id != inquilino.Id);

            if (dniOcupado)
            {
                ModelState.AddModelError(nameof(Inquilino.Dni), "El DNI pertenece a otro inquilino.");
                return View(inquilino);
            }

            try
            {
                _ctx.Update(inquilino);
                await _ctx.SaveChangesAsync();
                TempData["ok"] = "Inquilino actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _ctx.Inquilino.AnyAsync(i => i.Id == id)) return NotFound();
                throw;
            }
        }

        // GET: Inquilinos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var inq = await _ctx.Inquilino.FirstOrDefaultAsync(i => i.Id == id);
            if (inq == null) return NotFound();
            return View(inq);
        }

        // POST: Inquilinos/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inq = await _ctx.Inquilino.FindAsync(id);
            if (inq == null) return NotFound();

            _ctx.Inquilino.Remove(inq);
            await _ctx.SaveChangesAsync();
            TempData["ok"] = "Inquilino eliminado.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Inquilino/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var inq = await _ctx.Inquilino.FirstOrDefaultAsync(i => i.Id == id);
            if (inq == null) return NotFound();
            return View(inq);
        }

        // GET: Inquilino/DetailsPartial/5 (para modal)
        public async Task<IActionResult> DetailsPartial(int id)
        {
            var inq = await _ctx.Inquilino.FirstOrDefaultAsync(i => i.Id == id);
            if (inq == null) return NotFound();
            return PartialView("_DetailsPartial", inq);
        }
    }
}
