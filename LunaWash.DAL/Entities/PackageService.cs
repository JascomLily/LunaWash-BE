using System;

namespace LunaWash.DAL.Entities;

public partial class PackageService
{
    public string PackageId { get; set; } = null!;

    public string ServiceId { get; set; } = null!;

    public virtual ServicePackage ServicePackage { get; set; } = null!;

    public virtual WashService WashService { get; set; } = null!;
}
