using System.ComponentModel.DataAnnotations;

namespace WWN.Web.Middleware;

public class ValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        foreach (var arg in ctx.Arguments)
        {
            if (arg is null) continue;
            var ns = arg.GetType().Namespace;
            if (ns is null || ns.StartsWith("System") || ns.StartsWith("Microsoft")) continue;

            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(arg, new ValidationContext(arg), results, validateAllProperties: true))
            {
                var errors = results
                    .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
                    .ToDictionary(g => g.Key, g => g.Select(r => r.ErrorMessage ?? "Invalid.").ToArray());
                return Results.ValidationProblem(errors);
            }
        }
        return await next(ctx);
    }
}
