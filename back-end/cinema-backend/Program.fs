namespace CinemaSeatReservationSystem1

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

module Program =
    let MyAllowSpecificOrigins = "MyCorsPolicy"

    [<EntryPoint>]
    let main args =
        let builder = WebApplication.CreateBuilder(args)

        // =============== CORS ===============
        builder.Services.AddCors(fun options ->
            options.AddPolicy(MyAllowSpecificOrigins, fun builder ->
                builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                |> ignore
            )
        )
        |> ignore

        // =============== Services ===============
        builder.Services.AddControllers() |> ignore
        builder.Services.AddEndpointsApiExplorer() |> ignore
        builder.Services.AddSwaggerGen() |> ignore

        let app = builder.Build()

        // =============== Swagger ===============
        if app.Environment.IsDevelopment() then
            app.UseSwagger() |> ignore
            app.UseSwaggerUI() |> ignore

        // =============== Static Files (مهم جداً للصور) ===============
        // ده اللي بيخلي wwwroot يتعرض
        app.UseStaticFiles() |> ignore

        app.UseHttpsRedirection() |> ignore

        // =============== CORS ===============
        app.UseCors(MyAllowSpecificOrigins) |> ignore

        app.UseAuthorization() |> ignore

        app.MapControllers() |> ignore

        app.Run()

        0
