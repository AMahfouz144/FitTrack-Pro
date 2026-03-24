using FitTrack_Pro.Common;
using FitTrack_Pro.Services;
using FitTrack_Pro.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FitTrack_Pro.Controllers
{
    public class PlansController(IPlanService planService) : Controller
    {
        private readonly IPlanService _planService = planService;
        public async Task<IActionResult> Index()
        {
            IEnumerable<PlanResponseViewModel> plans = await _planService.GetAllPlansAsync();
            return View(plans);
        }

        [HttpGet]
        public async Task<IActionResult> AddPlan()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPlan(PlanRequestViewModel planRequest)
        {
            if (ModelState.IsValid)
            {
                ResponseModel<PlanResponseViewModel> response = await _planService.AddPlanAsync(planRequest);
                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    return View(planRequest);
                }
            }
            return View(planRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseModel<bool> isDeleted = await _planService.DeletePlanAsync(id);
            if (isDeleted.IsSuccess)
            {

                return Ok(new { message = isDeleted.Message ?? "Plan deleted successfully." });
            }
            else
            {
                return BadRequest(new { message = isDeleted.Message ?? "An error happened while deleting the plan." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            ResponseModel<PlanResponseViewModel> response = await _planService.GetPlanByIdAsync(Id);
            return View(response.Data);
        }
        [HttpPost]

        public async Task<IActionResult> Update(int id, PlanRequestViewModel planRequest)
        {
            if (ModelState.IsValid)
            {
                ResponseModel<bool> response = await _planService.UpdatePlanAsync(id, planRequest);
                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    return View(planRequest);
                }
            }
            return View(planRequest);
        }
    }
}
