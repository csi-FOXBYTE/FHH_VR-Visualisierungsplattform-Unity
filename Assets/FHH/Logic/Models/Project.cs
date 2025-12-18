using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace FHH.Logic.Models
{
    public enum BaseLayerType
    {
        Tiles3D,
        Imagery,
        Terrain
    }

    public class Vector3d
    {
        public double X;
        public double Y;
        public double Z;

        public Vector3d() { }
        public Vector3d(double x, double y, double z) { X = x; Y = y; Z = z; }
    }

    // Convenience wrapper to treat a list as "Projects"
    public class Projects : List<Project>
    {
        public static Projects FromJsonArray(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("json is null or empty", nameof(json));
            var dtos = JsonConvert.DeserializeObject<List<ProjectDto>>(json);
            var result = new Projects();
            if (dtos != null) foreach (var dto in dtos) result.Add(new Project(dto));
            return result;
        }

        public static Projects CreateMockList(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            var list = new Projects();
            for (int i = 1; i <= count; i++)
            {
                var p = Project.CreateMock();
                p.Id = $"proj-{i:D3}";
                p.Name = $"Sample Project {i}";
                list.Add(p);
            }
            return list;
        }
    }

    public class Project
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string ProjectSasQueryParameters { get; set; }
        public string Description { get; set; }
        public float MaximumFlyingHeight { get; set; }
        public List<StartingPoint> StartingPoints { get; set; }
        public List<ProjectVariant> Variants { get; set; }

        public Project() { }

        public Project(ProjectDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            Name = dto.Name;
            Id = dto.Id;
            ProjectSasQueryParameters = dto.ProjectSasQueryParameters;
            Description = dto.Description;
            MaximumFlyingHeight = dto.MaximumFlyingHeight;

            StartingPoints = new List<StartingPoint>();
            if (dto.StartingPoints != null)
            {
                foreach (var s in dto.StartingPoints)
                {
                    StartingPoints.Add(new StartingPoint
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Img = s.Img,
                        Description = s.Description,
                        //Origin = ToVector3(s.Origin),
                        //Target = ToVector3(s.Target)
                        Origin = ToVector3d(s.Origin),
                        Target = ToVector3d(s.Target)
                    });
                }
            }

            Variants = new List<ProjectVariant>();
            if (dto.Variants != null)
            {
                foreach (var v in dto.Variants)
                {
                    var variant = new ProjectVariant
                    {
                        Id = v.Id,
                        Name = v.Name,
                        BaseLayers = new List<ProjectBaseLayer>(),
                        ClippingPolygons = new List<ProjectClippingPolygon>(),
                        Models = new List<ProjectModelRef>()
                    };

                    if (v.BaseLayers != null)
                    {
                        foreach (var b in v.BaseLayers)
                        {
                            variant.BaseLayers.Add(new ProjectBaseLayer
                            {
                                Id = b.Id,
                                Name = b.Name,
                                Url = b.Url,
                                Type = MapBaseLayerType(b.Type)
                            });
                        }
                    }

                    if (v.ClippingPolygons != null)
                    {
                        foreach (var cp in v.ClippingPolygons)
                        {
                            var poly = new ProjectClippingPolygon
                            {
                                Id = cp.Id,
                                AffectsTerrain = cp.AffectsTerrain,
                                Points = new List<Vector3>()
                            };
                            if (cp.Points != null) foreach (var p in cp.Points) poly.Points.Add(ToVector3(p));
                            variant.ClippingPolygons.Add(poly);
                        }
                    }

                    if (v.Models != null)
                    {
                        foreach (var m in v.Models)
                        {
                            variant.Models.Add(new ProjectModelRef
                            {
                                Id = m.Id,
                                Url = m.Url,
                                Attributes = m.Attributes != null ? new Dictionary<string, string>(m.Attributes) : new Dictionary<string, string>(),
                                Scale = ToVector3(m.Scale),
                                Translation = ToVector3(m.Translation),
                                Rotation = ToQuaternion(m.Rotation),
                            });
                        }
                    }

                    Variants.Add(variant);
                }
            }
        }

        public static Project FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("json is null or empty", nameof(json));
            var dto = JsonConvert.DeserializeObject<ProjectDto>(json);
            if (dto == null) throw new JsonException("Invalid or empty Project JSON.");
            return new Project(dto);
        }

        public ProjectDto ToDto()
        {
            var dto = new ProjectDto
            {
                Name = Name,
                Id = Id,
                ProjectSasQueryParameters = ProjectSasQueryParameters,
                Description = Description,
                MaximumFlyingHeight = MaximumFlyingHeight,
                StartingPoints = new List<StartingPointDto>(),
                Variants = new List<VariantDto>()
            };

            if (StartingPoints != null)
            {
                foreach (var s in StartingPoints)
                {
                    dto.StartingPoints.Add(new StartingPointDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Img = s.Img,
                        Description = s.Description,
                        Origin = FromVector3d(s.Origin),
                        Target = FromVector3d(s.Target)
                    });
                }
            }

            if (Variants != null)
            {
                foreach (var v in Variants)
                {
                    var vDto = new VariantDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        BaseLayers = new List<BaseLayerDto>(),
                        ClippingPolygons = new List<ClippingPolygonDto>(),
                        Models = new List<ModelRefDto>()
                    };

                    if (v.BaseLayers != null)
                    {
                        foreach (var b in v.BaseLayers)
                        {
                            vDto.BaseLayers.Add(new BaseLayerDto
                            {
                                Id = b.Id,
                                Name = b.Name,
                                Url = b.Url,
                                Type = MapBaseLayerType(b.Type)
                            });
                        }
                    }

                    if (v.ClippingPolygons != null)
                    {
                        foreach (var cp in v.ClippingPolygons)
                        {
                            var cpDto = new ClippingPolygonDto
                            {
                                Id = cp.Id,
                                AffectsTerrain = cp.AffectsTerrain,
                                Points = new List<Vector3Dto>()
                            };
                            if (cp.Points != null) foreach (var p in cp.Points) cpDto.Points.Add(FromVector3(p));
                            vDto.ClippingPolygons.Add(cpDto);
                        }
                    }

                    if (v.Models != null)
                    {
                        foreach (var m in v.Models)
                        {
                            vDto.Models.Add(new ModelRefDto
                            {
                                Id = m.Id,
                                Url = m.Url,
                                Attributes = m.Attributes != null ? new Dictionary<string, string>(m.Attributes) : new Dictionary<string, string>(),
                                Scale = FromVector3(m.Scale),
                                Translation = FromVector3(m.Translation),
                                Rotation = FromQuaternion(m.Rotation),
                            });
                        }
                    }

                    dto.Variants.Add(vDto);
                }
            }

            return dto;
        }

        public static Project CreateMock()
        {
            var dto = new ProjectDto
            {
                Name = "Sample City",
                Id = "proj-001",
                //MyRole = MyRoleDto.MODERATOR,
                ProjectSasQueryParameters = "",
                Description = "Demo project for testing.",
                MaximumFlyingHeight = 1000.0f,
                StartingPoints = new List<StartingPointDto>
                {
                    new StartingPointDto
                    {
                        Id = "sp-1", Name = "Plaza", Img = "https://example.com/plaza.png", Description = "Central spawn",
                        Origin = new Vector3dDto { X = 0, Y = 2, Z = -10 },
                        Target = new Vector3dDto { X = 0, Y = 2, Z = 0 }
                    }
                },
                Variants = new List<VariantDto>
                {
                    new VariantDto
                    {
                        Id = "v-1", Name = "Default",
                        BaseLayers = new List<BaseLayerDto>
                        {
                            new BaseLayerDto { Id = "bl-tiles", Name = "City 3D Tiles", Url = "https://example.com/tiles", Type = BaseLayerTypeDto.TILES3D }
                        },
                        ClippingPolygons = new List<ClippingPolygonDto>
                        {
                            new ClippingPolygonDto
                            {
                                Id = "clip-1", AffectsTerrain = true,
                                Points = new List<Vector3Dto>
                                {
                                    new Vector3Dto { X = -10, Y = 0, Z = -10 },
                                    new Vector3Dto { X =  10, Y = 0, Z = -10 },
                                    new Vector3Dto { X =  10, Y = 0, Z =  10 },
                                    new Vector3Dto { X = -10, Y = 0, Z =  10 }
                                }
                            }
                        },
                        Models = new List<ModelRefDto>
                        {
                            new ModelRefDto
                            {
                                Id = "m-1", Url = "https://example.com/models/tower.glb",
                                Attributes = new Dictionary<string, string> { { "floors", "12" }, { "usage", "mixed" } },
                                Scale = new Vector3Dto { X = 1, Y = 1, Z = 1 },
                                Translation = new Vector3Dto { X = 0, Y = 0, Z = 0 },
                                Rotation = new QuaternionDto { X = 0, Y = 0.7071f, Z = 0, W = 0.7071f }
                            }
                        }
                    }
                }
            };
            return new Project(dto);
        }

        // Helpers
        private static Vector3 ToVector3(Vector3Dto v)
        {
            if (v == null) return Vector3.zero;
            return new Vector3(v.X, v.Y, v.Z);
        }

        private static Quaternion ToQuaternion(QuaternionDto q)
        {
            if (q == null) return Quaternion.identity;
            return new Quaternion(q.X, q.Y, q.Z, q.W);
        }

        private static Vector3Dto FromVector3(Vector3 v)
        {
            return new Vector3Dto { X = v.x, Y = v.y, Z = v.z };
        }

        private static Vector3d ToVector3d(Vector3dDto v)
        {
            if (v == null) return null;
            return new Vector3d(v.X, v.Y, v.Z);
        }

        private static Vector3dDto FromVector3d(Vector3d v) 
        {
            if (v == null) return null; 
            return new Vector3dDto { X = v.X, Y = v.Y, Z = v.Z }; 
        }

        private static QuaternionDto FromQuaternion(Quaternion q)
        {
            return new QuaternionDto { X = q.x, Y = q.y, Z = q.z, W = q.w };
        }
        private static BaseLayerType MapBaseLayerType(BaseLayerTypeDto t)
        {
            switch (t)
            {
                case BaseLayerTypeDto.TILES3D: return BaseLayerType.Tiles3D;
                case BaseLayerTypeDto.IMAGERY: return BaseLayerType.Imagery;
                case BaseLayerTypeDto.TERRAIN: return BaseLayerType.Terrain;
                default: return BaseLayerType.Tiles3D;
            }
        }

        private static BaseLayerTypeDto MapBaseLayerType(BaseLayerType t)
        {
            switch (t)
            {
                case BaseLayerType.Tiles3D: return BaseLayerTypeDto.TILES3D;
                case BaseLayerType.Imagery: return BaseLayerTypeDto.IMAGERY;
                case BaseLayerType.Terrain: return BaseLayerTypeDto.TERRAIN;
                default: return BaseLayerTypeDto.TILES3D;
            }
        }
    }

    public class StartingPoint
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public string Description { get; set; }
        public Vector3d Origin { get; set; }
        public Vector3d Target { get; set; }
    }

    public class ProjectVariant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ProjectBaseLayer> BaseLayers { get; set; }
        public List<ProjectClippingPolygon> ClippingPolygons { get; set; }
        public List<ProjectModelRef> Models { get; set; }
    }

    public class ProjectBaseLayer
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public BaseLayerType Type { get; set; }
    }

    public class ProjectClippingPolygon
    {
        public string Id { get; set; }
        public bool AffectsTerrain { get; set; }
        public List<Vector3> Points { get; set; }
    }

    public class ProjectModelRef
    {
        public string Id { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string Url { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Translation { get; set; }
        public Quaternion Rotation { get; set; }
    }
}