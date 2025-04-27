using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.Servicios;

namespace Restaurant.Utilidades
{
    public static class Seeding
    {
        private static List<string> roles = new List<string>
        {
            Constantes.ROL_ADMINISTRADOR,
            Constantes.ROL_CAJERO,
            Constantes.ROL_COCINERO,
            Constantes.ROL_MESERO
        };

        public static void Aplicar (DbContext context, bool _)
        {
            foreach(var rol in roles)
            {
                var rolDB = context.Set<IdentityRole>().FirstOrDefault(r => r.Name == rol);

                if (rolDB is null)
                {
                    context.Set<IdentityRole>().Add(new IdentityRole
                    {
                        Name = rol,
                        NormalizedName = rol.ToUpper()
                    });
                    context.SaveChanges(); //Verificar si sale error por estar dentro del foreach
                }
            }
        }

        //Asyncrono
        public static async Task AplicarAsync(DbContext context, bool _, CancellationToken cancellationToken)
        {
            foreach (var rol in roles)
            {
                var rolDB =await context.Set<IdentityRole>().FirstOrDefaultAsync(r => r.Name == rol);

                if (rolDB is null)
                {
                    context.Set<IdentityRole>().Add(new IdentityRole
                    {
                        Name = rol,
                        NormalizedName = rol.ToUpper()
                    });
                   await context.SaveChangesAsync(cancellationToken); //Verificar si sale error por estar dentro del foreach
                }
            }
        }
    }
}
