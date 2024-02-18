using CSharpFunctionalExtensions;

namespace Kurmann.InfuseMediaIntegrator.Entities
{
    /// <summary>
    /// Repräsentiert eine MPEG4-Video-Datei mit eingebetteten Metadaten.
    /// Enthält die Logik um die Metadaten auszulesen.
    /// </summary>
    public class Mpeg4VideoWithMetadata
    {
        public string Title { get; }
        public string TitleSort { get; }
        public string Description { get; }
        public uint? Year { get; }
        public string Album { get; }
        public byte[]? Artwork { get; }
        public string? ArtworkMimeType { get; }
        public Mpeg4Video Mpeg4Video { get; }

        private Mpeg4VideoWithMetadata(Mpeg4Video mpeg4Video, string title, string titleSort, string description, uint? year, string album, byte[]? artwork, string? artworkMimeType)
        {
            Mpeg4Video = mpeg4Video;
            Title = title;
            TitleSort = titleSort;
            Description = description;
            Year = year;
            Album = album;
            Artwork = artwork;
            ArtworkMimeType = artworkMimeType;
        }

        public static Result<Mpeg4VideoWithMetadata> Create(string? file)
        {
            var mpeg4Video = Mpeg4Video.Create(file);
            if (mpeg4Video.IsFailure)
                return Result.Failure<Mpeg4VideoWithMetadata>(mpeg4Video.Error);

            return Create(mpeg4Video.Value);
        }

        public static Result<Mpeg4VideoWithMetadata> Create(Mpeg4Video mpeg4Video)
        {
            try
            {
                // Erstelle ein TagLib-Objekt
                var tagLibFile = TagLib.File.Create(mpeg4Video.FileInfo.FullName);

                // Lies das Titelbild (Artwork) aus den Metadaten
                var tagLibPicture = tagLibFile.Tag.Pictures.ElementAtOrDefault(0);
                byte[]? artwork = tagLibPicture?.Data.Data;
                string? artworkMimeType = tagLibPicture?.MimeType;

                // Lies die restlichen Metadaten aus und erstelle ein Mpeg4VideoWithMetadata-Objekt
                return new Mpeg4VideoWithMetadata(mpeg4Video: mpeg4Video,
                                                  title: tagLibFile.Tag.Title,
                                                  titleSort: tagLibFile.Tag.TitleSort,
                                                  description: tagLibFile.Tag.Description,
                                                  year: tagLibFile.Tag.Year,
                                                  album: tagLibFile.Tag.Album,
                                                  artwork: artwork,
                                                  artworkMimeType: artworkMimeType);
            }
            catch (Exception ex)
            {
                return Result.Failure<Mpeg4VideoWithMetadata>(ex.Message);
            }
        }
    }
}