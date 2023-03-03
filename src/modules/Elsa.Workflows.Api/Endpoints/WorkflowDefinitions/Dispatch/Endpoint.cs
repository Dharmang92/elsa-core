using Elsa.Abstractions;
using Elsa.Common.Models;
using Elsa.Workflows.Management.Contracts;
using Elsa.Workflows.Runtime.Models;
using Elsa.Workflows.Runtime.Services;
using JetBrains.Annotations;

namespace Elsa.Workflows.Api.Endpoints.WorkflowDefinitions.Dispatch;

[PublicAPI]
internal class Endpoint : ElsaEndpoint<Request, Response>
{
    private readonly IWorkflowDefinitionStore _store;
    private readonly IWorkflowDispatcher _workflowDispatcher;

    public Endpoint(IWorkflowDefinitionStore store, IWorkflowDispatcher workflowDispatcher)
    {
        _store = store;
        _workflowDispatcher = workflowDispatcher;
    }

    public override void Configure()
    {
        Post("/workflow-definitions/{definitionId}/dispatch");
        ConfigurePermissions("exec:workflow-definitions");
    }

    public override async Task HandleAsync(Request request, CancellationToken cancellationToken)
    {
        var exists = await _store.AnyAsync(new WorkflowDefinitionFilter{ DefinitionId = request.DefinitionId, VersionOptions = VersionOptions.Published}, cancellationToken);

        if (!exists)
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }

        var result = await _workflowDispatcher.DispatchAsync(new DispatchWorkflowDefinitionRequest(
                request.DefinitionId,
                VersionOptions.Published,
                CorrelationId: request.CorrelationId),
            cancellationToken);

        await SendOkAsync(new Response(), cancellationToken);
    }
}