using Coderama.Core.Interfaces;
using Coderama.Infrastructure.Serialization;
using Coderama.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add memory cache
builder.Services.AddMemoryCache();

// Register our services
builder.Services.AddSingleton<IDocumentStorage, InMemoryDocumentStorage>();
builder.Services.AddSingleton<IDocumentSerializer, DocumentSerializer>();
builder.Services.AddSingleton<DocumentSerializerFactory>();
DocumentSerializerFactory.RegisterSerializers(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
