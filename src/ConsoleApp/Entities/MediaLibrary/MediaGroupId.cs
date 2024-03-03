namespace Kurmann.InfuseMediaIntegrator.Entities.MediaLibrary;

/// <summary>
/// Die ID einer Mediengruppe.
/// Besteht aus einem ISO-8601-Datum und einem Titel, getrennt durch ein Leerzeichen.
/// Die ID kann als Dateiname verwendet werden.
/// Beispiel: "2022-01-01 Neujahrskonzert"
/// </summary>
public class MediaGroupId(string id)
{
    private readonly string _id = id;

    public static implicit operator string(MediaGroupId mediaGroupId)
    {
        return mediaGroupId._id;
    }
}
