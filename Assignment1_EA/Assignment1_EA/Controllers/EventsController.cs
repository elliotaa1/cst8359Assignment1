using Assignment1_EA.Data;
using Assignment1_EA.Models;
using Assignment1_EA.Models.ViewModels;
using Assignment1_EA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Assignment1_EA.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blobService;

        public EventsController(
            ApplicationDbContext context,
            BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }



        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }



        // GET: Events/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.id == id);


            if (eventItem == null)
                return NotFound();


            return View(eventItem);
        }



        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }



        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? imageUrl = null;


                if (model.BannerImage != null)
                {
                    imageUrl =
                        await _blobService.UploadFile(
                            model.BannerImage);
                }


                Event eventItem = new Event
                {
                    title = model.title,
                    description = model.description,
                    date = model.date,
                    location = model.location,
                    BannerUrl = imageUrl
                };


                _context.Events.Add(eventItem);

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }


            return View(model);
        }



        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);


            if (eventItem == null)
                return NotFound();


            return View(eventItem);
        }



        // POST: Events/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Event eventItem)
        {

            if (id != eventItem.id)
                return NotFound();


            if (ModelState.IsValid)
            {
                _context.Update(eventItem);

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }


            return View(eventItem);
        }



        // GET Delete
        public async Task<IActionResult> Delete(int id)
        {
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.id == id);


            if (eventItem == null)
                return NotFound();


            return View(eventItem);
        }



        // POST Delete
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);


            if (eventItem != null)
            {
                _context.Events.Remove(eventItem);

                await _context.SaveChangesAsync();
            }


            return RedirectToAction(nameof(Index));
        }
    }
}