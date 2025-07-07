using control_ppt_server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<ISlideshowControlService, SlideshowControllerService>();
builder.Services.AddSingleton<ICreatePresentationService, CreatePresentationService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (true && app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// TODO allow user to set an access code
// mobile app should send access code with each request
app.Use(async (context, next) =>
{
    // TODO store access granted status by user
    bool isControlAccessPermitted = true;
    if (isControlAccessPermitted)
        await next.Invoke();
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Control permission denied");
        return;
    }
});
app.MapControllers();

app.Urls.Add("http://0.0.0.0:8080");
app.Run();
