using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace OnlyFarms.Core.Models;

public class ApplicationUser : IdentityUser
{
    public CompanyType? CompanyType { get; set; }       // Type, Id e Name sono nullable perche' l'utente admin NON appartiene a nessuna azienda
    public int? CompanyId { get; set; }             // chiave esterna che fa riferimento sia alla tabella FarmingCompany che a WaterCompany (in base a CompanyType)
    public string? CompanyName { get; set; }
}