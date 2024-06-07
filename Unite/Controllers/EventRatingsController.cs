using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Unite.Data;
using Unite.Models;

namespace Unite.Controllers
{
    [Authorize]
    public class EventRatingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Guid _userId;

        public EventRatingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _userId = new Guid(_userManager.GetUserId(User));
        }

        // GET: EventRatings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EventRatings.Include(e => e.Admin).Include(e => e.Event).Include(e => e.Reviewer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: EventRatings/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventRating = await _context.EventRatings
                .Include(e => e.Admin)
                .Include(e => e.Event)
                .Include(e => e.Reviewer)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (eventRating == null)
            {
                return NotFound();
            }

            return View(eventRating);
        }

        // GET: EventRatings/Create
        public IActionResult Create()
        {
            ViewData["AdminId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Id");
            ViewData["ReviewerId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: EventRatings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,AdminId,ReviewerId,Value,Review,CreatedDate,UpdatedDate")] EventRating eventRating)
        {
            if (ModelState.IsValid)
            {
                eventRating.EventId = Guid.NewGuid();
                _context.Add(eventRating);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminId"] = new SelectList(_context.Users, "Id", "Id", eventRating.AdminId);
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Id", eventRating.EventId);
            ViewData["ReviewerId"] = new SelectList(_context.Users, "Id", "Id", eventRating.ReviewerId);
            return View(eventRating);
        }

        // GET: EventRatings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventRating = await _context.EventRatings.FindAsync(id);
            if (eventRating == null)
            {
                return NotFound();
            }
            ViewData["AdminId"] = new SelectList(_context.Users, "Id", "Id", eventRating.AdminId);
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Id", eventRating.EventId);
            ViewData["ReviewerId"] = new SelectList(_context.Users, "Id", "Id", eventRating.ReviewerId);
            return View(eventRating);
        }

        // POST: EventRatings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("EventId,AdminId,ReviewerId,Value,Review,CreatedDate,UpdatedDate")] EventRating eventRating)
        {
            if (id != eventRating.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventRating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventRatingExists(eventRating.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminId"] = new SelectList(_context.Users, "Id", "Id", eventRating.AdminId);
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Id", eventRating.EventId);
            ViewData["ReviewerId"] = new SelectList(_context.Users, "Id", "Id", eventRating.ReviewerId);
            return View(eventRating);
        }

        // GET: EventRatings/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventRating = await _context.EventRatings
                .Include(e => e.Admin)
                .Include(e => e.Event)
                .Include(e => e.Reviewer)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (eventRating == null)
            {
                return NotFound();
            }

            return View(eventRating);
        }

        // POST: EventRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eventRating = await _context.EventRatings.FindAsync(id);
            if (eventRating != null)
            {
                _context.EventRatings.Remove(eventRating);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventRatingExists(Guid id)
        {
            return _context.EventRatings.Any(e => e.EventId == id);
        }
    }
}
