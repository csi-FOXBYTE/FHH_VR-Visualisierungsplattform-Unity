using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FHH.Logic.Models
{

    [JsonConverter(typeof(StringEnumConverter))]
    public enum BaseLayerTypeDto
    {
        TILES3D,
        IMAGERY,
        TERRAIN
    }

    public class Vector3dDto
    {
        [JsonProperty("x")] public double X { get; set; }
        [JsonProperty("y")] public double Y { get; set; }
        [JsonProperty("z")] public double Z { get; set; }
    }

    public class ProjectDto
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("projectSasQueryParameters")] public string ProjectSasQueryParameters { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("maximumFlyingHeight")] public float MaximumFlyingHeight { get; set; }
        [JsonProperty("startingPoints")] public List<StartingPointDto> StartingPoints { get; set; }
        [JsonProperty("variants")] public List<VariantDto> Variants { get; set; }
    }

    public class StartingPointDto
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("img")] public string Img { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("origin")] public Vector3dDto Origin { get; set; }
        [JsonProperty("target")] public Vector3dDto Target { get; set; }
    }

    public class VariantDto
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("baseLayers")] public List<BaseLayerDto> BaseLayers { get; set; }
        [JsonProperty("clippingPolygons")] public List<ClippingPolygonDto> ClippingPolygons { get; set; }
        [JsonProperty("models")] public List<ModelRefDto> Models { get; set; }
    }

    public class BaseLayerDto
    {
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("type")] public BaseLayerTypeDto Type { get; set; }
    }

    public class ClippingPolygonDto
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("affectsTerrain")] public bool AffectsTerrain { get; set; }
        [JsonProperty("points")] public List<Vector3Dto> Points { get; set; }
    }

    public class ModelRefDto
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("attributes")] public Dictionary<string, string> Attributes { get; set; }
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("scale")] public Vector3Dto Scale { get; set; }
        [JsonProperty("translation")] public Vector3Dto Translation { get; set; }
        [JsonProperty("rotation")] public QuaternionDto Rotation { get; set; }
    }

    public class Vector3Dto
    {
        [JsonProperty("x")] public float X { get; set; }
        [JsonProperty("y")] public float Y { get; set; }
        [JsonProperty("z")] public float Z { get; set; }
    }
    
    public class QuaternionDto
    {
        [JsonProperty("x")] public float X { get; set; }
        [JsonProperty("y")] public float Y { get; set; }
        [JsonProperty("z")] public float Z { get; set; }
        [JsonProperty("w")] public float W { get; set; }
    }
}