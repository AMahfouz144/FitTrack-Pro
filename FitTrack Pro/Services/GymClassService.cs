using Common;
using FitTrack_Pro.Interfaces;
using FitTrack_Pro.Models;
using FitTrack_Pro.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FitTrack_Pro.Services
{
    public class GymClassService(
        IUnitOfWork uow,
        ITrainerRepository trainerRepo) : IGymClassService
    {
        // ────────────────────────────────────────────────────────────
        //  PAGED LIST  (with optional search)
        // ────────────────────────────────────────────────────────────
        public async Task<GymClassIndexViewModel> GetPagedGymClassesAsync(
            int page, int pageSize, string? search)
        {
            IQueryable<GymClass> query = uow.GymClasses
                .GetAllAsync()
                .Where(c => !c.IsDeleted)
                .Include(c => c.Trainer)
                .Include(c => c.Attendees);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var kw = search.Trim().ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(kw) ||
                    (c.Trainer != null && c.Trainer.FullName.ToLower().Contains(kw)));
            }

            int total = await query.CountAsync();

            var classes = await query
                .OrderBy(c => c.ScheduleTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new GymClassIndexViewModel
            {
                GymClasses = classes.Select(MapToRow),
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                SearchQuery = search
            };
        }

        // ────────────────────────────────────────────────────────────
        //  DETAILS
        // ────────────────────────────────────────────────────────────
        public async Task<GymClassDetailsViewModel?> GetGymClassDetailsAsync(int id)
        {
            var gymClass = await uow.GymClasses
                .GetAllAsync()
                .Where(c => c.Id == id && !c.IsDeleted)
                .Include(c => c.Trainer)
                .Include(c => c.Attendees)
                .FirstOrDefaultAsync();

            if (gymClass is null) return null;

            return new GymClassDetailsViewModel
            {
                Id = gymClass.Id,
                Name = gymClass.Name,
                TrainerId = gymClass.TrainerId,
                TrainerName = gymClass.Trainer?.FullName ?? "—",
                ScheduleTime = gymClass.ScheduleTime,
                DurationInMinutes = gymClass.DurationInMinutes,
                MaxCapacity = gymClass.MaxCapacity,
                AttendeeCount = gymClass.Attendees?.Count ?? 0,
                CreatedAt = gymClass.CreatedAt
            };
        }

        // ────────────────────────────────────────────────────────────
        //  FORM  (for both Create and Edit)
        // ────────────────────────────────────────────────────────────
        public async Task<GymClassFormViewModel> GetCreateFormAsync()
        {
            return new GymClassFormViewModel
            {
                TrainerOptions = await BuildTrainerOptionsAsync()
            };
        }

        public async Task<GymClassFormViewModel?> GetEditFormAsync(int id)
        {
            var gymClass = await uow.GymClasses.GetByIdAsync(id);
            if (gymClass is null || gymClass.IsDeleted) return null;

            return new GymClassFormViewModel
            {
                Id = gymClass.Id,
                Name = gymClass.Name,
                TrainerId = gymClass.TrainerId,
                ScheduleTime = gymClass.ScheduleTime,
                DurationInMinutes = gymClass.DurationInMinutes,
                MaxCapacity = gymClass.MaxCapacity,
                TrainerOptions = await BuildTrainerOptionsAsync(gymClass.TrainerId)
            };
        }

        // ────────────────────────────────────────────────────────────
        //  CREATE
        // ────────────────────────────────────────────────────────────
        public async Task<(bool Success, string? Error, int NewId)> CreateGymClassAsync(
            GymClassFormViewModel model)
        {
            var gymClass = new GymClass
            {
                Name = model.Name.Trim(),
                TrainerId = model.TrainerId,
                ScheduleTime = model.ScheduleTime,
                DurationInMinutes = model.DurationInMinutes,
                MaxCapacity = model.MaxCapacity,
                CreatedAt = DateTime.Now
            };

            await uow.GymClasses.AddAsync(gymClass);
            await uow.CompleteAsync();

            return (true, null, gymClass.Id);
        }

        // ────────────────────────────────────────────────────────────
        //  UPDATE
        // ────────────────────────────────────────────────────────────
        public async Task<(bool Success, string? Error)> UpdateGymClassAsync(
            GymClassFormViewModel model)
        {
            var gymClass = await uow.GymClasses.GetByIdAsync(model.Id);
            if (gymClass is null || gymClass.IsDeleted)
                return (false, "Gym class not found.");

            gymClass.Name = model.Name.Trim();
            gymClass.TrainerId = model.TrainerId;
            gymClass.ScheduleTime = model.ScheduleTime;
            gymClass.DurationInMinutes = model.DurationInMinutes;
            gymClass.MaxCapacity = model.MaxCapacity;

            uow.GymClasses.Update(gymClass);
            await uow.CompleteAsync();

            return (true, null);
        }

        // ────────────────────────────────────────────────────────────
        //  SOFT DELETE
        // ────────────────────────────────────────────────────────────
        public async Task<(bool Success, string? Error)> DeleteGymClassAsync(int id)
        {
            var gymClass = await uow.GymClasses.GetByIdAsync(id);
            if (gymClass is null || gymClass.IsDeleted)
                return (false, "Gym class not found.");

            gymClass.IsDeleted = true;
            uow.GymClasses.Update(gymClass);
            await uow.CompleteAsync();

            return (true, null);
        }

        // ────────────────────────────────────────────────────────────
        //  PRIVATE HELPERS
        // ────────────────────────────────────────────────────────────
        private async Task<IEnumerable<SelectListItem>> BuildTrainerOptionsAsync(
            int selectedId = 0)
        {
            var trainers = await trainerRepo
                .GetPagedAsync(1, 1000, t => !t.IsDeleted);

            return trainers
                .OrderBy(t => t.FullName)
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.FullName,
                    Selected = t.Id == selectedId
                });
        }

        private static GymClassRowViewModel MapToRow(GymClass c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            TrainerName = c.Trainer?.FullName ?? "—",
            ScheduleTime = c.ScheduleTime,
            DurationInMinutes = c.DurationInMinutes,
            MaxCapacity = c.MaxCapacity,
            AttendeeCount = c.Attendees?.Count ?? 0,
            CreatedAt = c.CreatedAt
        };
    }
}
