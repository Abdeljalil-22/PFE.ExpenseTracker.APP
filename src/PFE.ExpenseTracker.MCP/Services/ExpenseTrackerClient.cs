
using System.Text.Json;
using MediatR;
using PFE.ExpenseTracker.MCP.Models;
using PFE.ExpenseTracker.Application.Features.Expenses.Commands;
using PFE.ExpenseTracker.Application.Features.Expenses.Queries;
using PFE.ExpenseTracker.Application.Features.Budgets.Commands;
using PFE.ExpenseTracker.Application.Features.Budgets.Queries;
using PFE.ExpenseTracker.Application.Features.Categories.Commands;
using PFE.ExpenseTracker.Application.Features.Categories.Queries;
using PFE.ExpenseTracker.Application.Features.FinancialGoals.Commands;
using PFE.ExpenseTracker.Application.Features.FinancialGoals.Queries;

namespace PFE.ExpenseTracker.MCP.Services;

public class ExpenseTrackerClient : IExpenseTrackerClient
{
    private readonly IMediator _mediator;
    private readonly ILogger<ExpenseTrackerClient> _logger;

    public ExpenseTrackerClient(IMediator mediator, ILogger<ExpenseTrackerClient> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<ActionResult> ExecuteActionAsync(McpAction action, string userId)
    {
        try
        {
            _logger.LogInformation("Executing action {Type} for entity {Entity} with parameters {@Parameters}", action.Type, action.Entity, action.Parameters);

            var entity = action.Entity?.ToLowerInvariant();
            var type = action.Type?.ToUpperInvariant();
            switch (entity)
            {
                case "expense":
                    switch (type)
                    {
                        case "CREATE":
                            var createExpense = new CreateExpenseCommand
                            {
                                UserId = Guid.TryParse(userId, out var uid) ? uid : Guid.Empty,
                                Description = action.Parameters.TryGetValue("description", out var desc) ? desc?.ToString() : null,
                                Amount = action.Parameters.TryGetValue("amount", out var amt) && decimal.TryParse(amt?.ToString(), out var d) ? d : 0,
                                Date = action.Parameters.TryGetValue("date", out var dt) && DateTime.TryParse(dt?.ToString(), out var date) ? date : DateTime.UtcNow,
                                CategoryId = action.Parameters.TryGetValue("categoryId", out var cat) && Guid.TryParse(cat?.ToString(), out var cid) ? cid : Guid.Empty,
                                Notes = action.Parameters.TryGetValue("notes", out var notes) ? notes?.ToString() : null,
                                IsRecurring = action.Parameters.TryGetValue("isRecurring", out var rec) && bool.TryParse(rec?.ToString(), out var b) ? b : false,
                                RecurringFrequency = action.Parameters.TryGetValue("recurringFrequency", out var freq) ? freq?.ToString() : null,
                                IsShared = action.Parameters.TryGetValue("isShared", out var shared) && bool.TryParse(shared?.ToString(), out var sh) ? sh : false
                            };
                            var resultCreateExpense = await _mediator.Send(createExpense);
                            return new ActionResult { Success = resultCreateExpense.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultCreateExpense.Data }, Error = resultCreateExpense.Errors != null ? string.Join("; ", resultCreateExpense.Errors) : null };
                        case "READ":
                            if (action.Parameters.TryGetValue("id", out var eid) && Guid.TryParse(eid?.ToString(), out var expenseId))
                            {
                                var getExpense = new GetExpenseByIdQuery { Id = expenseId };
                                var resultGetExpense = await _mediator.Send(getExpense);
                                return new ActionResult { Success = resultGetExpense.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultGetExpense.Data }, Error = resultGetExpense.Errors != null ? string.Join("; ", resultGetExpense.Errors) : null };
                            }
                            else
                            {
                                var getExpenses = new GetExpensesQuery { UserId = Guid.TryParse(userId, out var uid2) ? uid2 : Guid.Empty };
                                var resultGetExpenses = await _mediator.Send(getExpenses);
                                return new ActionResult { Success = resultGetExpenses.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultGetExpenses.Data }, Error = resultGetExpenses.Errors != null ? string.Join("; ", resultGetExpenses.Errors) : null };
                            }
                        case "UPDATE":
                            var updateExpense = new UpdateExpenseCommand
                            {
                                Id = action.Parameters.TryGetValue("id", out var eid2) && Guid.TryParse(eid2?.ToString(), out var expId) ? expId : Guid.Empty,
                                UserId = Guid.TryParse(userId, out var uid3) ? uid3 : Guid.Empty,
                                Description = action.Parameters.TryGetValue("description", out var desc2) ? desc2?.ToString() : null,
                                Amount = action.Parameters.TryGetValue("amount", out var amt2) && decimal.TryParse(amt2?.ToString(), out var d2) ? d2 : 0,
                                Date = action.Parameters.TryGetValue("date", out var dt2) && DateTime.TryParse(dt2?.ToString(), out var date2) ? date2 : DateTime.UtcNow,
                                CategoryId = action.Parameters.TryGetValue("categoryId", out var cat2) && Guid.TryParse(cat2?.ToString(), out var cid2) ? cid2 : Guid.Empty,
                                Notes = action.Parameters.TryGetValue("notes", out var notes2) ? notes2?.ToString() : null,
                                IsRecurring = action.Parameters.TryGetValue("isRecurring", out var rec2) && bool.TryParse(rec2?.ToString(), out var b2) ? b2 : false,
                                RecurringFrequency = action.Parameters.TryGetValue("recurringFrequency", out var freq2) ? freq2?.ToString() : null,
                                IsShared = action.Parameters.TryGetValue("isShared", out var shared2) && bool.TryParse(shared2?.ToString(), out var sh2) ? sh2 : false
                            };
                            var resultUpdateExpense = await _mediator.Send(updateExpense);
                            return new ActionResult { Success = resultUpdateExpense.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultUpdateExpense.Data }, Error = resultUpdateExpense.Errors != null ? string.Join("; ", resultUpdateExpense.Errors) : null };
                        case "DELETE":
                            var deleteExpense = new DeleteExpenseCommand
                            {
                                Id = action.Parameters.TryGetValue("id", out var did) && Guid.TryParse(did?.ToString(), out var delId) ? delId : Guid.Empty,
                                UserId = Guid.TryParse(userId, out var uid4) ? uid4 : Guid.Empty
                            };
                            var resultDeleteExpense = await _mediator.Send(deleteExpense);
                            return new ActionResult { Success = resultDeleteExpense.Succeeded, Error = resultDeleteExpense.Errors != null ? string.Join("; ", resultDeleteExpense.Errors) : null };
                    }
                    break;
                case "budget":
                    switch (type)
                    {
                        case "CREATE":
                            var createBudget = new CreateBudgetCommand
                            {
                                UserId = Guid.TryParse(userId, out var buid) ? buid : Guid.Empty,
                                // Name = action.Parameters.TryGetValue("name", out var bname) ? bname?.ToString() : null,
                                Amount = action.Parameters.TryGetValue("amount", out var bamt) && decimal.TryParse(bamt?.ToString(), out var bd) ? bd : 0,
                                StartDate = action.Parameters.TryGetValue("startDate", out var bsd) && DateTime.TryParse(bsd?.ToString(), out var bstart) ? bstart : DateTime.UtcNow,
                                EndDate = action.Parameters.TryGetValue("endDate", out var bed) && DateTime.TryParse(bed?.ToString(), out var bend) ? bend : DateTime.UtcNow,
                                CategoryId = action.Parameters.TryGetValue("categoryId", out var bcat) && Guid.TryParse(bcat?.ToString(), out var bcid) ? bcid : Guid.Empty,
                                // AlertEnabled = action.Parameters.TryGetValue("alertEnabled", out var baen) && bool.TryParse(baen?.ToString(), out var baeb) ? baeb : false,
                                // AlertThresholdPercentage = action.Parameters.TryGetValue("alertThresholdPercentage", out var batp) && int.TryParse(batp?.ToString(), out var batpi) ? batpi : 0
                            };
                            var resultCreateBudget = await _mediator.Send(createBudget);
                            return new ActionResult { Success = resultCreateBudget.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultCreateBudget.Data }, Error = resultCreateBudget.Errors != null ? string.Join("; ", resultCreateBudget.Errors) : null };
                        case "READ":
                            if (action.Parameters.TryGetValue("id", out var bid) && Guid.TryParse(bid?.ToString(), out var budgetId))
                            {
                                var getBudget = new GetBudgetByIdQuery { Id = budgetId };
                                var resultGetBudget = await _mediator.Send(getBudget);
                                return new ActionResult { Success = resultGetBudget.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultGetBudget.Data }, Error = resultGetBudget.Errors != null ? string.Join("; ", resultGetBudget.Errors) : null };
                            }
                            else
                            {
                                var getBudgets = new GetBudgetsQuery { UserId = Guid.TryParse(userId, out var buid2) ? buid2 : Guid.Empty };
                                var resultGetBudgets = await _mediator.Send(getBudgets);
                                return new ActionResult { Success = resultGetBudgets.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultGetBudgets.Data }, Error = resultGetBudgets.Errors != null ? string.Join("; ", resultGetBudgets.Errors) : null };
                            }
                        case "UPDATE":
                            var updateBudget = new UpdateBudgetCommand
                            {
                                Id = action.Parameters.TryGetValue("id", out var bid2) && Guid.TryParse(bid2?.ToString(), out var budId) ? budId : Guid.Empty,
                                UserId = Guid.TryParse(userId, out var buid3) ? buid3 : Guid.Empty,
                                // Description = action.Parameters.TryGetValue("description", out var desc2) ? desc2?.ToString() : null,/
                                // Name = action.Parameters.TryGetValue("name", out var bname2) ? bname2?.ToString() : null,
                                Amount = action.Parameters.TryGetValue("amount", out var bamt2) && decimal.TryParse(bamt2?.ToString(), out var bd2) ? bd2 : 0,
                                StartDate = action.Parameters.TryGetValue("startDate", out var bsd2) && DateTime.TryParse(bsd2?.ToString(), out var bstart2) ? bstart2 : DateTime.UtcNow,
                                EndDate = action.Parameters.TryGetValue("endDate", out var bed2) && DateTime.TryParse(bed2?.ToString(), out var bend2) ? bend2 : DateTime.UtcNow,
                                CategoryId = action.Parameters.TryGetValue("categoryId", out var bcat2) && Guid.TryParse(bcat2?.ToString(), out var bcid2) ? bcid2 : Guid.Empty,
                                // AlertEnabled = action.Parameters.TryGetValue("alertEnabled", out var baen2) && bool.TryParse(baen2?.ToString(), out var baeb2) ? baeb2 : false,
                                // AlertThresholdPercentage = action.Parameters.TryGetValue("alertThresholdPercentage", out var batp2) && int.TryParse(batp2?.ToString(), out var batpi2) ? batpi2 : 0
                            };
                            var resultUpdateBudget = await _mediator.Send(updateBudget);
                            return new ActionResult { Success = resultUpdateBudget.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultUpdateBudget.Data }, Error = resultUpdateBudget.Errors != null ? string.Join("; ", resultUpdateBudget.Errors) : null };
                        case "DELETE":
                            var deleteBudget = new DeleteBudgetCommand
                            {
                                Id = action.Parameters.TryGetValue("id", out var dbid) && Guid.TryParse(dbid?.ToString(), out var delBudId) ? delBudId : Guid.Empty,
                                UserId = Guid.TryParse(userId, out var buid4) ? buid4 : Guid.Empty
                            };
                            var resultDeleteBudget = await _mediator.Send(deleteBudget);
                            return new ActionResult { Success = resultDeleteBudget.Succeeded, Error = resultDeleteBudget.Errors != null ? string.Join("; ", resultDeleteBudget.Errors) : null };
                    }
                    break;
                case "category":
                    switch (type)
                    {
                        case "CREATE":
                            var createCategory = new CreateCategoryCommand
                            {
                                UserId = Guid.TryParse(userId, out var cuid) ? cuid : Guid.Empty,
                                Name = action.Parameters.TryGetValue("name", out var cname) ? cname?.ToString() : "",
                                Description = action.Parameters.TryGetValue("description", out var cdesc) ? cdesc?.ToString() : "",
                                Color = action.Parameters.TryGetValue("color", out var ccolor) ? ccolor?.ToString() : "#FFFFFF", // Default to white if not provided
                                Icon = action.Parameters.TryGetValue("icon", out var cicon) ? cicon?.ToString() : ""
                                
                            };
                            var resultCreateCategory = await _mediator.Send(createCategory);
                            return new ActionResult { Success = resultCreateCategory.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultCreateCategory.Data }, Error = resultCreateCategory.Errors != null ? string.Join("; ", resultCreateCategory.Errors) : null };
                        case "READ":
                            if (action.Parameters.TryGetValue("id", out var cid) && Guid.TryParse(cid?.ToString(), out var categoryId))
                            {
                                var getCategory = new GetCategoryByIdQuery { Id = categoryId };
                                var resultGetCategory = await _mediator.Send(getCategory);
                                return new ActionResult { Success = resultGetCategory.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultGetCategory.Data }, Error = resultGetCategory.Errors != null ? string.Join("; ", resultGetCategory.Errors) : null };
                            }
                            else
                            {
                                var getCategories = new GetCategoriesQuery { UserId = Guid.TryParse(userId, out var cuid2) ? cuid2 : Guid.Empty };
                                var resultGetCategories = await _mediator.Send(getCategories);
                                return new ActionResult { Success = resultGetCategories.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultGetCategories.Data }, Error = resultGetCategories.Errors != null ? string.Join("; ", resultGetCategories.Errors) : null };
                            }
                        case "UPDATE":
                            var updateCategory = new UpdateCategoryCommand
                            {
                                Id = action.Parameters.TryGetValue("id", out var cid2) && Guid.TryParse(cid2?.ToString(), out var catId2) ? catId2 : Guid.Empty,
                                UserId = Guid.TryParse(userId, out var cuid3) ? cuid3 : Guid.Empty,
                                Name = action.Parameters.TryGetValue("name", out var cname2) ? cname2?.ToString() : null,
                                Description = action.Parameters.TryGetValue("description", out var cdesc2) ? cdesc2?.ToString() : null,
                                Color = action.Parameters.TryGetValue("color", out var ccolor2) ? ccolor2?.ToString() : null,
                                Icon = action.Parameters.TryGetValue("icon", out var cicon2) ? cicon2?.ToString() : null
                            };
                            var resultUpdateCategory = await _mediator.Send(updateCategory);
                            return new ActionResult { Success = resultUpdateCategory.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultUpdateCategory.Data }, Error = resultUpdateCategory.Errors != null ? string.Join("; ", resultUpdateCategory.Errors) : null };
                        case "DELETE":
                            var deleteCategory = new DeleteCategoryCommand
                            {
                                Id = action.Parameters.TryGetValue("id", out var dcid) && Guid.TryParse(dcid?.ToString(), out var delCatId) ? delCatId : Guid.Empty,
                                UserId = Guid.TryParse(userId, out var cuid4) ? cuid4 : Guid.Empty
                            };
                            var resultDeleteCategory = await _mediator.Send(deleteCategory);
                            return new ActionResult { Success = resultDeleteCategory.Succeeded, Error = resultDeleteCategory.Errors != null ? string.Join("; ", resultDeleteCategory.Errors) : null };
                    }
                    break;
                case "financialgoal":
                case "financial_goal":
                    switch (type)
                    {
                        case "CREATE":
                            var createGoal = new CreateFinancialGoalCommand
                            {
                                UserId = Guid.TryParse(userId, out var guid) ? guid : Guid.Empty,
                                Name = action.Parameters.TryGetValue("name", out var gname) ? gname?.ToString() : null,
                                Description = action.Parameters.TryGetValue("description", out var gdesc) ? gdesc?.ToString() : null,
                                TargetAmount = action.Parameters.TryGetValue("targetAmount", out var gtamt) && decimal.TryParse(gtamt?.ToString(), out var gtd) ? gtd : 0,
                                TargetDate = action.Parameters.TryGetValue("targetDate", out var gtdt) && DateTime.TryParse(gtdt?.ToString(), out var gdate) ? gdate : DateTime.UtcNow
                            };
                            var resultCreateGoal = await _mediator.Send(createGoal);
                            return new ActionResult { Success = resultCreateGoal.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultCreateGoal.Data }, Error = resultCreateGoal.Errors != null ? string.Join("; ", resultCreateGoal.Errors) : null };
                        case "READ":
                            if (action.Parameters.TryGetValue("id", out var gid) && Guid.TryParse(gid?.ToString(), out var goalId))
                            {
                                var getGoal = new GetFinancialGoalByIdQuery { Id = goalId };
                                var resultGetGoal = await _mediator.Send(getGoal);
                                return new ActionResult { Success = resultGetGoal.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultGetGoal.Data }, Error = resultGetGoal.Errors != null ? string.Join("; ", resultGetGoal.Errors) : null };
                            }
                            else
                            {
                                var getGoals = new GetFinancialGoalsQuery { UserId = Guid.TryParse(userId, out var guid2) ? guid2 : Guid.Empty };
                                var resultGetGoals = await _mediator.Send(getGoals);
                                return new ActionResult { Success = resultGetGoals.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultGetGoals.Data }, Error = resultGetGoals.Errors != null ? string.Join("; ", resultGetGoals.Errors) : null };
                            }
                        case "UPDATE":
                            var updateGoal = new UpdateFinancialGoalCommand
                            {
                                Id = action.Parameters.TryGetValue("id", out var gid2) && Guid.TryParse(gid2?.ToString(), out var goalId2) ? goalId2 : Guid.Empty,
                                UserId = Guid.TryParse(userId, out var guid3) ? guid3 : Guid.Empty,
                                Name = action.Parameters.TryGetValue("name", out var gname2) ? gname2?.ToString() : null,
                                Description = action.Parameters.TryGetValue("description", out var gdesc2) ? gdesc2?.ToString() : null,
                                TargetAmount = action.Parameters.TryGetValue("targetAmount", out var gtamt2) && decimal.TryParse(gtamt2?.ToString(), out var gtd2) ? gtd2 : 0,
                                TargetDate = action.Parameters.TryGetValue("targetDate", out var gtdt2) && DateTime.TryParse(gtdt2?.ToString(), out var gdate2) ? gdate2 : DateTime.UtcNow
                            };
                            var resultUpdateGoal = await _mediator.Send(updateGoal);
                            return new ActionResult { Success = resultUpdateGoal.Succeeded, Data = new Dictionary<string, object> { ["result"] = resultUpdateGoal.Data }, Error = resultUpdateGoal.Errors != null ? string.Join("; ", resultUpdateGoal.Errors) : null };
                        case "DELETE":
                            var deleteGoal = new DeleteFinancialGoalCommand
                            {
                                Id = action.Parameters.TryGetValue("id", out var dgid) && Guid.TryParse(dgid?.ToString(), out var delGoalId) ? delGoalId : Guid.Empty,
                                UserId = Guid.TryParse(userId, out var guid4) ? guid4 : Guid.Empty
                            };
                            var resultDeleteGoal = await _mediator.Send(deleteGoal);
                            return new ActionResult { Success = resultDeleteGoal.Succeeded, Error = resultDeleteGoal.Errors != null ? string.Join("; ", resultDeleteGoal.Errors) : null };
                    }
                    break;
            }
            return new ActionResult { Success = false, Error = $"Unsupported action or entity: {action.Type} {action.Entity}" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing action {Type} for {Entity}", action.Type, action.Entity);
            return new ActionResult { Success = false, Error = ex.Message };
        }
    }
}
