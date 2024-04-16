using Announcement.Data;
using Announcement.Models;
using Announcement.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Announcement.Controllers
{
    public class BulletinsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public BulletinsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddBulletioViewModel viewModel)
        {
            if (viewModel.PublishDate == new DateTime(1, 1, 1) || viewModel.EndDate == new DateTime(1, 1, 1) || viewModel.Title == null) {
                ModelState.AddModelError(string.Empty, "PublishDate 和 EndDate 是必填字段。");
                return View(viewModel);
            }

            var bulletin = new Bulletin
            {
                Title = viewModel.Title,
                Category = viewModel.Category,
                PublishDate = viewModel.PublishDate,
                EndDate = viewModel.EndDate,
                Status = viewModel.Status,
                PinTop = viewModel.PinTop,
                Content = viewModel.Content,
            };
            await dbContext.Bulletins.AddAsync(bulletin);
            await dbContext.SaveChangesAsync();

            // 如果PinTop為1，則將其他公告的PinTop設為0
            if (bulletin.PinTop)
            {
                var otherBulletins = dbContext.Bulletins.Where(b => b.Id != bulletin.Id && b.PinTop);
                foreach (var otherBulletin in otherBulletins)
                {
                    otherBulletin.PinTop = false;
                }
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Bulletins");
        }

        [HttpGet]
        public async Task<IActionResult> List(string? searchQuery , DateTime? startDate, DateTime? endDate)
        {
            var query = dbContext.Bulletins.AsQueryable();
            ViewBag.SearchQuery = searchQuery;
            // Filtering
            query = query.Where(x => x.Status == "顯示");
            if (string.IsNullOrWhiteSpace(searchQuery) == false)
            {
                query = query.Where(x => x.Title.Contains(searchQuery));
            }

            if (startDate.HasValue)
            {
                query = query.Where(x => x.PublishDate >= startDate);
            }
            if (endDate.HasValue)
            {
                query = query.Where(x => x.PublishDate <= endDate);
            }
            // Sorting
            query = query.OrderByDescending(x => x.Sort);
            query = query.OrderByDescending(x => x.PinTop);

            return View(await query.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var bulletin = await dbContext.Bulletins.FindAsync(id);

            return View(bulletin);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Bulletin viewModel)
        {
            var bulletin = await dbContext.Bulletins.FindAsync(viewModel.Id);
            if (bulletin is not null)
            {
                bulletin.Title = viewModel.Title;
                bulletin.Category = viewModel.Category;
                bulletin.PublishDate = viewModel.PublishDate;
                bulletin.EndDate = viewModel.EndDate;
                bulletin.Status = viewModel.Status;
                bulletin.PinTop = viewModel.PinTop;
                bulletin.Content = viewModel.Content;

                await dbContext.SaveChangesAsync();

            }
            // 如果PinTop為1，則將其他公告的PinTop設為0
            if (bulletin.PinTop)
            {
                var otherBulletins = dbContext.Bulletins.Where(b => b.Id != bulletin.Id && b.PinTop);
                foreach (var otherBulletin in otherBulletins)
                {
                    otherBulletin.PinTop = false;
                }
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Bulletins");
        }


    }
}
