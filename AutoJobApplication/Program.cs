using AutoJobApplication.Data;
using AutoJobApplication.Interfaces;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IFileUploadService, FileUploadService>(); // Ensure that FileUploadService implements IFileUploadService
builder.Services.AddSingleton<ISkillService, SkillService>();           // Ensure that SkillService implements ISkillService
builder.Services.AddSingleton<ICvService, CvService>();                 // Ensure that CvService implements ICvService
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IFileStateService, FileStateService>(); // Register the FileStateService
builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
