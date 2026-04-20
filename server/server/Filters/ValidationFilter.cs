using FluentValidation;

namespace server.Filters;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var validator = ctx.HttpContext.RequestServices.GetRequiredService<IValidator<T>>();
        var argument = ctx.Arguments.OfType<T>().FirstOrDefault();

        if (argument is not null)
        {
            var result = await validator.ValidateAsync(argument);
            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Results.BadRequest(errors);
            }
        }

        return await next(ctx);
    }
}