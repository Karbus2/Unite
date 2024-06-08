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

        public EventRatingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EventRatings/Rate
        public async Task<IActionResult> Rate(Guid? id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            Event? @event = await _context.Events.SingleOrDefaultAsync(e => e.Id == id);
            if(@event == null)
            {
                return NotFound();
            }
            if (!IsParticipant(userId, (Guid)id))
            {
                return NotFound();
            }
            if (EventRatingExists((Guid)id, userId))
            {
                return BadRequest();
            }
            if (!await IsEventEnded((Guid)id))
            {
                return BadRequest();
            }
            
            ViewData["ReviewerId"] = userId;
            ViewData["AdminId"] = @event.AdminId;
            ViewData["EventId"] = id;
            return View();
        }

        // POST: EventRatings/Rate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate([Bind("EventId,AdminId,ReviewerId,Value,Review,CreatedDate")] EventRating eventRating)
        {
            if (eventRating == null)
            {
                return BadRequest();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            if(EventRatingExists(eventRating.EventId, userId))
            {
                return BadRequest();
            }
            if (!IsParticipant(userId, eventRating.EventId))
            {
                return NotFound();
            }
            if (!await IsEventEnded(eventRating.EventId))
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                _context.Add(eventRating);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "HistoryEvents", new {id = eventRating.EventId});
            }
            return View(eventRating);
        }

        // GET: EventRatings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            var eventRating = await _context.EventRatings.SingleOrDefaultAsync(e => e.EventId == id && e.ReviewerId == userId);
            if (eventRating == null)
            {
                return NotFound();
            }
            return View(eventRating);
        }

        // POST: EventRatings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("EventId,AdminId,ReviewerId,Value,Review,CreatedDate,UpdatedDate")] EventRating eventRating)
        {
            if (id != eventRating.EventId)
            {
                return NotFound();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            if (!_context.EventRatings.Any(e => e.EventId == id && e.ReviewerId == userId))
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
                    if (!EventRatingExists(eventRating.EventId, userId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "HistoryEvents", new {id = eventRating.EventId});
            }
            return View(eventRating);
        }

        // POST: EventRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            Guid userId = new Guid(_userManager.GetUserId(User));
            var eventRating = await _context.EventRatings.SingleOrDefaultAsync(e => e.EventId == id && e.ReviewerId == userId);
            if (eventRating == null)
            {
                return NotFound();
            }
            _context.EventRatings.Remove(eventRating);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "HistoryEvents", new { id = eventRating.EventId });
        }

        private bool EventRatingExists(Guid eventId, Guid reviewerId)
        {
            return _context.EventRatings.Any(e => e.EventId == eventId && e.ReviewerId == reviewerId);
        }
        private bool IsParticipant(Guid participantId, Guid eventId)
        {
            if(_context.UserEvents.Any(e => e.ParticipantId == participantId && e.EventId == eventId && e.State == UserEvent.UserEventState.Accepted))
            {
                return true;
            }
            return false;
        }
        private async Task<bool> IsEventEnded(Guid eventId) 
        { 
            Event? @event = await _context.Events.FindAsync(eventId);
            if (@event != null && @event.End <= DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}
