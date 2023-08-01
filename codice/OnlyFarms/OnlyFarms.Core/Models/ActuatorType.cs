using System.Text.Json.Serialization;

namespace OnlyFarms.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]    // attributo usato per poter leggere i valori dell'enum in formato stringa dal body delle richieste HTTP
public enum ActuatorType
{
    Sprinkler       // irrigatore
}