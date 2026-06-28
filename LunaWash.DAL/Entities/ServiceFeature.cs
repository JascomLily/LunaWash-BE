using System;

namespace LunaWash.DAL.Entities;

public partial class ServiceFeature
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string WashServiceId { get; set; } = null!;

    public string FeatureText { get; set; } = null!;

    public int DisplayOrder { get; set; } = 0;

    public virtual WashService WashService { get; set; } = null!;
}
