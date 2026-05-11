namespace Proyecto_SkyInit.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }   // Relación con Usuario
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
