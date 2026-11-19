namespace Api.Common.Filters;

using FluentValidation;

// Runs the FluentValidation validator for T BEFORE the endpoint handler.
// If the request is invalid, it returns 400 Bad Request (ProblemDetails with an "errors" object)
// and the handler is never called.
public sealed class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is null)
        {
            return await next(context);
        }

        var request = context.Arguments.OfType<T>().FirstOrDefault();

        if (request is null)
        {
            return Results.Problem(
                title: "Validation.Failed",
                detail: "The request body is missing or could not be read.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var validationResult = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());

            return Results.ValidationProblem(errors, title: "Validation.Failed");
        }

        return await next(context);
    }
}
