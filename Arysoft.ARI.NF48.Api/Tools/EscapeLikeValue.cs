namespace Arysoft.ARI.NF48.Api.Tools
{
    public partial class Strings
    {
        /// <summary>
        /// Escapa caracteres especiales para ser usados en una consulta SQL con LIKE.
        /// </summary>
        /// <param name="value">Termino de busqueda a escapar</param>
        /// <returns>Término escapado seguro para usar en Contains/LIKE</returns>
        /// <remarks>
        /// Autor: xBlaze
        /// Creacion: 2026-02-09
        /// Ultima Modificacion: 2026-02-09
        /// </remarks>
        public static string EscapeLikeValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value // El orden de los Replace es importante, el de "[" debe ser el primero
                .Replace("[", "[[]")    // Corchete izquierdo
                .Replace("]", "[]]")    // Corchete derecho
                .Replace("%", "[%]")    // Porcentaje
                .Replace("_", "[_]")    // Guion bajo
                .Replace("^", "[^]")    // Circunflejo
                .Replace("*", "[*]")    // Asterisco
                .Replace("&", "[&]")    // Ampersand
                .Replace("'", "''");    // Comilla simple
        } // EscapeLikeValue
    }
}
