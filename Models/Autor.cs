namespace Parcial1_P4_Yerferson.Models;

public record Autor(
    int IdAutor,
    string Nombres,
    string Nacionalidad,
    DateTime FechaNacimiento,
    decimal Sueldo);