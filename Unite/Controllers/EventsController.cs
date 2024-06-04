using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Unite.Data;
using Unite.Models;

namespace Unite.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Events.Include(e => e.Participants)
                                                      .AsSplitQuery()
                                                      .Include(e => e.Admin)
                                                      .ThenInclude(a => a!.LeftSideFriendships);
            Guid userId = new Guid(_userManager.GetUserId(User));

            List<Event>? events = await applicationDbContext
                .Where(e => e.End > DateTime.UtcNow 
                        && (e.Scope == Event.EventScope.Public 
                        || (e.Admin!.LeftSideFriendships != null 
                        &&  e.Scope != Event.EventScope.Private
                        &&  e.Admin.LeftSideFriendships.Any(l => l.RightSideId == userId
                                                              && l.State == Friendship.FriendshipState.Accepted)) 
                        ||  e.Participants!.Any(p => p.ParticipantId == userId)))
                .ToListAsync();
            
            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Admin)
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(m => m.Id == id);

            Guid userId = new Guid(_userManager.GetUserId(User));
            if (@event == null || (@event.Scope != Event.EventScope.Public && !@event.Participants.Any(p => p.ParticipantId == userId)))
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Location,Scope,Start,End,AdminId,Description,Size,CreatedDate,UpdatedDate")] Event @event)
        {
            if (ModelState.IsValid)
            {
                @event.Id = Guid.NewGuid();
                ApplicationUser user = await GetCurrentUserAsync();
                @event.AdminId = user.Id;
                @event.Admin = user;
                @event.Participants = [new UserEvent(user.Id, @event.Id, UserEvent.UserEventRole.Admin)];
                _context.Events.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Guid userId = new Guid(_userManager.GetUserId(User));
            var @event = await _context.Events.Include(e => e.Participants)
                                              .SingleOrDefaultAsync(e => e.Id == id);
            if (@event == null)
            {
                return NotFound();
            }
            if (!_context.UserEvents.Any(ue => ue.EventId == @event.Id
                                             && ue.ParticipantId == userId
                                             && (ue.Role == UserEvent.UserEventRole.Admin
                                             || ue.Role == UserEvent.UserEventRole.Moderator)))
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Location,Scope,Start,End,AdminId,Description,Size,CreatedDate,UpdatedDate")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            if(!_context.UserEvents.Any(ue => ue.EventId == @event.Id
                                             && ue.ParticipantId == userId
                                             && (ue.Role == UserEvent.UserEventRole.Admin 
                                             || ue.Role == UserEvent.UserEventRole.Moderator)))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Guid userId = new Guid(_userManager.GetUserId(User));
            var @event = await _context.Events
                .Include(e => e.Admin)
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null || (@event.Scope != Event.EventScope.Public && !@event.Participants.Any(p => p.ParticipantId == userId)))
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            Guid userId = new Guid(_userManager.GetUserId(User));
            var @event = await _context.Events.Include(e => e.Participants).SingleOrDefaultAsync(e => e.Id == id);
            if (@event != null && @event.AdminId == userId)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(Guid? id, string returnUrl)
        {
            if(id == null)
            {
                return BadRequest();
            }

            Guid userId = new Guid(_userManager.GetUserId(User));

            Event? @event = await _context.Events.Include(e => e.Participants)
                                                 .AsSplitQuery()
                                                 .Include(e => e.Admin)
                                                 .ThenInclude(a => a!.LeftSideFriendships)
                                                 .SingleOrDefaultAsync(e => e.Id == id); 

            if (@event == null)
            {
                return NotFound();
            }

            UserEvent? userEvent = await _context.UserEvents.SingleOrDefaultAsync(e => e.ParticipantId == userId && e.EventId == id);
            if (userEvent != null)
            {
                if(userEvent.State == UserEvent.UserEventState.Accepted)
                {
                    return BadRequest();
                }
                userEvent.State = UserEvent.UserEventState.Accepted;
            }
            else if (@event.Scope == Event.EventScope.Public 
             ||(@event.Admin!.LeftSideFriendships != null 
             && @event.Scope == Event.EventScope.FriendsOnly 
             && @event.Admin.LeftSideFriendships.Any(l => l.RightSideId == userId 
                                                       && l.State == Friendship.FriendshipState.Accepted)))
            {
                userEvent = new UserEvent(userId, (Guid)id, UserEvent.UserEventRole.Participant, UserEvent.UserEventState.Accepted);
                _context.UserEvents.Add(userEvent);
            }
            await _context.SaveChangesAsync();

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Leave(Guid? id, string returnUrl)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Guid userId = new Guid(_userManager.GetUserId(User));

            UserEvent? userEvent = await _context.UserEvents.SingleOrDefaultAsync(e => e.ParticipantId == userId && e.EventId == id);

            if(userEvent == null)
            {
                return NotFound();
            }

            _context.UserEvents.Remove(userEvent);
            await _context.SaveChangesAsync();

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Invite(Guid? participantId, Guid? eventId, string returnUrl)
        {
            if (participantId == null || eventId == null)
            {
                return BadRequest();
            }

            Guid userId = new Guid(_userManager.GetUserId(User));

            Event? @event = await _context.Events.Include(e => e.Participants)
                                                 .AsSplitQuery()
                                                 .SingleOrDefaultAsync(e => e.Id == eventId);

            // Czy event istnieje?

            if (@event == null)
            {
                return NotFound();
            }

            // Czy user istnieje?

            if(!_context.Users.Any(e => e.Id == participantId))
            {
                return NotFound();
            }

            // Czy są uczestnicy?
            // Czy zapraszający jest adminem lub moderatorem spotkania?
            // Czy zapraszany jest zaproszony lub uczestniczy?

            if (@event.Participants == null 
             || @event.Participants.Any(p => p.ParticipantId == userId 
                                         && (p.Role == UserEvent.UserEventRole.Admin 
                                          || p.Role == UserEvent.UserEventRole.Moderator)) 
             || @event.Participants.Any(p => p.ParticipantId == participantId))
            {
                return BadRequest();
            }

            UserEvent userEvent = new UserEvent((Guid)participantId, (Guid)eventId, UserEvent.UserEventRole.Participant, UserEvent.UserEventState.Invited);

            _context.UserEvents.Add(userEvent);
            await _context.SaveChangesAsync();

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(Guid id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(_userManager.GetUserId(User));
        }
    }
}
