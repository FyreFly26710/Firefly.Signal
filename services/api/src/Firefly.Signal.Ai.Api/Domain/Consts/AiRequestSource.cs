namespace Firefly.Signal.Ai.Domain;

public enum AiRequestSource
{
    /// <summary>
    /// Request arrived directly via HTTP endpoint (streaming or non-streaming).
    /// </summary>
    Http = 1,

    /// <summary>
    /// Request arrived via a MQ integration event from another service.
    /// Response is published back to the originating service via MQ.
    /// </summary>
    MqEvent = 2
}
