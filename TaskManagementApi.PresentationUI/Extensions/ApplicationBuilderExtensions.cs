namespace TaskManagementApi.PresentationUI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication ConfigureApplication(this WebApplication app)
        {
             // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();


            app.UseAuthentication(); // Note: This should come BEFORE UseAuthorization
            app.UseAuthorization();
            

            app.MapControllers();

            return app;
        }
    }
}
