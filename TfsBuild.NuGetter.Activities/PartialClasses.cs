using System.Drawing;
using Microsoft.TeamFoundation.Build.Client;

// ==============================================================================================
// http://NuGetter.codeplex.com/
//
// Author: Mark S. Nichols
//
// Copyright (c) 2013 Mark Nichols
//
// This source is subject to the Microsoft Permissive License. 
// ==============================================================================================

namespace TfsBuild.NuGetter.Activities
{
    /// <summary>
    /// Validates and performs the pre-packaging step
    /// </summary>    
    [ToolboxBitmap(typeof(PerformPrePackage), "Resources.nugetter.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed partial class PerformPrePackage
    {
    }

    /// <summary>
    /// Package the library using NuGet
    /// </summary>    
    [ToolboxBitmap(typeof(PerformNuGetPack), "Resources.nugetter.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed partial class PerformNuGetPack
    {
    }

    
    /// <summary>
    /// PrePackage and then Package the library using NuGet
    /// </summary>    
    [ToolboxBitmap(typeof(NuGetterProcess), "Resources.nugetter.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed partial class NuGetterProcess
    {
    }
}
