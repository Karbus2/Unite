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
    public class FriendsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FriendsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Friends
        public async Task<IActionResult> Index()
        {
            Guid userId = new Guid(_userManager.GetUserId(User));
            var applicationDbContext = _context.Friendships.Include(f => f.LeftSide)
                                                           .Include(f => f.RightSide)
                                                           .Where(e => e.RightSideId == userId);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Friends/Profile/5
        public async Task<IActionResult> Profile(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friend = await _context.Users
                .FindAsync(id);
            if (friend == null)
            {
                return NotFound();
            }

            return View(friend);
        }

        // GET: Friends/Search
        public async Task<IActionResult> Search(string? match)
        {
            var applicationDbContext = _context.Users;
            if(match != null)
            {
                return View(await applicationDbContext.Where(e => e.UserName.Contains(match)).ToListAsync());
            }
            return View(await applicationDbContext.ToListAsync());
        }

        // POST: Friends/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeftSideId,RightSideId,State,CreatedDate,UpdatedDate")] Friendship friendship)
        {
            if (ModelState.IsValid)
            {
                friendship.LeftSideId = Guid.NewGuid();
                _context.Add(friendship);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeftSideId"] = new SelectList(_context.Users, "Id", "Id", friendship.LeftSideId);
            ViewData["RightSideId"] = new SelectList(_context.Users, "Id", "Id", friendship.RightSideId);
            return View(friendship);
        }

        // GET: Friends/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friendship = await _context.Friendships.FindAsync(id);
            if (friendship == null)
            {
                return NotFound();
            }
            ViewData["LeftSideId"] = new SelectList(_context.Users, "Id", "Id", friendship.LeftSideId);
            ViewData["RightSideId"] = new SelectList(_context.Users, "Id", "Id", friendship.RightSideId);
            return View(friendship);
        }

        // POST: Friends/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("LeftSideId,RightSideId,State,CreatedDate,UpdatedDate")] Friendship friendship)
        {
            if (id != friendship.LeftSideId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(friendship);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FriendshipExists(friendship.LeftSideId))
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
            ViewData["LeftSideId"] = new SelectList(_context.Users, "Id", "Id", friendship.LeftSideId);
            ViewData["RightSideId"] = new SelectList(_context.Users, "Id", "Id", friendship.RightSideId);
            return View(friendship);
        }

        // GET: Friends/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friendship = await _context.Friendships
                .Include(f => f.LeftSide)
                .Include(f => f.RightSide)
                .FirstOrDefaultAsync(m => m.LeftSideId == id);
            if (friendship == null)
            {
                return NotFound();
            }

            return View(friendship);
        }

        // POST: Friends/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var friendship = await _context.Friendships.FindAsync(id);
            if (friendship != null)
            {
                _context.Friendships.Remove(friendship);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FriendshipExists(Guid id)
        {
            return _context.Friendships.Any(e => e.LeftSideId == id);
        }
    }
}
