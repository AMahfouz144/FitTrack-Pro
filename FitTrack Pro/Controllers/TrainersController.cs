using FitTrack_Pro.Interfaces;
using FitTrack_Pro.Models;
using FitTrack_Pro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitTrack_Pro.Controllers
{
    public class TrainersController(ITrainerService trainerService) : Controller
    {
        // ════════════════════════════════════════════════════════
        //  GET  /Trainers  –  paged list with optional search
        // ════════════════════════════════════════════════════════
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(
            string? search, int page = 1, int pageSize = 10)
        {
            var vm = await trainerService.GetPagedTrainersAsync(page, pageSize, search);
            return View(vm);
        }

        // ════════════════════════════════════════════════════════
        //  GET  /Trainers/Details/5
        // ════════════════════════════════════════════════════════
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var vm = await trainerService.GetTrainerDetailsAsync(id);
            if (vm is null) return NotFound();
            return View(vm);
        }

        // ════════════════════════════════════════════════════════
        //  GET  /Trainers/Create
        // ════════════════════════════════════════════════════════
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var vm = await trainerService.GetCreateFormAsync();
            return View(vm);
        }

        // ════════════════════════════════════════════════════════
        //  POST /Trainers/Create
        // ════════════════════════════════════════════════════════
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(TrainerFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, error, newId) = await trainerService.CreateTrainerAsync(model);
            if (!success || newId == 0)
            {
                ModelState.AddModelError(string.Empty, error ?? "Failed to create trainer record.");
                return View(model);
            }

            TempData["Success"] = $"Trainer \"{model.FullName}\" has been added successfully.";
            return RedirectToAction(nameof(Details), new { id = newId });
        }

        // ════════════════════════════════════════════════════════
        //  GET  /Trainers/Edit/5
        // ════════════════════════════════════════════════════════
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await trainerService.GetEditFormAsync(id);
            if (vm is null) return NotFound();
            return View(vm);
        }
		[HttpGet]
		[Authorize(Roles = "Trainer, Admin")]
		public async Task<IActionResult> ClassDetails(int classId)
		{
			
			var gymClass = await trainerService.GetClassWithMembersAsync(classId);

			if (gymClass is null)
			{
				return NotFound("Class not found.");
			}

			return View(gymClass);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddMemberVisitNote(MemberVisitFormViewModel model)
		{
			if (!ModelState.IsValid)
			{
				TempData["Error"] = "Please fill all required fields correctly.";
				return RedirectToAction("ClassDetails", new { classId = model.GymClassId });
			}

			var (success, error) = await trainerService.SaveMemberVisitNoteAsync(model);

			if (!success)
			{
				TempData["Error"] = error;
			}
			else
			{
				TempData["SuccessMessage"] = "Notes and measurements saved successfully!";
			}

			return RedirectToAction("ClassDetails", new { classId = model.GymClassId });
		}
		// ════════════════════════════════════════════════════════
		//  POST /Trainers/Edit
		// ════════════════════════════════════════════════════════
		[HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(TrainerFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, error) = await trainerService.UpdateTrainerAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error!);
                return View(model);
            }
            TempData["Success"] = $"Trainer \"{model.FullName}\" updated successfully.";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        // ════════════════════════════════════════════════════════
        //  POST /Trainers/Delete  (AJAX-friendly)
        // ════════════════════════════════════════════════════════
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await trainerService.DeleteTrainerAsync(id);
            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Trainer deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

		[HttpGet]
		[Authorize(Roles = "Trainer")]
		public async Task<IActionResult> Profile()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (userId == null) return Unauthorized();

			var trainer = await trainerService.GetTrainerProfileByUserIdAsync(userId);

			if (trainer is null) return NotFound("Trainer profile not found.");

			return View(trainer);
		}
	}
}
