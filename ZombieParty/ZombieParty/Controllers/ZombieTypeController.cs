using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZombieParty.Models;
using ZombieParty.Models.Data;
using ZombieParty.ViewModels;

namespace ZombieParty.Controllers
{
    public class ZombieTypeController : Controller
    {
        private ZombiePartyDbContext _baseDonnees { get; set; }

        public ZombieTypeController(ZombiePartyDbContext baseDonnees)
        {
            _baseDonnees = baseDonnees;
        }

        public async Task<IActionResult> Index()
        {
            var zombieTypesList = await _baseDonnees.ZombieTypes.ToListAsync();

            return View(zombieTypesList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var zombies = _baseDonnees.Zombies.Where(z => z.ZombieTypeId == id);

            ZombieTypeVM zombieTypeVM = new()
            {
                ZombieType = new(),
                ZombiesList = zombies.ToList(),
                ZombiesCount = zombies.Count(),
                PointsAverage = zombies.Average(p => p.Point)
            };

            zombieTypeVM.ZombieType = await _baseDonnees.ZombieTypes.FirstOrDefaultAsync(zt => zt.Id == id);
            return View(zombieTypeVM);
        }


        //GET CREATE
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ZombieType zombieType)
        {
            if (ModelState.IsValid)
            {
                // Ajouter à la BD
                _baseDonnees.ZombieTypes.AddAsync(zombieType);
                _baseDonnees.SaveChangesAsync();
                TempData["Success"] = $"{zombieType.TypeName} zombie type added";
                return this.RedirectToAction("Index");
            }

            return this.View(zombieType);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ZombieType zombieType = await _baseDonnees.ZombieTypes.FindAsync(id);
            
            return View(zombieType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ZombieType zombieType)
        {
            if (ModelState.IsValid)
            {
                _baseDonnees.ZombieTypes.Update(zombieType);
                _baseDonnees.SaveChanges();
                TempData["Success"] = $"ZombieType {zombieType.TypeName} has been modified";
                return this.RedirectToAction("Index");
            }

            return View(zombieType);
        }

        public IActionResult Delete(int id)
        {
            ZombieType? zombieType = _baseDonnees.ZombieTypes.Find(id);
            if (zombieType == null)
            {
                return NotFound();
            }

            return View(zombieType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            ZombieType? zombieType = _baseDonnees.ZombieTypes.Find(id);
            if (zombieType == null)
            {
                return NotFound();
            }

            _baseDonnees.ZombieTypes.Remove(zombieType);
            _baseDonnees.SaveChanges();
            TempData["Success"] = $"ZombieType {zombieType.TypeName} has been removed";
            return RedirectToAction("Index");
        }
    }
}
