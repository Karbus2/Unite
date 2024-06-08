using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UserRatingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRatingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: UserRatings/Rate
        public async Task<IActionResult> Rate(Guid? id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            if(!await HasCommonEvent(userId, (Guid)id))
            {
                return Unauthorized();
            }
            ViewData["UserId"] = id;
            ViewData["ReviewerId"] = userId;
            return View();
        }
        // POST: UserRatings/Rate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate([Bind("UserId,ReviewerId,Value,Review")] UserRating userRating)
        {
            if(userRating == null) 
            { 
                return BadRequest(); 
            }
            if(_context.UserRatings.Any(e => e.UserId == userRating.UserId && e.ReviewerId == userRating.ReviewerId))
            {
                return BadRequest();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            if(!await HasCommonEvent(userRating.UserId, userId))
            {
                return Unauthorized();
            }
            if (ModelState.IsValid)
            {
                _context.Add(userRating);
                await _context.SaveChangesAsync();
                return RedirectToAction("Profile", "Friends", new { id = userRating.UserId });
            }
            return View(userRating);
        }

        // GET: UserRatings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            var userRating = await _context.UserRatings.SingleOrDefaultAsync(e => e.UserId == id && e.ReviewerId == userId);
            if (userRating == null)
            {
                return NotFound();
            }
            return View(userRating);
        }

        // POST: UserRatings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserId,ReviewerId,Value,Review,CreatedDate,UpdatedDate")] UserRating userRating)
        {
            if (id != userRating.UserId)
            {
                return NotFound();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            if (!_context.UserRatings.Any(e => e.UserId == id && e.ReviewerId == userId))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userRating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserRatingExists(userRating.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Profile", "Friends", new { id = userRating.UserId });
            }
            return View(userRating);
        }

        // POST: UserRatings/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            var userRating = await _context.UserRatings.SingleOrDefaultAsync(e => e.UserId == id && e.ReviewerId == userId);
            if(userRating == null)
            {
                return NotFound();
            }
            _context.UserRatings.Remove(userRating);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile", "Friends", new {id = userRating.UserId});
        }

        private bool UserRatingExists(Guid id)
        {
            return _context.UserRatings.Any(e => e.UserId == id);
        }
        private async Task<bool> HasCommonEvent(Guid currentUserId, Guid userToRateId)
        {
            ApplicationUser? currentUser = await _context.Users.Include(e => e.Events!)
                                                               .ThenInclude(e => e.Event)
                                                               .SingleOrDefaultAsync(e => e.Id == currentUserId);
            if (currentUser == null)
            {
                return false;
            }
            if (currentUser.Events == null)
            {
                return false;
            }
            ApplicationUser? userToRate = await _context.Users.Include(e => e.Events!)
                                                              .ThenInclude(e => e.Event)
                                                              .SingleOrDefaultAsync(e => e.Id == userToRateId);
            if (userToRate == null)
            {
                return false;
            }
            if (userToRate.Events == null)
            {
                return false;
            }
            if (currentUser.Events.Where(e => e.Event!.End <= DateTime.Now && e.State == UserEvent.UserEventState.Accepted).Select(e => e.Event)
                                  .Intersect(userToRate.Events.Where(e => e.Event!.End <= DateTime.Now && e.State == UserEvent.UserEventState.Accepted).Select(e => e.Event))
                                  .Any())
            {
                return true;
            }
            return false;
        }
    }
}
