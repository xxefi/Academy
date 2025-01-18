using System.Text;
using Academy.Application.Mappings;
using Academy.Infrastructure.Context;
using Academy.Domain.Abstractions.Repositories;
using Academy.Infrastructure.Repositories;
using Academy.Application.Services.Auth;
using Academy.Application.Services.Main;
using Academy.Application.Validators.Create;
using Academy.Application.Validators.Update;
using Microsoft.EntityFrameworkCore;
using Academy.Domain.Abstractions.Services.Auth;
using Academy.Domain.Abstractions.Services.Main;
using Academy.Domain.Abstractions.UOW;
using Academy.Infrastructure.UOW;
using Academy.Presentation.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value,
        IssuerSigningKey = 
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:Secret").Value))
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Academy API",
        Version = "v1",
        Description = "API for managing the Academy system, including students, teachers, and groups",
        TermsOfService = new Uri("https://github.com/xxefi"),
        Contact = new OpenApiContact
        {
            Name = "Academy Support Team",
            Email = "magsudluefgan@gmail.com",
            Url = new Uri("https://instagram.com/xx.efi")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

});

builder.Services.AddDbContext<AcademyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Academy")));

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));


builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IFacultyRepository, FacultyRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IFacultyService, FacultyService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<CreateUserValidator>();
builder.Services.AddScoped<CreateRoleValidator>();
builder.Services.AddScoped<CreateGroupValidator>();
builder.Services.AddScoped<CreateFacultyValidator>();
builder.Services.AddScoped<CreateDepartmentValidator>();

builder.Services.AddScoped<UpdateUserValidator>();
builder.Services.AddScoped<UpdateRoleValidator>();
builder.Services.AddScoped<UpdateGroupValidator>();
builder.Services.AddScoped<UpdateFacutyValidator>();
builder.Services.AddScoped<UpdateDepartmentValidator>();


builder.Services.AddSingleton<ILocalizationService, LocalizationService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<CustomSuccessResponseMiddleware>();
app.UseMiddleware<CustomExceptionMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();


//app.UseSwagger();
//app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseCors();

app.Run();
