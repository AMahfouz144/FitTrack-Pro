using Common;
using FitTrack_Pro.Common;
using FitTrack_Pro.Models;
using FitTrack_Pro.ViewModels;

namespace FitTrack_Pro.Services
{
    public class PlanService(IUnitOfWork unitOfWork) : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ResponseModel<PlanResponseViewModel>> AddPlanAsync(PlanRequestViewModel planRequest)
        {
            var plan = new SubscriptionPlan
            {
                Name = planRequest.Name,
                DurationInDays = planRequest.DurationInDays,
                Price = planRequest.Price,
                Description = planRequest.Description
            };

            try
            {
                await _unitOfWork.SubscriptionPlans.AddAsync(plan);
                await _unitOfWork.CompleteAsync();
                ResponseModel<PlanResponseViewModel> response = new ResponseModel<PlanResponseViewModel>
                {
                    IsSuccess = true,
                    Message = "Plan added successfully",
                    Data = new PlanResponseViewModel
                    {
                        Id = plan.Id,
                        Name = plan.Name,
                        DurationInDays = plan.DurationInDays,
                        Price = plan.Price,
                        Description = plan.Description
                    }
                };
                return response;
            }
            catch (Exception ex)
            {
                ResponseModel<PlanResponseViewModel> response = new ResponseModel<PlanResponseViewModel>
                {
                    IsSuccess = false,
                    Message = $"Error adding plan: {ex.Message}",
                    Data = null
                };
                return response;
            }
        }

        public async Task<IEnumerable<PlanResponseViewModel>> GetAllPlansAsync()
        {
            IEnumerable<PlanResponseViewModel> plans = await _unitOfWork.SubscriptionPlans.GetAllAsync()
                .Where(p => !p.IsDeleted)
                .Select(p => new PlanResponseViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    DurationInDays = p.DurationInDays,
                    Price = p.Price,
                    Description = p.Description
                }).ToListAsync();
            return plans;
        }

        public async Task<ResponseModel<bool>> DeletePlanAsync(int id)
        {
            var plan = await _unitOfWork.SubscriptionPlans.GetByIdAsync(id);
            ResponseModel<bool> response = new ResponseModel<bool>();
            if (plan == null || plan.IsDeleted)
            {
                response.IsSuccess = false;
                response.Message = "Plan not found";
                return response;
            }
            else
            {
                try
                {
                    plan.IsDeleted = true;
                    _unitOfWork.SubscriptionPlans.Update(plan);
                    await _unitOfWork.CompleteAsync();
                    response.Data = true;
                    response.IsSuccess = true;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Data = false;
                    response.IsSuccess = false;
                    response.Message = $"Error deleting plan: {ex.Message}";
                    return response;
                }
            }
        }

        public async Task<ResponseModel<bool>> UpdatePlanAsync(int id, PlanRequestViewModel planRequest)
        {
            SubscriptionPlan plan = await _unitOfWork.SubscriptionPlans.GetByIdAsync(id);
            ResponseModel<bool> response = new ResponseModel<bool>();

            if (plan == null || plan.IsDeleted)
            {
                response.IsSuccess = false;
                response.Message = "Plan not found";
                response.Data = false;
                return response;
            }
            else
            {
                try
                {
                    plan.Name = planRequest.Name;
                    plan.DurationInDays = planRequest.DurationInDays;
                    plan.Price = planRequest.Price;
                    plan.Description = planRequest.Description;
                    _unitOfWork.SubscriptionPlans.Update(plan);
                    await _unitOfWork.CompleteAsync();
                    response.IsSuccess = true;
                    response.Message = "Plan updated successfully";
                    response.Data = true;
                    return response;
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = $"Error updating plan: {ex.Message}";
                    response.Data = false;
                    return response;
                }
            }
        }

        public async Task<ResponseModel<PlanResponseViewModel>> GetPlanByIdAsync(int id)
        {
            SubscriptionPlan plan = await _unitOfWork.SubscriptionPlans.GetByIdAsync(id);
            ResponseModel<PlanResponseViewModel> response = new ResponseModel<PlanResponseViewModel>();

            if (plan is null || plan.IsDeleted)
            {
                response.IsSuccess = false;
                response.Message = "Plan not found";
                response.Data = null;
                return response;
            }
            else
            {
                response.IsSuccess = true;
                response.Message = "Plan retrieved successfully";
                response.Data = new PlanResponseViewModel
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    DurationInDays = plan.DurationInDays,
                    Price = plan.Price,
                    Description = plan.Description
                };
                return response;
            }
        }
    }
}
