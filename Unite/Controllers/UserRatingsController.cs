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
    public class UserRatingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Guid _userId;

        public UserRatingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _userId = new Guid(_userManager.GetUserId(User));
        }
        // GET: UserRatings/Rate
        public async Task<IActionResult> Rate(Guid? id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            ApplicationUser? currentUser = await _context.Users.Include(e => e.Events)
                                                              .SingleOrDefaultAsync(e => e.Id == _userId);
            if(currentUser == null)
            {
                return NotFound();
            }
            if(currentUser.Events == null)
            {
                return Unauthorized();
            }
            ApplicationUser? userToRate = await _context.Users.Include(e => e.Events)
                                                              .SingleOrDefaultAsync(e => e.Id == id);
            if(userToRate == null)
            {
                return NotFound();
            }
            if(userToRate.Events == null)
            {
                return Unauthorized();
            }
            if (currentUser.Events.Intersect(userToRate.Events).Any())
            {
                ViewData["UserId"] = userToRate.Id;
                ViewData["ReviewerId"] = _userId;
                return View();
            }
            return Unauthorized();
        }
        // POST: UserRatings/Rate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate([Bind("UserId,ReviewerId,Value,Review")] UserRating userRating)
        {
            ApplicationUser? currentUser = await _context.Users.Include(e => e.Events)
                                                              .SingleOrDefaultAsync(e => e.Id == _userId);
            if (currentUser == null)
            {
                return NotFound();
            }
            if (currentUser.Events == null)
            {
                return Unauthorized();
            }
            ApplicationUser? userToRate = await _context.Users.Include(e => e.Events)
                                                              .SingleOrDefaultAsync(e => e.Id == userRating.UserId);
            if (userToRate == null)
            {
                return NotFound();
            }
            if (userToRate.Events == null)
            {
                return Unauthorized();
            }
            if (currentUser.Events.Intersect(userToRate.Events).Any() && ModelState.IsValid)
            {
                _context.Add(userRating);
                await _context.SaveChangesAsync();
                return RedirectToAction("Profile", "Friends", userRating.UserId);
            }
            return View(userRating);
        }

        // GET: UserRatings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userRating = await _context.UserRatings.FindAsync(id);
            if (userRating == null)
            {
                return NotFound();
            }
            ViewData["ReviewerId"] = new SelectList(_context.Users, "Id", "Id", userRating.ReviewerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userRating.UserId);
            return View(userRating);
        }

        // POST: UserRatings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserId,ReviewerId,Value,Review,CreatedDate,UpdatedDate")] UserRating userRating)
        {
            if (id != userRating.UserId)
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReviewerId"] = new SelectList(_context.Users, "Id", "Id", userRating.ReviewerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", userRating.UserId);
            return View(userRating);
        }

        // GET: UserRatings/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userRating = await _context.UserRatings
                .Include(u => u.Reviewer)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userRating == null)
            {
                return NotFound();
            }

            return View(userRating);
        }

        // POST: UserRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userRating = await _context.UserRatings.FindAsync(id);
            if (userRating != null)
            {
                _context.UserRatings.Remove(userRating);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserRatingExists(Guid id)
        {
            return _context.UserRatings.Any(e => e.UserId == id);
        }
    }
}
