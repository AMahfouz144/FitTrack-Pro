using FitTrack_Pro.Common;
using FitTrack_Pro.ViewModels;

namespace FitTrack_Pro.Services
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanResponseViewModel>> GetAllPlansAsync();
        Task<ResponseModel<PlanResponseViewModel>> AddPlanAsync(PlanRequestViewModel planRequest);

        Task<ResponseModel<bool>> DeletePlanAsync(int id);

        Task<ResponseModel<bool>>UpdatePlanAsync(int id, PlanRequestViewModel planRequest);

        Task<ResponseModel<PlanResponseViewModel>> GetPlanByIdAsync(int id);
    }
}
