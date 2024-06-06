using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Unite.Data;
using Unite.Models;

namespace Unite.Controllers
{
    public class HistoryEventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HistoryEventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: HistoryEvents
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Events.Include(e => e.Participants)
                                                      .AsSplitQuery()
                                                      .Include(e => e.Admin);
            Guid userId = new Guid(_userManager.GetUserId(User));
            List<Event>? events = await applicationDbContext
                .Where(e => e.End <= DateTime.Now
                        && e.Participants!.Any(p => p.ParticipantId == userId))
                .ToListAsync();

            List<EventDTO>? eventDTOs = new List<EventDTO>();
            foreach (Event @event in events)
            {
                eventDTOs.Add(new EventDTO(@event));
            }
            return View(eventDTOs);
        }

        // GET: HistoryEvents/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(@event => @event.Admin)
                .AsSplitQuery()
                .Include(e => e.Participants)
                .ThenInclude(p => p.Participant)
                .SingleOrDefaultAsync(m => m.Id == id);
            Guid userId = new Guid(_userManager.GetUserId(User));
            if (@event == null || @event.Participants == null)
            {
                return NotFound();
            }
            if (@event.Participants.Any(e => e.ParticipantId == userId))
            {
                return View(new EventDTO(@event));
            }
            return NotFound();
        }
    }
}
