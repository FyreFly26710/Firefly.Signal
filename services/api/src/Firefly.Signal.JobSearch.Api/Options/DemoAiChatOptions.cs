namespace Firefly.Signal.JobSearch.Api.Options;

public sealed class DemoAiChatOptions
{
    public const string SectionName = "DemoAiChat";

    public string Provider { get; init; } = "ChatGpt";
    public string Model { get; init; } = "gpt-5.4-nano";
    public long? SystemPromptMessageId { get; init; }
}
