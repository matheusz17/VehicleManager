using Microsoft.EntityFrameworkCore;
using VehicleManager.Api.Data;
using VehicleManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Tudo que começa com builder.Services vira algo que pode ser injetado nas classes depois.
// Libero o acesso do front local à API enquanto o projeto não tem autenticação.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // Para o desafio qualquer origem pode consumir a API; em produção eu restringiria o domínio.
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Controllers concentram as rotas HTTP; a regra de negócio fica no service.
builder.Services.AddControllers();
// Swagger ajuda a testar cada endpoint sem precisar abrir o front.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// O service nasce uma vez por requisição e recebe o mesmo contexto do banco.
builder.Services.AddScoped<VeiculoService>();

// Uso PostgreSQL porque ele sobe fácil pelo docker-compose do projeto.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// A partir daqui monto o "caminho" de cada requisição até os controllers.
if (app.Environment.IsDevelopment())
{
    // Deixo a documentação interativa exposta só durante o desenvolvimento.
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciono HTTP para HTTPS quando a aplicação estiver configurada com certificado.
app.UseHttpsRedirection();
// CORS precisa vir antes das rotas para o navegador aceitar as chamadas do Vue.
app.UseCors("AllowFrontend");
// Não há login neste desafio, mas deixo o middleware preparado para uma evolução futura.
app.UseAuthorization();
// Finalmente as URLs /api/... são associadas aos métodos dos controllers.
app.MapControllers();

app.Run();
