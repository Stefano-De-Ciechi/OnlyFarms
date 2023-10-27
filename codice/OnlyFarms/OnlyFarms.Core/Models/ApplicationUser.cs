using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace OnlyFarms.Core.Models;

public class ApplicationUser : IdentityUser
{
    public CompanyType? CompanyType { get; set; }       // Type, Id e Name sono nullable perche' l'utente admin NON appartiene a nessuna azienda
    public int? CompanyId { get; set; }             // TODO come specificare che e' una chiave esterna che deve riferire sia a FarmingCompany che a WaterCompany?
    public string? CompanyName { get; set; }
}