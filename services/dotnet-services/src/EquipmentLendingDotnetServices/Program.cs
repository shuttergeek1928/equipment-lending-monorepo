using EquipmentLendingDotnetServices.Data;
using EquipmentLendingDotnetServices.DTOs;
using EquipmentLendingDotnetServices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<EquipmentLendingDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Use(async (ctx, next) =>
{
    Console.WriteLine($"--> {ctx.Request.Method} {ctx.Request.Path}");
    await next();
    Console.WriteLine($"<-- {ctx.Response.StatusCode}");
});


app.MapControllers();

# region Equipment Endpoints
//app.MapGet("/api/equipments", async (EquipmentLendingDBContext context) =>
//{
//    Console.WriteLine("Handler entered: /api/equipments");
//    var equipments = await context.Equipments
//                                  .AsNoTracking()
//                                  .ToListAsync();   // use async
//    return Results.Ok(equipments);  // ToList* never returns null
//});

//app.MapGet("/api/equipments/get", (EquipmentLendingDBContext conetxt) =>
//{
//    var equipments = conetxt.Equipments.ToList();
//    return equipments is null ? Results.NotFound() : Results.Ok(equipments);
//})
//.WithName("GetEquipmentList")
//.WithOpenApi();

//app.MapPost("/api/equipments/add", async (AddEquipmentDTO addEquipment, EquipmentLendingDBContext conetxt) =>
//{
//    var equipments = new Equipment();

//    equipments.EquipmentName = addEquipment.EquipmentName;
//    equipments.Catgory = addEquipment.Catgory;
//    equipments.EquipmentCondition = addEquipment.EquipmentCondition;
//    equipments.TotalQuantity = addEquipment.TotalQuantity;
//    equipments.AvailableQuantity = addEquipment.AvailableQuantity;
//    equipments.IsAvailable = addEquipment.IsAvailable;

//    await conetxt.Equipments.AddAsync(equipments);

//    return equipments is null ? Results.NotFound() : Results.Ok(equipments);
//})
//.WithName("AddEquipment")
//.WithOpenApi();

//app.MapDelete("/api/equipments/delete/{equipmentId}", async (int equipmentId, EquipmentLendingDBContext context) =>
//{
//    var equipment = await context.Equipments.FindAsync(equipmentId);
//    if (equipment is null)
//    {
//        return Results.NotFound();
//    }
//    context.Equipments.Remove(equipment);
//    await context.SaveChangesAsync();
//    return Results.Ok(equipment);
//})
//.WithName("DeleteEquipment")
//.WithOpenApi();

//app.MapPut("/api/equipments/put/{equipmentId}", async (int equipmentId, [FromBody] EditEquipmentDTO editEquipment, EquipmentLendingDBContext context) =>
//{
//    var equipment = await context.Equipments.FindAsync(equipmentId);
//    if (equipment is null)
//    {
//        return Results.NotFound();
//    }

//    equipment.EquipmentCondition = editEquipment.EquipmentCondition ?? equipment.EquipmentCondition;
//    equipment.TotalQuantity = editEquipment.TotalQuantity;
//    equipment.AvailableQuantity = editEquipment.AvailableQuantity;
//    equipment.IsAvailable = editEquipment.IsAvailable ?? equipment.IsAvailable;

//    await context.SaveChangesAsync();

//    return Results.Ok(equipment);
//})
//.WithName("UpdateEquipment")
//.WithOpenApi();

#endregion

app.Run();
