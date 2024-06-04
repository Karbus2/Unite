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
using Microsoft.AspNetCore.Authorization;

namespace Unite.Controllers
{
    [Authorize]
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
            var applicationDbContext = _context.Users.Include(e => e.LeftSideFriendships)
                                                     .Include(e => e.RightSideFriendships);
            if(match != null)
            {
                return View(await applicationDbContext.Where(e => e.UserName.Contains(match)).ToListAsync());
            }
            return View(await applicationDbContext.ToListAsync());
        }

        // POST: Friends/Add/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Guid? id, string returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            if (id == userId)
            {
                return BadRequest();
            }
            Friendship friendship = new Friendship();
            friendship.State = Friendship.FriendshipState.ToAccept;
            friendship.LeftSideId = userId;
            if(await _context.Users.SingleOrDefaultAsync(e => e.Id == id) == null)
            {
                return NotFound();
            }
            friendship.RightSideId = (Guid)id;
            _context.Add(friendship);
            await _context.SaveChangesAsync();
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }
        // POST: Friends/Accept/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(Guid? id, string returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            if (id == userId)
            {
                return BadRequest();
            }

            Friendship? friendship = await _context.Friendships.SingleOrDefaultAsync(e => e.LeftSideId == id && e.RightSideId == userId && e.State == Friendship.FriendshipState.ToAccept);
            if (friendship == null)
            {
                return NotFound();
            }
            friendship.State = Friendship.FriendshipState.Accepted;
            Friendship reversedFriendship = new Friendship();
            reversedFriendship.State = friendship.State;
            reversedFriendship.LeftSideId = friendship.RightSideId;
            reversedFriendship.RightSideId = friendship.LeftSideId;

            _context.Add(reversedFriendship);
            await _context.SaveChangesAsync();

            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        // POST: Friends/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid? id, string returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            if (id == userId)
            {
                return BadRequest();
            }

            Friendship? friendship = await _context.Friendships.SingleOrDefaultAsync(e => e.LeftSideId == userId && e.RightSideId == id && e.State == Friendship.FriendshipState.ToAccept);
            if (friendship == null)
            {
                return NotFound();
            }
            
            _context.Remove(friendship);
            await _context.SaveChangesAsync();

            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }

        // POST: Friends/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid? id, string returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }
            Guid userId = new Guid(_userManager.GetUserId(User));
            if (id == userId)
            {
                return BadRequest();
            }

            Friendship? friendship = await _context.Friendships.SingleOrDefaultAsync(e => e.LeftSideId == userId && e.RightSideId == id && e.State == Friendship.FriendshipState.Accepted);
            if (friendship == null)
            {
                return NotFound();
            }
            Friendship? reversedFriendship = await _context.Friendships.SingleOrDefaultAsync(e => e.LeftSideId == id && e.RightSideId == userId && e.State == Friendship.FriendshipState.Accepted);
            if (reversedFriendship == null)
            {
                return NotFound();
            }
            _context.Remove(friendship);
            _context.Remove(reversedFriendship);
            await _context.SaveChangesAsync();

            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }
    }
}
