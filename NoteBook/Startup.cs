using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NoteBook.HttpNote;
using NoteBook.ZZService;

namespace NoteBook
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NoteBook", Version = "v1" });
            });

            // �򵥵�IOC
            services.AddScoped<BaseClients>();

            // �����÷�
            services.AddHttpClient();
            // ���ͻ��ͻ���
            //services.AddHttpClient<TypedClients>();

            // accepts any access token issued by identity server
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            // adds an authorization policy to make sure the token is for scope 'api1'
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "api1");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NoteBook v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // �������֤�м����ӵ��ܵ��У����ÿ�ε�������ʱ�����Զ�ִ�������֤��
            app.UseAuthentication();
            // �����Ȩ�м����ȷ�������ͻ����޷��������ǵ� API �˵㡣
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization("ApiScope");
            });
        }
    }
}
