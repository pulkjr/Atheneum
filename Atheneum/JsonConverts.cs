using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Atheneum;
public class FileInfoConverter : JsonConverter<FileInfo>
{
    public override FileInfo Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
            new FileInfo(reader.GetString()!);

    public override void Write(
        Utf8JsonWriter writer,
        FileInfo fileInfo,
        JsonSerializerOptions options) =>
            writer.WriteStringValue(fileInfo.FullName);
}
public class DirectoryInfoConverter : JsonConverter<DirectoryInfo>
{
    public override DirectoryInfo Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
            new DirectoryInfo(reader.GetString()!);

    public override void Write(
        Utf8JsonWriter writer,
        DirectoryInfo directoryInfo,
        JsonSerializerOptions options) =>
            writer.WriteStringValue(directoryInfo.FullName);
}