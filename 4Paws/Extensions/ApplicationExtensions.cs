namespace _4Paws.Extensions
{
    public static class ApplicationExtensions
    {
        public static WebApplication UseApp(this WebApplication app)
        {
            app.UseCors("AllowAngular");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // ── Serve uploaded images as static files ─────────────────────
            // Files in wwwroot/ are served at their path e.g.:
            // /uploads/avatars/abc123.jpg
            // /uploads/pets/def456.png
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}
