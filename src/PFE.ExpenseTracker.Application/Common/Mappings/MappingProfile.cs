using AutoMapper;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.Preferences, opt => opt.MapFrom(s => s.Preferences));

            CreateMap<UserPreferences, UserPreferencesDto>();

            CreateMap<Expense, ExpenseDto>()
                .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category))
                .ForMember(d => d.Attachments, opt => opt.MapFrom(s => s.Attachments));

            CreateMap<Category, CategoryDto>();

            CreateMap<Attachment, AttachmentDto>();

            CreateMap<Budget, BudgetDto>()
                .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category));

            CreateMap<FinancialGoal, FinancialGoalDto>()
                .ForMember(d => d.Contributions, opt => opt.MapFrom(s => s.Contributions));

            CreateMap<GoalContribution, GoalContributionDto>();

            // CreateMap<Notification, NotificationDto>();
        }
    }
}
