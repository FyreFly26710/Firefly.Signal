using Firefly.Signal.EventBus;
using Firefly.Signal.EventBus.Events.Ai;
using Firefly.Signal.JobSearch.Api.Options;
using Firefly.Signal.JobSearch.Application.Mappers;
using Firefly.Signal.JobSearch.Contracts.Responses;
using Firefly.Signal.JobSearch.Domain;
using Firefly.Signal.JobSearch.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Firefly.Signal.JobSearch.Application.Commands;

public sealed class StartUserJobAiChatDemoCommandHandler(
    JobSearchDbContext dbContext,
    IEventBus eventBus,
    IOptions<DemoAiChatOptions> options) : IRequestHandler<StartUserJobAiChatDemoCommand, UserJobAiChatDemoResponse?>
{
    public async Task<UserJobAiChatDemoResponse?> Handle(StartUserJobAiChatDemoCommand request, CancellationToken cancellationToken)
    {
        var job = await dbContext.Jobs
            .SingleOrDefaultAsync(jobPosting => jobPosting.Id == request.JobId, cancellationToken);

        if (job is null)
            return null;

        var settings = options.Value;
        var prompt = BuildPrompt(job);
        var correlationId = $"job-demo-{Guid.NewGuid():N}";

        var demoRun = UserJobAiChatDemoRun.Start(
            userAccountId: request.UserAccountId,
            jobPostingId: job.Id,
            correlationId: correlationId,
            provider: settings.Provider,
            model: settings.Model,
            prompt: prompt);

        dbContext.UserJobAiChatDemoRuns.Add(demoRun);
        await dbContext.SaveChangesAsync(cancellationToken);

        await eventBus.PublishAsync(new AiChatRequestedIntegrationEvent
        {
            CorrelationId = correlationId,
            CallerService = "job-search-api",
            ReplyEventType = nameof(AiChatCompletedIntegrationEvent),
            Provider = settings.Provider,
            Model = settings.Model,
            SystemPromptMessageId = settings.SystemPromptMessageId,
            UserPrompt = prompt
        }, cancellationToken);

        return UserJobAiChatDemoResponseMappers.ToResponse(demoRun);
    }

    private static string BuildPrompt(JobPosting job) =>
        $"""
        Demo job analysis request.
        JobId: {job.Id}
        Title: {job.Title}
        Company: {job.Company}
        Location: {job.LocationName}
        Summary: {job.Summary}
        Description: {job.Description}
        Reply with a short demo assessment.
        """;
}
