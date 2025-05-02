using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class User
{
    
    [Key]
    public int Id { get; set; }

  
    // The username chosen by the user.
   
    [Required]
    public string Username { get; set; } = string.Empty;

   
    // The email address of the user. 
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

   
    // The hashed version of the user's password.
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

   
    [NotMapped] // Excludes this field from being stored in the database.
    public string Password { get; set; } = string.Empty;
}
