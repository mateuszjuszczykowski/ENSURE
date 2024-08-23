using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DATABASE_library.Models.User;

public class UserModel
{
    public string _id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    // public List<string> Devices { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
    public DateTime TokenExpiration { get; set; }
    public float EnergyPrice { get; set; }
}