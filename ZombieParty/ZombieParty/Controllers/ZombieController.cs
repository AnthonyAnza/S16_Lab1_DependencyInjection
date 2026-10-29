using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZombieParty.Utility;
using Microsoft.EntityFrameworkCore;
using ZombieParty.Models;
using ZombieParty.Models.Data;

namespace ZombieParty.Controllers
{
    public class ZombieController : Controller
    {
        private readonly ZombiePartyDbContext _dbContext;
        private readonly IImageFileManager _imageFileManager;

        public ZombieController(ZombiePartyDbContext dbContext, IImageFileManager imageFileManager)
        {
            _dbContext = dbContext;
            _imageFileManager = imageFileManager;
        }

        public async Task<IActionResult> Index()
        {
            this.ViewBag.MaListe = await this._dbContext.Zombie.ToListAsync();

            return View();
        }

        //GET UPSERT
        public async Task<IActionResult> Upsert(int zombieId = 0)
        {
            if (zombieId == 0)
            {
                ViewBag.ZombieTypes = new SelectList(await this._dbContext.ZombieType.ToListAsync(), "Id", "TypeName", null);
                return View(new Zombie());

            }
            else
            {
                var zombie = await this._dbContext.Zombie.FindAsync(zombieId);
                ZombieType selectedZombieType = await this._dbContext.ZombieType.FirstOrDefaultAsync(x => x.Id == zombie.ZombieTypeId);

                ViewBag.ZombieTypes = new SelectList(await this._dbContext.ZombieType.ToListAsync(), "Id", "TypeName", selectedZombieType);
                return View(zombie);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromServices] IImageFileManager imageFileManager, Zombie zombie)
        {
            if (ModelState.IsValid)
            {
                IFormCollection form = HttpContext.Request.Form;
                bool edit = zombie.Id != 0;
                string? oldImagePath = zombie.Image;

                zombie.Image = imageFileManager.UploadImage(form, edit, oldImagePath);

                if (zombie.Id == 0)
                    this._dbContext.Zombie.Add(zombie);
                else
                    this._dbContext.Update(zombie);

                await this._dbContext.SaveChangesAsync();
                return this.RedirectToAction("Index");
            }

            
            ZombieType selectedZombieType = await this._dbContext.ZombieType.FirstAsync(x => x.Id == zombie.ZombieTypeId);
            ViewBag.ZombieTypes = new SelectList(await this._dbContext.ZombieType.ToListAsync(), "Id", "TypeName", selectedZombieType);
            return this.View(zombie);
        }
    }
}
